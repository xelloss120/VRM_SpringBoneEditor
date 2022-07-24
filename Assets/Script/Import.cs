using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using VRM;

public class Import : MonoBehaviour
{
    public GameObject Root;

    [SerializeField] GameObject BonePrefab;

    [SerializeField] Text BoneName;
    [SerializeField] Dropdown Dropdown;

    [SerializeField] Transform Camera;

    public async void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open VRM", "", "", false);
        if (paths.Length == 0) return;

        var instance = await VrmUtility.LoadAsync(paths[0]);

        instance.EnableUpdateWhenOffscreen();
        instance.ShowMeshes();

        foreach (var bone in Bone.List)
        {
            Destroy(bone.gameObject);
        }
        Bone.List.Clear();
        Destroy(Root);

        Root = instance.Root;

        SetSpring();
        SetBone();

        Bone.Camera = Camera;
    }

    void SetSpring()
    {
        var springs = Root.GetComponentsInChildren<VRMSpringBone>();
        var normalSet = true;
        var softSet = true;
        var hardSet = true;
        foreach (var spring in springs)
        {
            if (spring.m_comment == Editor.VRMSB_Normal)
            {
                normalSet = false;
            }
            if (spring.m_comment == Editor.VRMSB_Soft)
            {
                softSet = false;
            }
            if (spring.m_comment == Editor.VRMSB_Hard)
            {
                hardSet = false;
            }
        }

        var secondary = Root.transform.Find("secondary").gameObject;
        if (normalSet)
        {
            var normal = secondary.AddComponent<VRMSpringBone>();
            normal.m_comment = Editor.VRMSB_Normal;
            normal.m_stiffnessForce = 1.0f;
        }
        if (softSet)
        {
            var soft = secondary.AddComponent<VRMSpringBone>();
            soft.m_comment = Editor.VRMSB_Soft;
            soft.m_stiffnessForce = 0.2f;
        }
        if (hardSet)
        {
            var hard = secondary.AddComponent<VRMSpringBone>();
            hard.m_comment = Editor.VRMSB_Hard;
            hard.m_stiffnessForce = 4.0f;
        }

        var list = new List<string>();
        list.Add(Editor.VRMSB_None);

        var no = 0;
        springs = Root.GetComponentsInChildren<VRMSpringBone>();
        foreach (var spring in springs)
        {
            if (spring.m_comment == null || spring.m_comment == "")
            {
                spring.m_comment = Editor.VRMSB_Set + no;
                no++;
            }
            list.Add(spring.m_comment);
        }
        list.Add(Editor.VRMSB_Collider);

        Dropdown.ClearOptions();
        Dropdown.AddOptions(list);
    }

    void SetBone()
    {
        var springs = Root.GetComponentsInChildren<VRMSpringBone>();
        var transforms = Root.GetComponentsInChildren<Transform>();
        foreach (var transform in transforms)
        {
            // 重複確認
            bool check = false;
            foreach(var b in Bone.List)
            {
                if (b.Target.position == transform.position)
                {
                    //Debug.Log("重複発生" + b.Target.name + " : " + transform.name);
                    check = true;
                }
            }
            if (check)
            {
                continue;
            }

            var obj = Instantiate(BonePrefab);
            var bone = obj.GetComponent<Bone>();
            bone.Target = transform;
            var state = Bone.StateType.None;
            foreach (var spring in springs)
            {
                foreach (var b in spring.RootBones)
                {
                    if (transform == b)
                    {
                        bone.SetTest(spring.m_comment);
                        state = Bone.StateType.SpringBone;
                    }
                }
                if (spring.ColliderGroups == null) continue;
                foreach (var b in spring.ColliderGroups)
                {
                    if (transform == b.transform)
                    {
                        state = Bone.StateType.Collider;
                    }
                }
            }
            bone.Setting(state);
            Bone.List.Add(bone);
        }

        BoneName.text = "BoneName";
    }
}

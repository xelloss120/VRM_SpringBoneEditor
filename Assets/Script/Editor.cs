using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Editor : MonoBehaviour
{
    public const string VRMSB_None = "Ç»Çµ";
    public const string VRMSB_Normal = "Ç”Ç¬Ç§";
    public const string VRMSB_Soft = "Ç”ÇÒÇÌÇË";
    public const string VRMSB_Hard = "Ç©ÇΩÇﬂ";
    public const string VRMSB_Set = "ê›íËçœÇ›";
    public const string VRMSB_Collider = "ìñÇΩÇËîªíË";

    public bool MouseSelect = false;

    [SerializeField] Import Import;
    [SerializeField] Mouse Mouse;
    [SerializeField] Select Select;
    [SerializeField] Dropdown Dropdown;

    [SerializeField] InputField Size;
    [SerializeField] InputField OffsetX;
    [SerializeField] InputField OffsetY;
    [SerializeField] InputField OffsetZ;

    public void OnValueChanged(int value)
    {
        if (MouseSelect)
        {
            return;
        }

        var text = Dropdown.options[value].text;

        var springs = Import.Root.GetComponentsInChildren<VRMSpringBone>();
        switch (text)
        {
            case VRMSB_None:
                None(springs, Select.Bone);
                break;
            case VRMSB_Collider:
                Collider(springs, Select.Bone);
                break;
            default:
                Spring(springs, Select.Bone, text);
                break;
        }
    }

    void None(VRMSpringBone[] springs, Bone bone)
    {
        bool delete;
        foreach (var spring in springs)
        {
            delete = false;
            foreach (var b in spring.RootBones)
            {
                if (bone.Target == b)
                {
                    delete = true;
                }
            }
            if (delete)
            {
                var transforms = Import.Root.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    if (transform.position == bone.Target.position)
                    {
                        spring.RootBones.Remove(transform);
                    }
                }

                spring.Setup();
                bone.State = Bone.StateType.None;
            }
            delete = false;
            if (spring.ColliderGroups != null)
            {
                foreach (var b in spring.ColliderGroups)
                {
                    if (bone.Target == b.gameObject.transform)
                    {
                        delete = true;
                    }
                }
                if (delete)
                {
                    var collider = bone.Target.gameObject.GetComponent<VRMSpringBoneColliderGroup>();
                    var list = spring.ColliderGroups.ToList();
                    list.Remove(collider);
                    spring.ColliderGroups = list.ToArray();
                    DestroyImmediate(collider);
                    bone.State = Bone.StateType.None;
                    bone.Select();
                }
            }
        }
        bone.SetTest("");
    }

    void Spring(VRMSpringBone[] springs, Bone bone, string comment)
    {
        None(springs, bone);

        foreach (var spring in springs)
        {
            if (spring.m_comment == comment)
            {
                spring.RootBones.Add(bone.Target);

                var transforms = Import.Root.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    if (transform.position == bone.Target.position && transform != bone.Target)
                    {
                        spring.RootBones.Add(transform);
                        break;
                    }
                }

                bone.State = Bone.StateType.SpringBone;
                spring.Setup();
            }
        }

        bone.SetTest(comment);
    }

    void Collider(VRMSpringBone[] springs, Bone bone)
    {
        None(springs, bone);

        var tmp = bone.Target.gameObject.AddComponent<VRMSpringBoneColliderGroup>();

        foreach (var spring in springs)
        {
            List<VRMSpringBoneColliderGroup> list;
            if (spring.ColliderGroups != null)
            {
                list = spring.ColliderGroups.ToList();
            }
            else
            {
                list = new List<VRMSpringBoneColliderGroup>();
            }
            list.Add(tmp);
            spring.ColliderGroups = list.ToArray();
        }

        bone.State = Bone.StateType.Collider;
        bone.Select();

        Size.interactable = true;
        OffsetX.interactable = true;
        OffsetY.interactable = true;
        OffsetZ.interactable = true;

        Size.text = tmp.Colliders[0].Radius.ToString();
        OffsetX.text = tmp.Colliders[0].Offset.x.ToString();
        OffsetY.text = tmp.Colliders[0].Offset.y.ToString();
        OffsetZ.text = tmp.Colliders[0].Offset.z.ToString();
    }

    public void OnValueChangedSize(string text)
    {
        if (MouseSelect)
        {
            return;
        }
        var collider = Select.Bone.Target.GetComponent<VRMSpringBoneColliderGroup>();
        collider.Colliders[0].Radius = float.Parse(text);
        Select.Bone.Select();
    }

    public void OnValueChangedOffsetX(string text)
    {
        if (MouseSelect)
        {
            return;
        }
        var collider = Select.Bone.Target.GetComponent<VRMSpringBoneColliderGroup>();
        var offset = collider.Colliders[0].Offset;
        offset.x = float.Parse(text);
        collider.Colliders[0].Offset = offset;
        Select.Bone.Select();
    }

    public void OnValueChangedOffsetY(string text)
    {
        if (MouseSelect)
        {
            return;
        }
        var collider = Select.Bone.Target.GetComponent<VRMSpringBoneColliderGroup>();
        var offset = collider.Colliders[0].Offset;
        offset.y = float.Parse(text);
        collider.Colliders[0].Offset = offset;
        Select.Bone.Select();
    }

    public void OnValueChangedOffsetZ(string text)
    {
        if (MouseSelect)
        {
            return;
        }
        var collider = Select.Bone.Target.GetComponent<VRMSpringBoneColliderGroup>();
        var offset = collider.Colliders[0].Offset;
        offset.z = float.Parse(text);
        collider.Colliders[0].Offset = offset;
        Select.Bone.Select();
    }
}

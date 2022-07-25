using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using VRM;

public class Import : MonoBehaviour
{
    public GameObject Root;

    [SerializeField] GameObject BonePrefab;
    [SerializeField] Dropdown Dropdown;
    [SerializeField] Transform Camera;

    void Start()
    {
        Bone.Camera = Camera;
    }

    public async void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open VRM", "", "", false);
        if (paths.Length == 0) return;

        var instance = await VrmUtility.LoadAsync(paths[0]);

        instance.EnableUpdateWhenOffscreen();
        instance.ShowMeshes();

        Destroy(Root);
        Root = instance.Root;

        InitSpring();
        InitBone();
    }

    /// <summary>
    /// �h�ꕨ�Ɋւ��鏉����
    /// </summary>
    void InitSpring()
    {
        // �{�c�[���̗h�ꕨ�v���Z�b�g�L�����m�F
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

        // �{�c�[���̗h�ꕨ�v���Z�b�g��������Βǉ�
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

        // Dropdown�̏�����
        var list = new List<string>();
        list.Add(Editor.VRMSB_None);

        var no = 0;
        springs = Root.GetComponentsInChildren<VRMSpringBone>();
        foreach (var spring in springs)
        {
            // �h�ꕨ�̐ݒ薼��Dropdown�ɒǉ�
            if (spring.m_comment == null || spring.m_comment == "")
            {
                // �����̏ꍇ�͘A�Ԗ���t�^
                spring.m_comment = Editor.VRMSB_Set + no;
                no++;
            }
            list.Add(spring.m_comment);
        }

        // �����蔻��p�̑I�����ڂ�ǉ�
        list.Add(Editor.VRMSB_Collider);

        Dropdown.ClearOptions();
        Dropdown.AddOptions(list);
    }

    /// <summary>
    /// ����p�̃{�[��UI��������
    /// </summary>
    void InitBone()
    {
        // �S�폜
        foreach (var bone in Bone.List)
        {
            Destroy(bone.gameObject);
        }
        Bone.List.Clear();

        // �ǉ�
        var springs = Root.GetComponentsInChildren<VRMSpringBone>();
        var transforms = Root.GetComponentsInChildren<Transform>();
        foreach (var transform in transforms)
        {
            // �{�[���ʒu�̏d���m�F
            bool duplicate = false;
            foreach(var item in Bone.List)
            {
                if (item.Target.position == transform.position)
                {
                    duplicate = true;
                }
            }
            if (duplicate)
            {
                // �d������ꍇ�͒��f
                continue;
            }

            // �{�[��UI�̐���
            var obj = Instantiate(BonePrefab);
            var bone = obj.GetComponent<Bone>();
            bone.Target = transform;

            // �{�[����Ԃ̎擾
            bone.SetState(Bone.StateType.None);
            foreach (var spring in springs)
            {
                foreach (var item in spring.RootBones)
                {
                    if (transform == item)
                    {
                        // �h�ꕨ
                        bone.SetText(spring.m_comment);
                        bone.SetState(Bone.StateType.SpringBone);
                    }
                }
                if (spring.ColliderGroups == null) continue;
                foreach (var item in spring.ColliderGroups)
                {
                    if (transform == item.transform)
                    {
                        // �����蔻��
                        bone.SetState(Bone.StateType.Collider);
                    }
                }
            }

            Bone.List.Add(bone);
        }
    }
}

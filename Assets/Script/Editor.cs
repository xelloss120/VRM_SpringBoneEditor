using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Editor : MonoBehaviour
{
    public const string VRMSB_None = "�Ȃ�";
    public const string VRMSB_Normal = "�ӂ�";
    public const string VRMSB_Soft = "�ӂ���";
    public const string VRMSB_Hard = "������";
    public const string VRMSB_Set = "�ݒ�ς�";
    public const string VRMSB_Collider = "�����蔻��";

    [SerializeField] Import Import;
    [SerializeField] Select Select;

    [SerializeField] Dropdown Dropdown;

    [SerializeField] InputField Size;
    [SerializeField] InputField OffsetX;
    [SerializeField] InputField OffsetY;
    [SerializeField] InputField OffsetZ;

    public void OnValueChanged(int value)
    {
        if (Select.Changing)
        {
            return;
        }

        var text = Dropdown.options[value].text;

        var springs = Import.Root.GetComponentsInChildren<VRMSpringBone>();
        switch (text)
        {
            case VRMSB_None:
                Delete(springs);
                break;
            case VRMSB_Collider:
                Collider(springs);
                break;
            default:
                Spring(springs, text);
                break;
        }
    }

    /// <summary>
    /// �h�ꕨ�܂��͓����蔻��̐ݒ���폜
    /// </summary>
    void Delete(VRMSpringBone[] springs)
    {
        bool delete;
        bool deleteCollider = false;
        foreach (var spring in springs)
        {
            // �h�ꕨ
            delete = false;
            foreach (var bone in spring.RootBones)
            {
                if (Bone.Selected.Target == bone)
                {
                    // �h�ꕨ�ݒ肳��Ă���{�[���Ȃ��ō폜
                    delete = true;
                }
            }
            if (delete)
            {
                var transforms = Import.Root.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    if (Bone.Selected.Target.position == transform.position)
                    {
                        // ����p�{�[��UI�̑ΏۂƓ����ʒu�̃{�[�����폜�i�����ւ��ȂǂɑΉ����邽�߁j
                        spring.RootBones.Remove(transform);
                    }
                }
                // �\������폜
                spring.Setup();
                Bone.Selected.State = Bone.StateType.None;
                Bone.Selected.SetText("");
            }

            // �����蔻��
            delete = false;
            if (spring.ColliderGroups != null)
            {
                foreach (var item in spring.ColliderGroups)
                {
                    if (Bone.Selected.Target == item.transform)
                    {
                        // �����蔻��ݒ肳��Ă���{�[���Ȃ��ō폜
                        deleteCollider = delete = true;
                        
                    }
                }
                if (delete)
                {
                    // �����蔻��̃R���|�[�l���g���̂͌�ō폜�i���ڈȍ~���񂷂��߁j
                    var list = spring.ColliderGroups.ToList();
                    var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
                    list.Remove(collider);
                    spring.ColliderGroups = list.ToArray();
                }
            }
        }
        if (deleteCollider)
        {
            // �����蔻��̃R���|�[�l���g���폜
            var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
            DestroyImmediate(collider);

            // �\������폜
            Bone.Selected.State = Bone.StateType.None;
            Bone.Selected.UpdateCollider();
        }
    }

    /// <summary>
    /// �h�ꕨ��ݒ�
    /// </summary>
    void Spring(VRMSpringBone[] springs, string comment)
    {
        // �ݒ�ς݂̃{�[��������΍폜
        Delete(springs);

        foreach (var spring in springs)
        {
            if (spring.m_comment == comment)
            {
                var transforms = Import.Root.GetComponentsInChildren<Transform>();
                foreach (var transform in transforms)
                {
                    if (Bone.Selected.Target.position == transform.position)
                    {
                        // ����p�{�[��UI�̑ΏۂƓ����ʒu�̃{�[����h�ꕨ�Ƃ��Đݒ�i�����ւ��ȂǂɑΉ����邽�߁j
                        spring.RootBones.Add(transform);
                    }
                }

                // �\������ݒ�
                spring.Setup();
                Bone.Selected.State = Bone.StateType.SpringBone;
                Bone.Selected.SetText(comment);
            }
        }
    }

    /// <summary>
    /// �����蔻��̐ݒ�
    /// </summary>
    void Collider(VRMSpringBone[] springs)
    {
        // �ݒ�ς݂̃{�[��������΍폜
        Delete(springs);

        var collider = Bone.Selected.Target.gameObject.AddComponent<VRMSpringBoneColliderGroup>();

        foreach (var spring in springs)
        {
            // �S�Ă̗h�ꕨ�ɓ����蔻���ݒ�
            List<VRMSpringBoneColliderGroup> list;
            if (spring.ColliderGroups != null)
            {
                list = spring.ColliderGroups.ToList();
            }
            else
            {
                list = new List<VRMSpringBoneColliderGroup>();
            }
            list.Add(collider);
            spring.ColliderGroups = list.ToArray();
        }

        // �\������ݒ�
        Bone.Selected.State = Bone.StateType.Collider;
        Bone.Selected.UpdateCollider();

        Size.interactable = true;
        OffsetX.interactable = true;
        OffsetY.interactable = true;
        OffsetZ.interactable = true;

        Size.text = collider.Colliders[0].Radius.ToString();
        OffsetX.text = collider.Colliders[0].Offset.x.ToString();
        OffsetY.text = collider.Colliders[0].Offset.y.ToString();
        OffsetZ.text = collider.Colliders[0].Offset.z.ToString();
    }

    public void OnValueChangedSize(string text)
    {
        if (Select.Changing)
        {
            return;
        }
        var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (float.TryParse(text, out float value))
        {
            collider.Colliders[0].Radius = value;
            Bone.Selected.UpdateCollider();
        }
    }

    public void OnValueChangedOffsetX(string text)
    {
        if (Select.Changing)
        {
            return;
        }
        var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (float.TryParse(text, out float value))
        {
            var offset = collider.Colliders[0].Offset;
            offset.x = value;
            collider.Colliders[0].Offset = offset;
            Bone.Selected.UpdateCollider();
        }
    }

    public void OnValueChangedOffsetY(string text)
    {
        if (Select.Changing)
        {
            return;
        }
        var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (float.TryParse(text, out float value))
        {
            var offset = collider.Colliders[0].Offset;
            offset.y = value;
            collider.Colliders[0].Offset = offset;
            Bone.Selected.UpdateCollider();
        }
    }

    public void OnValueChangedOffsetZ(string text)
    {
        if (Select.Changing)
        {
            return;
        }
        var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (float.TryParse(text, out float value))
        {
            var offset = collider.Colliders[0].Offset;
            offset.z = value;
            collider.Colliders[0].Offset = offset;
            Bone.Selected.UpdateCollider();
        }
    }
}

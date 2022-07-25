using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Editor : MonoBehaviour
{
    public const string VRMSB_None = "なし";
    public const string VRMSB_Normal = "ふつう";
    public const string VRMSB_Soft = "ふんわり";
    public const string VRMSB_Hard = "かため";
    public const string VRMSB_Set = "設定済み";
    public const string VRMSB_Collider = "当たり判定";

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
    /// 揺れ物または当たり判定の設定を削除
    /// </summary>
    void Delete(VRMSpringBone[] springs)
    {
        bool delete;
        bool deleteCollider = false;
        foreach (var spring in springs)
        {
            // 揺れ物
            delete = false;
            foreach (var bone in spring.RootBones)
            {
                if (Bone.Selected.Target == bone)
                {
                    // 揺れ物設定されているボーンなら後で削除
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
                        // 操作用ボーンUIの対象と同じ位置のボーンを削除（着せ替えなどに対応するため）
                        spring.RootBones.Remove(transform);
                    }
                }
                // 表示上も削除
                spring.Setup();
                Bone.Selected.State = Bone.StateType.None;
                Bone.Selected.SetText("");
            }

            // 当たり判定
            delete = false;
            if (spring.ColliderGroups != null)
            {
                foreach (var item in spring.ColliderGroups)
                {
                    if (Bone.Selected.Target == item.transform)
                    {
                        // 当たり判定設定されているボーンなら後で削除
                        deleteCollider = delete = true;
                        
                    }
                }
                if (delete)
                {
                    // 当たり判定のコンポーネント自体は後で削除（二回目以降を回すため）
                    var list = spring.ColliderGroups.ToList();
                    var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
                    list.Remove(collider);
                    spring.ColliderGroups = list.ToArray();
                }
            }
        }
        if (deleteCollider)
        {
            // 当たり判定のコンポーネントを削除
            var collider = Bone.Selected.Target.GetComponent<VRMSpringBoneColliderGroup>();
            DestroyImmediate(collider);

            // 表示上も削除
            Bone.Selected.State = Bone.StateType.None;
            Bone.Selected.UpdateCollider();
        }
    }

    /// <summary>
    /// 揺れ物を設定
    /// </summary>
    void Spring(VRMSpringBone[] springs, string comment)
    {
        // 設定済みのボーンがあれば削除
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
                        // 操作用ボーンUIの対象と同じ位置のボーンを揺れ物として設定（着せ替えなどに対応するため）
                        spring.RootBones.Add(transform);
                    }
                }

                // 表示上も設定
                spring.Setup();
                Bone.Selected.State = Bone.StateType.SpringBone;
                Bone.Selected.SetText(comment);
            }
        }
    }

    /// <summary>
    /// 当たり判定の設定
    /// </summary>
    void Collider(VRMSpringBone[] springs)
    {
        // 設定済みのボーンがあれば削除
        Delete(springs);

        var collider = Bone.Selected.Target.gameObject.AddComponent<VRMSpringBoneColliderGroup>();

        foreach (var spring in springs)
        {
            // 全ての揺れ物に当たり判定を設定
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

        // 表示上も設定
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

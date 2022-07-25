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
    /// 揺れ物に関する初期化
    /// </summary>
    void InitSpring()
    {
        // 本ツールの揺れ物プリセット有無を確認
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

        // 本ツールの揺れ物プリセットが無ければ追加
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

        // Dropdownの初期化
        var list = new List<string>();
        list.Add(Editor.VRMSB_None);

        var no = 0;
        springs = Root.GetComponentsInChildren<VRMSpringBone>();
        foreach (var spring in springs)
        {
            // 揺れ物の設定名をDropdownに追加
            if (spring.m_comment == null || spring.m_comment == "")
            {
                // 無名の場合は連番名を付与
                spring.m_comment = Editor.VRMSB_Set + no;
                no++;
            }
            list.Add(spring.m_comment);
        }

        // 当たり判定用の選択項目を追加
        list.Add(Editor.VRMSB_Collider);

        Dropdown.ClearOptions();
        Dropdown.AddOptions(list);
    }

    /// <summary>
    /// 操作用のボーンUIを初期化
    /// </summary>
    void InitBone()
    {
        // 全削除
        foreach (var bone in Bone.List)
        {
            Destroy(bone.gameObject);
        }
        Bone.List.Clear();

        // 追加
        var springs = Root.GetComponentsInChildren<VRMSpringBone>();
        var transforms = Root.GetComponentsInChildren<Transform>();
        foreach (var transform in transforms)
        {
            // ボーン位置の重複確認
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
                // 重複する場合は中断
                continue;
            }

            // ボーンUIの生成
            var obj = Instantiate(BonePrefab);
            var bone = obj.GetComponent<Bone>();
            bone.Target = transform;

            // ボーン状態の取得
            bone.SetState(Bone.StateType.None);
            foreach (var spring in springs)
            {
                foreach (var item in spring.RootBones)
                {
                    if (transform == item)
                    {
                        // 揺れ物
                        bone.SetText(spring.m_comment);
                        bone.SetState(Bone.StateType.SpringBone);
                    }
                }
                if (spring.ColliderGroups == null) continue;
                foreach (var item in spring.ColliderGroups)
                {
                    if (transform == item.transform)
                    {
                        // 当たり判定
                        bone.SetState(Bone.StateType.Collider);
                    }
                }
            }

            Bone.List.Add(bone);
        }
    }
}

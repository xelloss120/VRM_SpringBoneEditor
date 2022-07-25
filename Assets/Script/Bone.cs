using System.Collections.Generic;
using UnityEngine;
using VRM;
using TMPro;

public class Bone : MonoBehaviour
{
    public static List<Bone> List = new List<Bone>();
    public static Transform Camera;
    public static Bone Selected;

    public Transform Target;

    public enum StateType
    {
        None,
        SpringBone,
        Collider
    }
    public StateType State;

    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject Line;
    [SerializeField] GameObject Collider;
    [SerializeField] TextMeshPro TextMeshPro;

    [SerializeField] Material BoneC;
    [SerializeField] Material BoneM;
    [SerializeField] Material BoneY;
    [SerializeField] Material BoneR;

    void Update()
    {
        transform.position = Target.position;

        if (Target.parent == null) return;

        // 対象と対象の親の中間点にライン用オブジェクトを配置
        var position = (transform.position + Target.parent.position) / 2;
        Line.transform.position = position;

        // ライン用オブジェクトの大きさを対象と対象の親の距離に設定
        var scale = Line.transform.localScale;
        scale.z = Vector3.Distance(transform.position, Target.parent.position);
        Line.transform.localScale = scale;

        // ライン用オブジェクトを対象の親に向けて繋がっているように見せる
        Line.transform.LookAt(Target.parent);

        if (TextMeshPro.text != "")
        {
            // 揺れ物の設定がされている場合はテキストをカメラに向ける
            TextMeshPro.transform.LookAt(Camera);
        }
    }

    /// <summary>
    /// 当たり判定用オブジェクトの表示を更新
    /// </summary>
    public void UpdateCollider()
    {
        var collider = Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (collider == null)
        {
            Collider.SetActive(false);
        }
        else
        {
            Collider.SetActive(true);
            Collider.transform.localPosition = collider.Colliders[0].Offset;
            Collider.transform.localScale = Vector3.one * collider.Colliders[0].Radius;
        }
    }

    /// <summary>
    /// 選択された場合の処理
    /// </summary>
    public void Select()
    {
        // 表示をリセット
        foreach (var bone in List)
        {
            bone.SetState(bone.State);
            bone.Collider.SetActive(false);
        }

        // 選択オブジェクトは赤色表示
        Sphere.GetComponent<MeshRenderer>().material = BoneR;

        UpdateCollider();

        Selected = this;
    }

    /// <summary>
    /// 揺れ物設定の状態を設定
    /// </summary>
    public void SetState(StateType state)
    {
        State = state;

        // 状態に合わせた色に設定
        switch (State)
        {
            case StateType.None:
                Sphere.GetComponent<MeshRenderer>().material = BoneC;
                break;
            case StateType.SpringBone:
                Sphere.GetComponent<MeshRenderer>().material = BoneY;
                break;
            case StateType.Collider:
                Sphere.GetComponent<MeshRenderer>().material = BoneM;
                break;
        }
    }

    /// <summary>
    /// 揺れ物設定名の設定
    /// </summary>
    public void SetText(string text)
    {
        TextMeshPro.text = text;
    }
}

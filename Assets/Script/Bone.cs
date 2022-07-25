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

        // �ΏۂƑΏۂ̐e�̒��ԓ_�Ƀ��C���p�I�u�W�F�N�g��z�u
        var position = (transform.position + Target.parent.position) / 2;
        Line.transform.position = position;

        // ���C���p�I�u�W�F�N�g�̑傫����ΏۂƑΏۂ̐e�̋����ɐݒ�
        var scale = Line.transform.localScale;
        scale.z = Vector3.Distance(transform.position, Target.parent.position);
        Line.transform.localScale = scale;

        // ���C���p�I�u�W�F�N�g��Ώۂ̐e�Ɍ����Čq�����Ă���悤�Ɍ�����
        Line.transform.LookAt(Target.parent);

        if (TextMeshPro.text != "")
        {
            // �h�ꕨ�̐ݒ肪����Ă���ꍇ�̓e�L�X�g���J�����Ɍ�����
            TextMeshPro.transform.LookAt(Camera);
        }
    }

    /// <summary>
    /// �����蔻��p�I�u�W�F�N�g�̕\�����X�V
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
    /// �I�����ꂽ�ꍇ�̏���
    /// </summary>
    public void Select()
    {
        // �\�������Z�b�g
        foreach (var bone in List)
        {
            bone.SetState(bone.State);
            bone.Collider.SetActive(false);
        }

        // �I���I�u�W�F�N�g�͐ԐF�\��
        Sphere.GetComponent<MeshRenderer>().material = BoneR;

        UpdateCollider();

        Selected = this;
    }

    /// <summary>
    /// �h�ꕨ�ݒ�̏�Ԃ�ݒ�
    /// </summary>
    public void SetState(StateType state)
    {
        State = state;

        // ��Ԃɍ��킹���F�ɐݒ�
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
    /// �h�ꕨ�ݒ薼�̐ݒ�
    /// </summary>
    public void SetText(string text)
    {
        TextMeshPro.text = text;
    }
}

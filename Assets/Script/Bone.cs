using System.Collections.Generic;
using UnityEngine;
using VRM;
using TMPro;

public class Bone : MonoBehaviour
{
    public static List<Bone> List = new List<Bone>();
    public static Transform Camera;

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

    [SerializeField] Material BoneC;
    [SerializeField] Material BoneM;
    [SerializeField] Material BoneY;
    [SerializeField] Material BoneR;

    [SerializeField] TextMeshPro TextMeshPro;

    void Update()
    {
        if (Target.parent == null) return;

        transform.position = Target.position;

        var position = (transform.position + Target.parent.position) / 2;
        Line.transform.position = position;

        var scale = Line.transform.localScale;
        scale.z = Vector3.Distance(transform.position, Target.parent.position) * (1 / transform.localScale.z);
        Line.transform.localScale = scale;

        Line.transform.LookAt(Target.parent);

        if (TextMeshPro.text != "")
        {
            TextMeshPro.gameObject.transform.LookAt(Camera);
        }
    }

    public void Select()
    {
        foreach (var bone in List)
        {
            bone.Setting(bone.State);
            bone.Collider.SetActive(false);
        }
        Sphere.GetComponent<MeshRenderer>().material = BoneR;

        var collider = Target.GetComponent<VRMSpringBoneColliderGroup>();
        if (collider != null)
        {
            Collider.transform.localScale = Vector3.one * collider.Colliders[0].Radius;
            Collider.transform.localPosition = collider.Colliders[0].Offset;
            Collider.SetActive(true);
        }
    }

    public void Setting(StateType state)
    {
        State = state;
        switch (state)
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

    public void SetTest(string text)
    {
        TextMeshPro.text = text;
    }
}

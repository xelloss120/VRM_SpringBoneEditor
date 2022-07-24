using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] Import Import;
    [SerializeField] float Range = 0.1f;
    [SerializeField] float Speed = 0.005f;

    enum EnumType
    {
        UpDown,
        LeftRight,
        FrontBack,
        Stop
    }
    EnumType Type;

    Vector3 Target = Vector3.zero;

    void Update()
    {
        if (Import.Root == null) return;

        var pos = Import.Root.transform.position;
        switch(Type)
        {
            case EnumType.UpDown:
                if (pos == Vector3.up * Range) Target = Vector3.down * Range;
                if (pos == Vector3.down * Range) Target = Vector3.up * Range;
                break;
            case EnumType.LeftRight:
                if (pos == Vector3.left * Range) Target = Vector3.right * Range;
                if (pos == Vector3.right * Range) Target = Vector3.left * Range;
                break;
            case EnumType.FrontBack:
                if (pos == Vector3.forward * Range) Target = Vector3.back * Range;
                if (pos == Vector3.back * Range) Target = Vector3.forward * Range;
                break;
        }
        Import.Root.transform.position = Vector3.MoveTowards(pos, Target, Speed);
    }

    void SetBoneActive(bool active)
    {
        foreach (var bone in Bone.List)
        {
            bone.gameObject.SetActive(active);
        }
    }

    public void UpDown()
    {
        Type = EnumType.UpDown;
        Target = Vector3.up * Range;
        SetBoneActive(false);
    }

    public void LeftRight()
    {
        Type = EnumType.LeftRight;
        Target = Vector3.left * Range;
        SetBoneActive(false);
    }

    public void FrontBack()
    {
        Type = EnumType.FrontBack;
        Target = Vector3.forward * Range;
        SetBoneActive(false);
    }

    public void Stop()
    {
        Type = EnumType.Stop;
        Target = Vector3.zero;
        SetBoneActive(true);
    }
}

using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] Import Import;
    [SerializeField] float Range = 0.1f;
    [SerializeField] float Speed = 0.5f;

    enum CtrlType
    {
        UpDown,
        LeftRight,
        ForwardBack,
        Stop
    }
    CtrlType Type;

    Vector3 Target = Vector3.zero;

    void Update()
    {
        if (Import.Root == null) return;

        var pos = Import.Root.transform.position;
        switch(Type)
        {
            case CtrlType.UpDown:
                if (pos == Vector3.up * Range) Target = Vector3.down * Range;
                if (pos == Vector3.down * Range) Target = Vector3.up * Range;
                break;
            case CtrlType.LeftRight:
                if (pos == Vector3.left * Range) Target = Vector3.right * Range;
                if (pos == Vector3.right * Range) Target = Vector3.left * Range;
                break;
            case CtrlType.ForwardBack:
                if (pos == Vector3.forward * Range) Target = Vector3.back * Range;
                if (pos == Vector3.back * Range) Target = Vector3.forward * Range;
                break;
        }

        float step = Speed * Time.deltaTime;
        Import.Root.transform.position = Vector3.MoveTowards(pos, Target, step);
    }

    void SetBoneActive(bool active)
    {
        foreach (var bone in Bone.List)
        {
            bone.gameObject.SetActive(active);
        }
    }

    public void OnClickUpDown()
    {
        Type = CtrlType.UpDown;
        Target = Vector3.up * Range;
        SetBoneActive(false);
    }

    public void OnClickLeftRight()
    {
        Type = CtrlType.LeftRight;
        Target = Vector3.left * Range;
        SetBoneActive(false);
    }

    public void OnClickForwardBack()
    {
        Type = CtrlType.ForwardBack;
        Target = Vector3.forward * Range;
        SetBoneActive(false);
    }

    public void OnClickStop()
    {
        Type = CtrlType.Stop;
        Target = Vector3.zero;
        SetBoneActive(true);
    }
}

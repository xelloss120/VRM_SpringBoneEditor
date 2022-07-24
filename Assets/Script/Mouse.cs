using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Transform ViewPoint;
    [SerializeField] Camera Camera;

    [SerializeField] float Angle_Speed = 1.0f;
    [SerializeField] float PosXY_Speed = 0.005f;
    [SerializeField] float Size_Speed = 0.5f;

    Vector3 MousePos;

    /// <summary>
    /// �J�����̃}�E�X����
    /// </summary>
    void Update()
    {
        var pos = Input.mousePosition;
        if (pos.x < 0 || pos.x > Screen.width ||
            pos.y < 0 || pos.y > Screen.height)
        {
            // ��ʊO����̖���
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // ����N���b�N���̉�]��ړ��𖳌���
            MousePos = pos;
        }

        // �}�E�X�̈ړ��ʂ��擾
        var diff = MousePos - pos;
        MousePos = pos;
        
        if (Input.GetMouseButton(0))
        {
            // ���N���b�N�i��]�j
            var tmp = diff;
            diff.x = +tmp.y;
            diff.y = -tmp.x;
            ViewPoint.eulerAngles += diff * Angle_Speed;
        }
        else if (Input.GetMouseButton(1))
        {
            // �E�N���b�N�i�ړ��j
            var tmp = diff;
            diff.x = +tmp.x;
            diff.y = +tmp.y;
            ViewPoint.position += ViewPoint.up * diff.y * PosXY_Speed * Camera.orthographicSize;
            ViewPoint.position += ViewPoint.right * diff.x * PosXY_Speed * Camera.orthographicSize;
        }

        // �X�N���[���i�g�k�Ƃ������O��Ƃ�������p�j
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Camera.orthographicSize < 0.01 && scroll > 0)
        {
            // �߂Â������h�~
            scroll = 0;
        }
        Camera.orthographicSize -= scroll * Size_Speed;
    }
}

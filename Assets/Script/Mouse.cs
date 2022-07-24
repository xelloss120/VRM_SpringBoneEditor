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
    /// カメラのマウス操作
    /// </summary>
    void Update()
    {
        var pos = Input.mousePosition;
        if (pos.x < 0 || pos.x > Screen.width ||
            pos.y < 0 || pos.y > Screen.height)
        {
            // 画面外操作の無視
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // 初回クリック時の回転や移動を無効化
            MousePos = pos;
        }

        // マウスの移動量を取得
        var diff = MousePos - pos;
        MousePos = pos;
        
        if (Input.GetMouseButton(0))
        {
            // 左クリック（回転）
            var tmp = diff;
            diff.x = +tmp.y;
            diff.y = -tmp.x;
            ViewPoint.eulerAngles += diff * Angle_Speed;
        }
        else if (Input.GetMouseButton(1))
        {
            // 右クリック（移動）
            var tmp = diff;
            diff.x = +tmp.x;
            diff.y = +tmp.y;
            ViewPoint.position += ViewPoint.up * diff.y * PosXY_Speed * Camera.orthographicSize;
            ViewPoint.position += ViewPoint.right * diff.x * PosXY_Speed * Camera.orthographicSize;
        }

        // スクロール（拡縮というか前後というか画角）
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Camera.orthographicSize < 0.01 && scroll > 0)
        {
            // 近づきすぎ防止
            scroll = 0;
        }
        Camera.orthographicSize -= scroll * Size_Speed;
    }
}

using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Mouse : MonoBehaviour
{
    public Bone Select = null;

    [SerializeField] Import Import;
    [SerializeField] Editor Editor;

    [SerializeField] Text BoneName;
    [SerializeField] Dropdown Dropdown;

    [SerializeField] InputField Size;
    [SerializeField] InputField OffsetX;
    [SerializeField] InputField OffsetY;
    [SerializeField] InputField OffsetZ;

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
        if (pos.x < 160 && pos.y > Screen.height - 240)
        {
            // UI�㑀��̖���
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

        SelectBone();
    }

    void SelectBone()
    {
        // �{�[���I��
        // �Q�l
        // https://tech.pjin.jp/blog/2018/09/03/unity_get-clicked-gameobject/
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                var bone = hit.collider.gameObject.transform.parent.GetComponent<Bone>();
                if (bone != null)
                {
                    Editor.MouseSelect = true;

                    Select = bone;
                    bone.Select();

                    BoneName.text = bone.Target.name;

                    Size.text = "";
                    OffsetX.text = "";
                    OffsetY.text = "";
                    OffsetZ.text = "";

                    Size.interactable = false;
                    OffsetX.interactable = false;
                    OffsetY.interactable = false;
                    OffsetZ.interactable = false;

                    var value = 0;
                    var springs = Import.Root.GetComponentsInChildren<VRMSpringBone>();
                    foreach (var spring in springs)
                    {
                        if (spring.RootBones != null)
                        {
                            foreach (var b in spring.RootBones)
                            {
                                if (bone.Target == b)
                                {
                                    for (int i = 0; i < Dropdown.options.Count; i++)
                                    {
                                        if (spring.m_comment == Dropdown.options[i].text)
                                        {
                                            value = i;
                                        }
                                    }
                                }
                            }
                        }
                        if (spring.ColliderGroups != null)
                        {
                            foreach (var b in spring.ColliderGroups)
                            {
                                if (bone.Target == b.gameObject.transform)
                                {
                                    value = Dropdown.options.Count - 1;

                                    Size.interactable = true;
                                    OffsetX.interactable = true;
                                    OffsetY.interactable = true;
                                    OffsetZ.interactable = true;

                                    Size.text = b.Colliders[0].Radius.ToString();
                                    OffsetX.text = b.Colliders[0].Offset.x.ToString();
                                    OffsetY.text = b.Colliders[0].Offset.y.ToString();
                                    OffsetZ.text = b.Colliders[0].Offset.z.ToString();
                                }
                            }
                        }
                    }

                    Dropdown.value = value;

                    Editor.MouseSelect = false;
                }
            }
        }
    }
}

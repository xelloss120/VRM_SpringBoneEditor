using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Select : MonoBehaviour
{
    public bool Changing;

    [SerializeField] Import Import;

    [SerializeField] Text BoneName;
    [SerializeField] Dropdown Dropdown;

    [SerializeField] InputField Size;
    [SerializeField] InputField OffsetX;
    [SerializeField] InputField OffsetY;
    [SerializeField] InputField OffsetZ;

    void Update()
    {
        var pos = Input.mousePosition;
        if (pos.x < 160 && pos.y > Screen.height - 240)
        {
            // UI上操作の無視
            return;
        }

        // 参考
        // https://tech.pjin.jp/blog/2018/09/03/unity_get-clicked-gameobject/
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                var bone = hit.collider.transform.parent.GetComponent<Bone>();
                if (bone != null)
                {
                    // 操作用のボーンUIの選択
                    Changing = true;

                    bone.Select();

                    BoneName.text = bone.Target.name;

                    Dropdown.value = 0;

                    Size.text = "";
                    OffsetX.text = "";
                    OffsetY.text = "";
                    OffsetZ.text = "";

                    Size.interactable = false;
                    OffsetX.interactable = false;
                    OffsetY.interactable = false;
                    OffsetZ.interactable = false;

                    var springs = Import.Root.GetComponentsInChildren<VRMSpringBone>();
                    foreach (var spring in springs)
                    {
                        if (spring.RootBones != null)
                        {
                            // 揺れ物
                            foreach (var item in spring.RootBones)
                            {
                                if (bone.Target == item)
                                {
                                    for (int i = 0; i < Dropdown.options.Count; i++)
                                    {
                                        if (spring.m_comment == Dropdown.options[i].text)
                                        {
                                            // 操作用ボーンUIの対象が揺れ物ならDropdownから対応する揺れ物を選択
                                            Dropdown.value = i;
                                        }
                                    }
                                }
                            }
                        }
                        if (spring.ColliderGroups != null)
                        {
                            // 当たり判定
                            foreach (var item in spring.ColliderGroups)
                            {
                                if (bone.Target == item.transform)
                                {
                                    // 操作用ボーンUIの対象が当たり判定ならDropdownから当たり判定を選択
                                    Dropdown.value = Dropdown.options.Count - 1;

                                    Size.interactable = true;
                                    OffsetX.interactable = true;
                                    OffsetY.interactable = true;
                                    OffsetZ.interactable = true;

                                    Size.text = item.Colliders[0].Radius.ToString();
                                    OffsetX.text = item.Colliders[0].Offset.x.ToString();
                                    OffsetY.text = item.Colliders[0].Offset.y.ToString();
                                    OffsetZ.text = item.Colliders[0].Offset.z.ToString();
                                }
                            }
                        }
                    }

                    Changing = false;
                }
            }
        }
    }
}

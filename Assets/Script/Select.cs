using UnityEngine;
using UnityEngine.UI;
using VRM;

public class Select : MonoBehaviour
{
    public Bone Bone;

    [SerializeField] Import Import;
    [SerializeField] Editor Editor;

    [SerializeField] Text BoneName;
    [SerializeField] Dropdown Dropdown;

    [SerializeField] InputField Size;
    [SerializeField] InputField OffsetX;
    [SerializeField] InputField OffsetY;
    [SerializeField] InputField OffsetZ;

    /// <summary>
    /// É{Å[ÉìëIë
    /// </summary>
    /// <remarks>
    /// https://tech.pjin.jp/blog/2018/09/03/unity_get-clicked-gameobject/
    /// </remarks>
    void Update()
    {
        var pos = Input.mousePosition;
        if (pos.x < 160 && pos.y > Screen.height - 240)
        {
            // UIè„ëÄçÏÇÃñ≥éã
            return;
        }

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

                    Bone = bone;
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

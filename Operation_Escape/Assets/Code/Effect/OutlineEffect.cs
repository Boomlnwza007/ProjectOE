using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Transform mainObject;
    public float outlineSize = 0.1f;

    void LateUpdate()
    {
        if (mainObject != null)
        {
            // ปรับตำแหน่งให้ซ้อนทับกัน
            transform.position = mainObject.position;

            // ปรับขนาดให้ขอบไม่เปลี่ยนขนาด
            transform.localScale = Vector3.one * (1 + outlineSize);
        }
    }
}

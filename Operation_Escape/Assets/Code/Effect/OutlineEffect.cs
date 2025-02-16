using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Transform mainObject;
    public float outlineSize = 0.1f;

    void LateUpdate()
    {
        if (mainObject != null)
        {
            // ��Ѻ���˹�����͹�Ѻ�ѹ
            transform.position = mainObject.position;

            // ��Ѻ��Ҵ���ͺ�������¹��Ҵ
            transform.localScale = Vector3.one * (1 + outlineSize);
        }
    }
}

using UnityEngine;

public class ParticleAlongLineWithShape : MonoBehaviour
{
    public LineRenderer lineRenderer;     // LineRenderer ที่ใช้สร้างเส้น
    public ParticleSystem particle; // ParticleSystem ต้นแบบ
    private ParticleSystem.ShapeModule shapeModule;  // โมดูล Shape ของ ParticleSystem
    private float baseSize;
    private float baseRate;

    void Start()
    {
        baseSize = particle.shape.scale.x;
        baseRate = particle.emission.rateOverTime.constant;
        shapeModule = particle.shape;
    }

    void Update()
    {
        Size();
        SizeUp();
    }

    public void Size()
    {
        // ตรวจสอบว่า LineRenderer มีจุดเริ่มต้นและจุดปลาย
        if (lineRenderer.positionCount < 2) return;
        if (lineRenderer.enabled && !particle.isPlaying)
        {
            particle.Play();
        }
        else if (!lineRenderer.enabled && particle.isPlaying)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        // ดึงจุดเริ่มต้นและจุดปลายจาก LineRenderer
        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        // คำนวณระยะห่างระหว่างจุดเริ่มต้นและจุดปลาย
        float lineLength = Vector3.Distance(startPoint, endPoint);

        // ตั้งค่าระยะทางของ Shape ให้ตรงกับความยาวของเส้น
        shapeModule.scale = new Vector3(lineLength, lineRenderer.startWidth, 1.0f);
        Vector2 direction = endPoint - startPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        particle.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        // ตั้งค่าตำแหน่งของการปล่อย Particle ให้ตรงกับจุดเริ่มต้นของ LineRenderer
        particle.gameObject.transform.position = startPoint;
    }

    public void SizeUp()
    {
        // คำนวณพื้นที่ของสี่เหลี่ยม
        float area = shapeModule.scale.x * shapeModule.scale.z;

        // คำนวณจำนวนพาร์ติเคิลทั้งหมด
        float totalParticles = baseRate * area;

        // คำนวณ rateOverTime ที่จะใช้
        float rateOverTime = totalParticles;

        // ตั้งค่า rateOverTime ให้กับ ParticleSystem
        var emission = particle.emission;
        emission.rateOverTime = rateOverTime;

    }
}

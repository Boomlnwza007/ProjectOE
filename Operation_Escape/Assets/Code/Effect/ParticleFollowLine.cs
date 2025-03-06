using UnityEngine;

public class ParticleAlongLineWithShape : MonoBehaviour
{
    public LineRenderer lineRenderer;     // LineRenderer ที่ใช้สร้างเส้น
    public ParticleSystem particle; // ParticleSystem ต้นแบบ
    private ParticleSystem.ShapeModule shapeModule;  // โมดูล Shape ของ ParticleSystem
    private float baseSize;
    private float baseRate;
    private bool particleActice = true;

    void Start()
    {
        baseSize = particle.shape.scale.x;
        baseRate = particle.emission.rateOverTime.constant;
        shapeModule = particle.shape;
        particleActice = true;
    }

    void Update()
    {
        Size();
        SizeUp();
    }

    public void Size()
    {
        if (lineRenderer.enabled && !particle.isPlaying && particleActice)
        {
            particle.Play();
        }
        else if (!lineRenderer.enabled && particle.isPlaying)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        float lineLength = Vector3.Distance(startPoint, endPoint);

        shapeModule.scale = new Vector3(lineLength, lineRenderer.startWidth, 1.0f);
        //Vector2 direction = endPoint - startPoint;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //particle.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        particle.gameObject.transform.position = startPoint;
    }

    public void ActiceParticle(bool on)
    {
        particleActice = on;
        if (on)
        {
            particle.Play();

        }
        else
        {
            particle.Stop();
        }
    }

    public void SizeUp()
    {
        float area = shapeModule.scale.x * shapeModule.scale.z;

        float totalParticles = baseRate * area;

        float rateOverTime = totalParticles;

        var emission = particle.emission;
        emission.rateOverTime = rateOverTime;
    }
}

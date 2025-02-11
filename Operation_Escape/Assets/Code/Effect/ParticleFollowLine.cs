using UnityEngine;

public class ParticleAlongLineWithShape : MonoBehaviour
{
    public LineRenderer lineRenderer;     // LineRenderer ��������ҧ���
    public ParticleSystem particle; // ParticleSystem ��Ẻ
    private ParticleSystem.ShapeModule shapeModule;  // ����� Shape �ͧ ParticleSystem
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
        // ��Ǩ�ͺ��� LineRenderer �ըش���������Шش����
        if (lineRenderer.positionCount < 2) return;
        if (lineRenderer.enabled && !particle.isPlaying)
        {
            particle.Play();
        }
        else if (!lineRenderer.enabled && particle.isPlaying)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        // �֧�ش���������Шش���¨ҡ LineRenderer
        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        // �ӹǳ������ҧ�����ҧ�ش���������Шش����
        float lineLength = Vector3.Distance(startPoint, endPoint);

        // ��駤�����зҧ�ͧ Shape ���ç�Ѻ������Ǣͧ���
        shapeModule.scale = new Vector3(lineLength, lineRenderer.startWidth, 1.0f);
        Vector2 direction = endPoint - startPoint;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        particle.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        // ��駤�ҵ��˹觢ͧ��û���� Particle ���ç�Ѻ�ش������鹢ͧ LineRenderer
        particle.gameObject.transform.position = startPoint;
    }

    public void SizeUp()
    {
        // �ӹǳ��鹷��ͧ����������
        float area = shapeModule.scale.x * shapeModule.scale.z;

        // �ӹǳ�ӹǹ�������ŷ�����
        float totalParticles = baseRate * area;

        // �ӹǳ rateOverTime ������
        float rateOverTime = totalParticles;

        // ��駤�� rateOverTime ���Ѻ ParticleSystem
        var emission = particle.emission;
        emission.rateOverTime = rateOverTime;

    }
}

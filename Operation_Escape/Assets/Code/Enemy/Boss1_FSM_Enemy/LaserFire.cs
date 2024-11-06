using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class LaserFire : MonoBehaviour
{
    public int dmg;
    public LineRenderer m_lineRenderer;
    public LineRenderer pre_lineRenderer;
    public Gradient laserColorGradientOriginal;
    public Gradient laserColorGradient;
    public Transform laserFireStart;
    public Transform targetPlayer;
    public Vector3 target;
    public bool laserHitZone;
    public bool laserFiring;
    public float speedRot = 10f;
    public float accelerationTime = 0.2f;
    public float laserDistance = 100;
    public float dpsDamage = 1;
    private bool canDamage = true;
    public LayerMask obstacleLayer;
    public LayerMask ShootLayer;
    public bool follow;
    public float overshootAngle = 0;
    public int followCode;
    public GameObject startSFX;
    public GameObject endSFX;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    private void Start()
    {
        FillLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (laserHitZone)
        {
            LaserHitZone();
        }

        if (laserFiring)
        {
            LaserFiring();
        }

        if (follow)
        {
            switch (followCode)
            {
                case 0:
                    if (targetPlayer!=null)
                    {
                        LaserFollowIN(targetPlayer);
                    }
                    else
                    {
                        Debug.Log(gameObject.name);
                    }
                    break;
                case 1:
                    LaserFollowStF(target);
                    break;
                case 2:
                    LaserFollow(targetPlayer);
                    break;
                default:
                    break;
            }
        }
    }

    public async UniTask ShootLaser(float charge, float duration, float speedMulti)
    {
        float speedRotOri = speedRot;
        speedRot = speedRot * speedMulti;
        pre_lineRenderer.colorGradient = laserColorGradient;
        pre_lineRenderer.enabled = true;
        laserHitZone = true;

        await FadeLaser(pre_lineRenderer,charge, laserColorGradient, false);
        pre_lineRenderer.enabled = false;

        laserHitZone = false;

        m_lineRenderer.colorGradient = laserColorGradientOriginal;
        m_lineRenderer.enabled = true;
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Play();
        }
        laserFiring = true;

        await UniTask.WaitForSeconds(duration);

        await FadeLaser(m_lineRenderer,0.1f, laserColorGradientOriginal, true);
        laserHitZone = false;
        m_lineRenderer.enabled = false;
        laserFiring = false;
        speedRot = speedRotOri;
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Stop();
        }
    }
    
    public void DrawRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
        startSFX.transform.position = startPos;
        endSFX.transform.position = endPos;
        Vector3 aimDir = (endPos - startPos).normalized;
        float _angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        startSFX.transform.eulerAngles = new Vector3(0, 0, _angle);
        endSFX.transform.eulerAngles = new Vector3(0, 0, _angle);
    }

    public void DrawRayHitZone(Vector2 startPos, Vector2 endPos)
    {
        pre_lineRenderer.SetPosition(0, startPos);
        pre_lineRenderer.SetPosition(1, endPos);
    }

    private async UniTask FadeLaser(LineRenderer line,float charge, Gradient colorGradient, bool fadeOut)
    {
        float elapsedTime = 0f;
        int fadeStart = 0;
        int fadeEnd = 1;
        if (fadeOut)
        {
            fadeStart = 1;
            fadeEnd = 0;
        }
        GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;

        while (elapsedTime < charge)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(fadeStart, fadeEnd, elapsedTime / charge);

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = alphaValue;
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(colorGradient.colorKeys, alphaKeys);
            line.colorGradient = gradient;

            await UniTask.Yield();
        }
    }
    
    public void LaserHitZone()
    {
        if (Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right, laserDistance, obstacleLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right, laserDistance, obstacleLayer);
            DrawRayHitZone(laserFireStart.position, _hit.point);
        }
        else
        {
            DrawRayHitZone(laserFireStart.position, laserFireStart.position + laserFireStart.transform.right * laserDistance);
        }
    }
    
    public void LaserFiring()
    {
        if (Physics2D.BoxCast(laserFireStart.position, new Vector2(m_lineRenderer.startWidth-1.5f, m_lineRenderer.startWidth-1.5f), 0f, transform.right, laserDistance, ShootLayer))
        {
            RaycastHit2D hitInfo = Physics2D.BoxCast(laserFireStart.position, new Vector2(m_lineRenderer.startWidth-1.5f, m_lineRenderer.startWidth-1.5f), 0f, transform.right, laserDistance, ShootLayer);
            if (hitInfo.collider.CompareTag("Player"))
            {
                IDamageable player = hitInfo.collider.GetComponent<IDamageable>();
                if (player != null)
                {
                    ////////Edit
                    if (canDamage)
                    {
                        canDamage = false;
                        player.Takedamage(dmg, DamageType.Rang, 0);
                        DamageHit().Forget();
                    }
                    //Debug.Log("hit");
                }
            }
        }
       

        if (Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right,laserDistance, ShootLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, laserFireStart.transform.right, laserDistance, ShootLayer);
            DrawRay(laserFireStart.position, _hit.point);
        }
        else
        {
            DrawRay(laserFireStart.position, laserFireStart.position + laserFireStart.transform.right * laserDistance);
        }
    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
        await UniTask.WaitForSeconds(dpsDamage);
    }

    public void LaserFollow()
    {
        Vector2 dir = (target - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);

        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public void LaserFollow(Vector3 target)
    {
        Vector2 dir = (target).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);

        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public void LaserFollow(Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speedRot);

        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public void LaserFollowStF(Vector3 target)
    {
        Vector2 dir = ((transform.position+target) - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        targetAngle += overshootAngle;
        float t = Mathf.Clamp01(Time.deltaTime * speedRot);
        t = Mathf.SmoothStep(0, accelerationTime, t);

        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, t);

        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    public void LaserFollowIN(Transform target)
    {
        Vector2 dir = target.right;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        targetAngle += overshootAngle;
        float t = Mathf.Clamp01(Time.deltaTime * speedRot);
        t = Mathf.SmoothStep(0, accelerationTime, t);

        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, t); 

        transform.eulerAngles = new Vector3(0, 0, newAngle);
    }
    public void SetStartFollow(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, targetAngle);
    }

    public void SetStartFollow(float target)
    {
        transform.eulerAngles = new Vector3(0, 0, target);       
    }

    public void SetovershootAngle(float _overshootAngle,Transform target)
    {
        Vector2 dir = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        Debug.Log(angleDifference);

        if (angleDifference < 0)
        {
            _overshootAngle *= -1;
        }

        overshootAngle = _overshootAngle;
    }

    public async UniTask Aim(float wait)
    {
        follow = true;
        await UniTask.WaitForSeconds(wait);
        follow = false;
    }

    public void FillLaser()
    {
        for (int i = 0; i < startSFX.transform.childCount; i++)
        {
            var ps = startSFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particleSystems.Add(ps);
            }
        }

        for (int i = 0; i < endSFX.transform.childCount; i++)
        {
            var ps = endSFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particleSystems.Add(ps);
            }
        }


        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Stop();
        }
    }
}

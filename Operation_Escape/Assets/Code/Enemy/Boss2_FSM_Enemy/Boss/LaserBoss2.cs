using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBoss2 : MonoBehaviour
{
    public int dmg;
    public float dpsDamage = 1;
    public LineRenderer pre_lineRenderer;
    public LineRenderer m_lineRenderer;
    public Transform laserFireStart;
    public bool isUp;
    public LayerMask obstacleLayer;
    public LayerMask ShootLayer;
    public GameObject startSFX;
    public GameObject endSFX;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    [SerializeField]private GameObject[] spritLaser;
    public float timer = 1;
    private float time;
    public float duration = 0.5f;
    private bool isFiring;
    public float laserDistance = 100;
    private bool canDamage = true;

    private void Start()
    {
        FillLaser();
    }

    private void Update()
    {
        if (!isFiring && time >= timer)
        {
            isFiring = true;
            m_lineRenderer.enabled = true;
            startSFX.SetActive(true);
            endSFX.SetActive(true);
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
            foreach (var item in spritLaser)
            {
                item.SetActive(true);
            }
            pre_lineRenderer.enabled = false;
            time = 0f;
        }

        if (isFiring)
        {
            time += Time.deltaTime;
            LaserFire();
            if (time >= duration)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            ShootPreLaser();
            time += Time.deltaTime;
        }
    }

    public void LaserFire()
    {
        Vector2 dir = isUp ? -transform.up : transform.right;
        if (Physics2D.BoxCast(laserFireStart.position, new Vector2(m_lineRenderer.startWidth, m_lineRenderer.startWidth), 0f, dir, laserDistance, ShootLayer))
        {
            RaycastHit2D hitInfo = Physics2D.BoxCast(laserFireStart.position, new Vector2(m_lineRenderer.startWidth, m_lineRenderer.startWidth), 0f, dir, laserDistance, ShootLayer);
            if (hitInfo.collider.CompareTag("Player"))
            {
                IDamageable player = hitInfo.collider.GetComponent<IDamageable>();
                if (player != null)
                {
                    if (canDamage)
                    {
                        canDamage = false;
                        player.Takedamage(dmg, DamageType.Rang, 0);
                        DamageHit().Forget();
                        Debug.Log("hit");
                    }
                }
            }
        }


        if (Physics2D.Raycast(laserFireStart.position, dir, laserDistance, ShootLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFireStart.position, dir, laserDistance, ShootLayer);
            DrawRay(laserFireStart.position, _hit.point);
        }
        else
        {
            DrawRay(laserFireStart.position, (Vector2)laserFireStart.position + (dir * laserDistance));
        }
    }

    public void ShootPreLaser()
    {
        Vector2 dir = isUp ? -transform.up : transform.right;
        DrawPreRay(laserFireStart.position, (Vector2)laserFireStart.position + (dir * laserDistance));
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

    public void DrawPreRay(Vector2 startPos, Vector2 endPos)
    {
        pre_lineRenderer.SetPosition(0, startPos);
        pre_lineRenderer.SetPosition(1, endPos);
    }


    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
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
    }
}

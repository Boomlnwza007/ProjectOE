using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrone : MonoBehaviour
{
    public Transform firstPos;
    public Transform endPos;
    private Vector2 direction;
    private float distance;
    private float time;
    private bool isFiring = false;
    public float timeBetween;
    public float duration;
    public int damage = 60;
    public float dpsDamage = 1;
    private bool canDamage = true;
    public GameObject startSFX;
    public GameObject endSFX;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private LineRenderer laser;
    public LayerMask ShootLayer;

    // Start is called before the first frame update
    void Start()
    {
        FillLaser();
        time = 0;
        laser = GetComponent<LineRenderer>();
        distance = Vector2.Distance(firstPos.position, endPos.position);
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFiring && time >= timeBetween)
        {
            isFiring = true;
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
            time = 0f;
        }

        if (isFiring)
        {
            time += Time.deltaTime;
            LaserFire();
            if (time >= duration)
            {
                isFiring = false;
                laser.enabled = false;
                for (int i = 0; i < particleSystems.Count; i++)
                {
                    particleSystems[i].Stop();
                }
                time = 0f;
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    public void LaserFire()
    {
        laser.enabled = true;
        direction = (endPos.position - firstPos.position).normalized;
        distance = Vector2.Distance(firstPos.position, endPos.position);

        if (Physics2D.BoxCast(firstPos.position, new Vector2(laser.startWidth, laser.startWidth), 0f, direction, distance, ShootLayer))
        {
            RaycastHit2D hitInfo = Physics2D.BoxCast(firstPos.position, new Vector2(laser.startWidth, laser.startWidth), 0f, direction, distance, ShootLayer);
            if (hitInfo.collider.CompareTag("Player"))
            {
                IDamageable player = hitInfo.collider.GetComponent<IDamageable>();
                if (player != null)
                {
                    if (canDamage)
                    {
                        canDamage = false;
                        player.Takedamage(damage, DamageType.Rang, 0);
                        DamageHit().Forget();
                    }
                }
            }
        }

        if (Physics2D.Raycast(firstPos.position, direction, distance, ShootLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firstPos.position, direction, distance, ShootLayer);
            DrawRay(firstPos.position, _hit.point);
        }
        else
        {
            DrawRay(firstPos.position, endPos.position);
        }
    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
        await UniTask.WaitForSeconds(dpsDamage);
    }

    public void DrawRay(Vector2 startPos, Vector2 endPos)
    {
        laser.SetPosition(0, startPos);
        laser.SetPosition(1, endPos);
        startSFX.transform.position = startPos;
        endSFX.transform.position = endPos;
        Vector3 aimDir = (endPos - startPos).normalized;
        float _angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        startSFX.transform.eulerAngles = new Vector3(0, 0, _angle);
        endSFX.transform.eulerAngles = new Vector3(0, 0, _angle);
    }

    public void Destroy()
    {
        Destroy(laser);
        Destroy(GetComponent<LaserDrone>());
        Destroy(firstPos.gameObject);
        Destroy(endPos.gameObject);
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

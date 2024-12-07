using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class LaserCharge : MonoBehaviour
{
    public LineRenderer laserUltiPrefab;
    public int dmgUl;
    public int maxDmgUl;
    private int startDmg;
    public float laserDistance = 100;
    public float laserSizeOffset = 1;
    public LayerMask ShootLayer;
    private bool canDamage = true;
    public float dpsDamage = 1;
    public Transform bulletTranform;
    public GameObject startSFX;
    public GameObject endSFX;
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    private float offset = 0;
    float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        FillLaser();
        time = 0;
        laserUltiPrefab.enabled = true;
        laserUltiPrefab.startWidth = 1f;
        laserUltiPrefab.endWidth = 1f;
        startDmg = dmgUl;
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = bulletTranform.transform.right * offset;
        ShootLaser();
    }

    public void ShootLaser()
    {
        //for (int i = 0; i < particleSystems.Count; i++)
        //{
        //    particleSystems[i].Play();
        //}

        time += Time.deltaTime;
        if (time > 1)
        {          
            laserUltiPrefab.startWidth = 2f + laserSizeOffset;
            laserUltiPrefab.endWidth = 2f + laserSizeOffset;
            startSFX.transform.localScale = new Vector3(6, 6, 0);
            endSFX.transform.localScale = new Vector3(6, 6, 0);
            offset = 2;
            dmgUl = maxDmgUl;
        }
        else
        {
            float t = time / 1;
            laserUltiPrefab.startWidth = Mathf.Lerp(1f+ laserSizeOffset, 2f+ laserSizeOffset, t);
            laserUltiPrefab.endWidth = Mathf.Lerp(1f+ laserSizeOffset, 2f+ laserSizeOffset, t);
            startSFX.transform.localScale = Vector3.Lerp(new Vector3(3, 3, 0),new Vector3(6, 6, 0), t);
            endSFX.transform.localScale = Vector3.Lerp(new Vector3(3, 3, 0), new Vector3(6, 6, 0), t);
            offset = Mathf.Lerp(0f, 1f, t);
            dmgUl = (int)Mathf.Lerp(startDmg, maxDmgUl, t);
        }

        if (Physics2D.Raycast(bulletTranform.transform.position, bulletTranform.transform.right, laserDistance, ShootLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(bulletTranform.transform.position, bulletTranform.transform.right, laserDistance, ShootLayer);
            DrawRay(bulletTranform.transform.position, _hit.point);
        }
        else
        {
            DrawRay(bulletTranform.transform.position, bulletTranform.transform.position + bulletTranform.transform.right * laserDistance);
        }

        RaycastHit2D hitInfo = Physics2D.BoxCast(bulletTranform.transform.position, new Vector2(laserUltiPrefab.endWidth, laserUltiPrefab.endWidth), 0f, bulletTranform.transform.right, laserDistance, ShootLayer);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.name);
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                IDamageable Enemy = hitInfo.collider.GetComponent<IDamageable>();
                GuardShield guard = hitInfo.collider.GetComponent<GuardShield>();
                if (Enemy != null && canDamage)
                {
                    canDamage = false;
                    Enemy.Takedamage(dmgUl, DamageType.Rang, 0);
                    DamageHit().Forget();
                }
                guard?.BreakShield();
            }
        }

        //hitInfo = default;
    }


    public void DrawRay(Vector2 startPos, Vector2 endPos)
    {
        laserUltiPrefab.SetPosition(0, startPos);
        laserUltiPrefab.SetPosition(1, endPos);
        startSFX.transform.position = startPos;
        endSFX.transform.position = endPos;
        startSFX.transform.rotation = bulletTranform.rotation;
        endSFX.transform.rotation = bulletTranform.rotation;

    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
        await UniTask.WaitForSeconds(dpsDamage);
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

    public void LaserDis()
    {
        for (int i = 0; i < particleSystems.Count; i++)
        {
            particleSystems[i].Stop();
        }
    }
}

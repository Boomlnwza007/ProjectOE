using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class LaserCharge : MonoBehaviour
{
    public LineRenderer laserUltiPrefab;
    public GameObject startLaser;
    public GameObject endLaser;
    public int dmgUl;
    public int maxDmgUl;
    private int startDmg;
    public float laserDistance = 100;
    public LayerMask ShootLayer;
    private bool canDamage = true;
    public float dpsDamage = 1;
    public Transform bulletTranform;
    private float offset = 0;
    float time = 0;


    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        laserUltiPrefab.enabled = true;
        startLaser.SetActive(true);
        laserUltiPrefab.startWidth = 1f;
        laserUltiPrefab.endWidth = 1f;
        startDmg = dmgUl;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = bulletTranform.transform.right * offset;
        gameObject.transform.position = bulletTranform.position + dir;
        ShootLaser();
    }

    public void ShootLaser()
    {
        time += Time.deltaTime;
        if (time > 1)
        {          
            laserUltiPrefab.startWidth = 2f;
            laserUltiPrefab.endWidth = 2f;
            startLaser.transform.localScale = new Vector3(9, 9, 0);
            offset = 2;
            dmgUl = maxDmgUl;
        }
        else
        {
            float t = time / 1;
            laserUltiPrefab.startWidth = Mathf.Lerp(1f, 2f, t);
            laserUltiPrefab.endWidth = Mathf.Lerp(1f, 2f, t);
            laserUltiPrefab.endWidth = Mathf.Lerp(1f, 2f, t);
            startLaser.transform.localScale = Vector3.Lerp(new Vector3(5, 5, 0),new Vector3(9, 9, 0), t);
            offset = Mathf.Lerp(0f, 2f, t);
            dmgUl = (int)Mathf.Lerp(startDmg, maxDmgUl, t);
        }

        if (Physics2D.BoxCast(startLaser.transform.position, new Vector2(laserUltiPrefab.endWidth, laserUltiPrefab.endWidth), 0f, transform.right, laserDistance, ShootLayer))
        {
            RaycastHit2D hitInfo = Physics2D.BoxCast(startLaser.transform.position, new Vector2(laserUltiPrefab.endWidth, laserUltiPrefab.endWidth), 0f, transform.right, laserDistance, ShootLayer);
            if (hitInfo.collider.CompareTag("Player"))
            {
                IDamageable player = hitInfo.collider.GetComponent<IDamageable>();
                if (player != null)
                {
                    ////////Edit
                    if (canDamage)
                    {
                        canDamage = false;
                        player.Takedamage(dmgUl, DamageType.Rang, 0);
                        DamageHit().Forget();
                    }
                    //Debug.Log("hit");
                }
            }
        }
        if (Physics2D.Raycast(startLaser.transform.position, bulletTranform.transform.right, laserDistance, ShootLayer))
        {
            RaycastHit2D _hit = Physics2D.Raycast(startLaser.transform.position, bulletTranform.transform.right, laserDistance, ShootLayer);
            DrawRay(startLaser.transform.position, _hit.point);
        }
        else
        {
            DrawRay(startLaser.transform.position, startLaser.transform.position + bulletTranform.transform.right * laserDistance);
        }
    }   

    public void DrawRay(Vector2 startPos, Vector2 endPos)
    {
        laserUltiPrefab.SetPosition(0, startPos);
        laserUltiPrefab.SetPosition(1, endPos);
    }

    public async UniTask DamageHit()
    {
        await UniTask.WaitForSeconds(dpsDamage);
        canDamage = true;
        await UniTask.WaitForSeconds(dpsDamage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SpikeN : MonoBehaviour
{
    public int dmg = 10;
    public int count = 10;
    public Animator animator;
    public float range = 2.5f;
    public float preAtk;
    public bool final;

    private void Start()
    {
        Next().Forget();
    }

    public async UniTask Next()
    {
        await UniTask.WaitForSeconds(preAtk);
        animator.SetTrigger("Go");
    }

    public void End()
    {
        preAtk = 0.2f;
        if (count > 1)
        {
            Move();
            animator.SetTrigger("Reset");
            count--;
            range *= 2f;
            Next().Forget(); ;
            //SpikeN newSpike = Instantiate(gameObject).GetComponent<SpikeN>();
            //newSpike.count--;
        }
        else
        {
            final = true;
            Destroy(gameObject);
        }

        //Destroy(gameObject);
    }

    public void Move()
    {
        Transform player = CinemachineControl.Instance.player;
        if (Vector2.Distance(gameObject.transform.position,player.position) <= range+1)
        {
            transform.position = player.position;
        }
        else
        {            
            Vector3 dir = (player.position - gameObject.transform.position).normalized;
            dir = dir * range;
            dir.z = 0;
            transform.position += dir;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count = 0;
            collision.GetComponent<IDamageable>().Takedamage(dmg,DamageType.Melee,10);
        }
    }

    public void ChangeSortingLayer(string newLayer)
    {
        GetComponent<SpriteRenderer>().sortingLayerName = newLayer;
    }

}

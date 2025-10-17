using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletBounce : BaseBullet
{
    public int maxBunceCount = 3;
    public int bounceCount = 0;    
    private float time = 0;
    public float timer = 3;

    [Range(0f, 1f)] public float value = 0f;
    public bool useCustomColors = true;             

    [Header("Custom 2-Color Gradient")]
    public Color startColor = Color.white;
    public Color endColor = Color.cyan;
    public int maxBound=20;

    [Header("Custom 3-Color Gradient (Editable)")]
    public Color start3Color = Color.yellow;
    public Color mid3Color = new Color(1f, 0.5f, 0f);
    public Color end3Color = Color.red;

    [Header("Target Components")]
    public SpriteRenderer targetSprite;

    void Start()
    {
        ready = true;
        rb.velocity = transform.right * speed;
    }

    private void Update()
    {
        if (ultimate)
        {
            time += Time.deltaTime;
            if (time > timer)
            {
                Destroy(gameObject);
                Expo();
            }
        }        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            bounceCount++;
            SetColorByValue(bounceCount, 1, 3);

            if (bounceCount > maxBunceCount)
            {
                if (!ultimate)
                {
                    Destroy(gameObject);
                    Expo();
                    return;
                }
                else
                {
                    useCustomColors = true;
                    SetColorByValue(bounceCount, 1, maxBound);
                }
            }

            if (!ultimate)
            {
                damage *= 2;
            }
            else
            {
                damage += 10;
            }

            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagUse)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            if (target != null)
            {
                target.Takedamage(damage, DamageType.Rang, knockbackForce);
                KnockBackPush(collision);
            }
            if (collision.gameObject.TryGetComponent(out IObjInteract bulletInteract))
            {
                bulletInteract.Interact(DamageType.Rang);
            }
            Destroy(gameObject);
            Expo();
        }        
    }

    public override void ResetGameObj()
    {
        bounceCount = 0;
    }

    public void SetColorByValue(int value, int minValue = 0, int maxValue = 100)
    {
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        ApplyColor(t);
    }

    public void SetColorByValue(float value)
    {
        ApplyColor(Mathf.Clamp01(value));
    }



    private void ApplyColor(float value)
    {
        Color currentColor;

        if (useCustomColors)
        {
            currentColor = Color.Lerp(startColor, endColor, value);
        }
        else
        {
            if (value < 0.5f)
            {
                float t = value / 0.5f;
                currentColor = Color.Lerp(start3Color, mid3Color, t);
            }
            else
            {
                float t = (value - 0.5f) / 0.5f;
                currentColor = Color.Lerp(mid3Color, end3Color, t);
            }
        }

        if (targetSprite != null)
            targetSprite.color = currentColor;
    }
}

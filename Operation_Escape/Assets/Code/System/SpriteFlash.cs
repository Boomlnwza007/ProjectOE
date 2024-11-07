using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    public Material flashMaterial;
    private Material originalMaterial;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public float duration = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        originalColor = spriteRenderer.color;
    }

    public void Flash()
    {
        StartCoroutine(FlashRun());
    }

    private IEnumerator FlashRun()
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.material = flashMaterial;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = originalColor;
        spriteRenderer.material = originalMaterial;
    }

    public void ReFlash()
    {
        spriteRenderer.color = originalColor;
        spriteRenderer.material = originalMaterial;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{ 
    public List<LootItem> lootlist = new List<LootItem>();

    public void InstantiateLoot(float dropChamge)
    {
        InstantiateLoot(dropChamge, 2);
    }

    public void InstantiateLoot(float dropChamge,float radious)
    {
        foreach (var item in lootlist)
        {
            for (int i = 0; i < item.minDrop + dropChamge; i++)
            {
                Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * radious;
                GameObject lootGameObject = Instantiate(item.droppedItemPrefab, dropPosition, Quaternion.identity);
                Rigidbody2D rb = lootGameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(Random.insideUnitCircle * 2f, ForceMode2D.Impulse);
                }
            }
        }
        foreach (var item in lootlist)
        {
            if ((item.maxDrop - item.minDrop) != 0)
            {
                if (Random.value < item.dropChance)
                {
                    int amountToDrop = Random.Range(0, (item.maxDrop - item.minDrop) + 1);
                    for (int i = 0; i < amountToDrop; i++)
                    {
                        Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * radious;
                        GameObject lootGameObject = Instantiate(item.droppedItemPrefab, dropPosition, Quaternion.identity);
                        Rigidbody2D rb = lootGameObject.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.AddForce(Random.insideUnitCircle * 2f, ForceMode2D.Impulse);
                        }
                    }
                }
            }
        }
    }
}

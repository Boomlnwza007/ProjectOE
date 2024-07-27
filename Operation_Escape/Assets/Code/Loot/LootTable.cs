using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{ 
    public List<LootItem> lootlist = new List<LootItem>();

    public void InstantiateLoot(float dropMinus)
    {
        foreach (var item in lootlist)
        {
            for (int i = 0; i <  item.minDrop/ dropMinus; i++)
            {
                Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 1f;
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
            if (Random.value < item.dropChance)
            {
                int amountToDrop = Random.Range(0, (item.maxDrop - item.minDrop) + 1);
                for (int i = 0; i < amountToDrop/ dropMinus; i++)
                {
                    Vector2 dropPosition = (Vector2)transform.position + Random.insideUnitCircle * 1f;
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

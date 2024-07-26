using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LootItem : MonoBehaviour
{
    //public Sprite lootSprite;
    public string lootName;
    public int id;
    [Tooltip("โอกาสในการดรอปไอเทมนี้ (ค่า 0.0 ถึง 1.0)")]
    public int dropChance;
    public int minDrop;
    public int maxDrop;
    public GameObject droppedItemPrefab;

    public LootItem(string lootName, int dropChance, int minDrop, int maxDrop, int id)
    {
        this.id = id;
        this.lootName = lootName;
        this.dropChance = dropChance;
        this.minDrop = minDrop;
        this.maxDrop = maxDrop;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemLoot", menuName = "Loot System/ItemLoot")]
public class LootItem : ScriptableObject
{
    //public Sprite lootSprite;
    public string lootName;
    public int id;
    [Tooltip("�͡��㹡�ô�ͻ������� (��� 0.0 �֧ 1.0)")]
    public float dropChance;
    public int minDrop;
    public int maxDrop;
    public GameObject droppedItemPrefab;

    public LootItem(string lootName, float dropChance, int minDrop, int maxDrop, int id)
    {
        this.id = id;
        this.lootName = lootName;
        this.dropChance = dropChance;
        this.minDrop = minDrop;
        this.maxDrop = maxDrop;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewID", menuName = "Custom/ID")]
public class ID : ScriptableObject
{
    [SerializeField] public GameObject[] Item;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item",menuName ="Inventory/New Item")]
public class ItemData_SO :ScriptableObject
{
    public List<ItemDetails> itemData;
}

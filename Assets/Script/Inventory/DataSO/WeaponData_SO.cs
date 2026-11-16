using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/New Weapon")]
public class WeaponData_SO : ScriptableObject
{
    public List<WeaponDetails> weaponData;
}
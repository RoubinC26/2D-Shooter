using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public static InventoryManager instance;

    public ItemData_SO inventoryData;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    void OnEnable()
    {
        EventHandler.ItemPickedEvent += AddItemToInventory;
    }

    void OnDisable()
    {
        EventHandler.ItemPickedEvent -= AddItemToInventory;
    }

    void AddItemToInventory(ItemDetails newItem)
    {
        // 安全过滤
        if (newItem == null || string.IsNullOrEmpty(newItem.itemName))
            return;

        // 1. 遍历背包，查找同名物品，直接叠加数量
        foreach (var item in inventoryData.itemData)
        {
            if (item.itemName == newItem.itemName)
            {
                item.itemHeld++;
                Debug.Log("合并物品：" + item.itemName + " 数量：" + item.itemHeld);
                return;
            }
        }

        // 2. 背包里没有 → 才添加新物品
        ItemDetails copyItem = new ItemDetails();
        copyItem.itemName = newItem.itemName;
        copyItem.itemInfo = newItem.itemInfo;      
        copyItem.itemImage = newItem.itemImage;    
        copyItem.equip = newItem.equip;
        copyItem.itemType = newItem.itemType;
        copyItem.equipPrefab = newItem.equipPrefab;
        copyItem.effectValue = newItem.effectValue;
        copyItem.bulletDamage = newItem.bulletDamage;
        copyItem.useType = newItem.useType;
        copyItem.grenadeDamage = newItem.grenadeDamage;
        copyItem.grenadeRadius = newItem.grenadeRadius;
        copyItem.itemHeld = 1;

        inventoryData.itemData.Add(copyItem);
        Debug.Log("新增物品：" + newItem.itemName);
    }

    // 使用物品
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventoryData.itemData.Count)
            return;

        ItemDetails item = inventoryData.itemData[slotIndex];

        if (item.itemHeld <= 0)
            return;

        item.itemHeld--;

        if (item.equip)
        {
            EventHandler.CallEquipItemEvent(item);
        }
        else
        {
            EventHandler.CallUseConsumableEvent(item);
        }
    }
}
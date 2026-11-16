using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BagUIManager : MonoBehaviour
{
    [Header("背包面板")]
    public GameObject bagPanel;

    [Header("格子父物体 Slots")]
    public Transform content;

    [Header("物品描述文本")]
    public TextMeshProUGUI itemDescriptionText;

    [Header("Use按钮")]
    public Button useButton;

    //空格子默认背景图片
    [Header("空格子默认背景")]
    public Sprite defaultSlotSprite;

    public static bool IsBagOpen = false;

    private List<Transform> slotList = new List<Transform>();
    private int selectedSlotIndex = -1;

    void Awake()
    {
        if (bagPanel != null)
            bagPanel.SetActive(false);

        IsBagOpen = false;

        slotList.Clear();
        for (int i = 0; i < content.childCount; i++)
        {
            Transform slot = content.GetChild(i);
            slotList.Add(slot);

            Button slotBtn = slot.GetComponent<Button>();
            if (slotBtn != null)
            {
                int index = i;
                slotBtn.onClick.AddListener(() => OnSlotClicked(index));
            }
        }

        if (useButton != null)
        {
            useButton.onClick.AddListener(OnUseButtonClicked);
        }
    }

    void OnEnable()
    {
        EventHandler.ToggleBagUIEvent += ToggleBag;
        EventHandler.ItemPickedEvent += RefreshUI;
    }

    void OnDisable()
    {
        EventHandler.ToggleBagUIEvent -= ToggleBag;
        EventHandler.ItemPickedEvent -= RefreshUI;
    }

    public void ToggleBag()
    {
        if (bagPanel != null)
        {
            bagPanel.SetActive(!bagPanel.activeSelf);

            IsBagOpen = bagPanel.activeSelf;

            if (bagPanel.activeSelf)
            {
                RefreshUI(null);
                ClearSelection();
            }
        }
    }

    public void CloseBag()
    {
        if (bagPanel != null)
        {
            bagPanel.SetActive(false);

            IsBagOpen = false;
        }
    }

    void RefreshUI(ItemDetails item)
    {
        if (InventoryManager.instance == null || content == null) return;

        var inventory = InventoryManager.instance.inventoryData;
        if (inventory == null || inventory.itemData == null) return;

        // 把所有格子重置为默认状态：UI-38 + 00
        for (int i = 0; i < slotList.Count; i++)
        {
            // 重置背景为UI-38
            Image icon = slotList[i].GetComponent<Image>();
            if (icon != null && defaultSlotSprite != null)
            {
                icon.sprite = defaultSlotSprite;
            }

            // 重置数量为00
            TextMeshProUGUI countText = slotList[i].Find("Count").GetComponent<TextMeshProUGUI>();
            if (countText != null)
            {
                countText.text = "00";
            }
        }

        // 2. 只给数量>0的物品赋值
        int slotIndex = 0;
        for (int i = 0; i < inventory.itemData.Count && slotIndex < slotList.Count; i++)
        {
            var data = inventory.itemData[i];
            if (data.itemHeld <= 0) continue;

            var slot = slotList[slotIndex];

            // 替换为物品图片
            Image icon = slot.GetComponent<Image>();
            if (icon != null && data.itemImage != null)
            {
                icon.sprite = data.itemImage;
            }

            // 替换为实际数量
            TextMeshProUGUI countText = slot.Find("Count").GetComponent<TextMeshProUGUI>();
            if (countText != null)
            {
                countText.text = data.itemHeld.ToString();
            }

            slotIndex++;
        }
    }

    void OnSlotClicked(int index)
    {
        selectedSlotIndex = index;

        var inventory = InventoryManager.instance.inventoryData;
        if (inventory == null || inventory.itemData == null) return;

        int count = 0;
        ItemDetails selectedItem = null;
        for (int i = 0; i < inventory.itemData.Count; i++)
        {
            if (inventory.itemData[i].itemHeld <= 0) continue;

            if (count == index)
            {
                selectedItem = inventory.itemData[i];
                break;
            }
            count++;
        }

        if (selectedItem != null && itemDescriptionText != null)
        {
            itemDescriptionText.text = selectedItem.itemInfo;
        }
        else if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "This is for item descriptions.";
        }

        HighlightSelectedSlot(index);
    }

    public void OnUseButtonClicked()
    {
        if (selectedSlotIndex < 0)
        {
            Debug.Log("没有选中任何物品");
            return;
        }

        if (InventoryManager.instance == null || InventoryManager.instance.inventoryData == null)
        {
            Debug.LogError("InventoryManager 为空！");
            return;
        }

        var inventory = InventoryManager.instance.inventoryData;
        if (inventory == null || inventory.itemData == null) return;

        // 找到选中的物品
        int count = 0;
        ItemDetails selectedItem = null;
        int selectedItemIndex = -1;
        for (int i = 0; i < inventory.itemData.Count; i++)
        {
            if (inventory.itemData[i].itemHeld <= 0) continue;

            if (count == selectedSlotIndex)
            {
                selectedItem = inventory.itemData[i];
                selectedItemIndex = i;
                break;
            }
            count++;
        }

        if (selectedItem == null)
        {
            Debug.Log("选中的物品不存在");
            return;
        }

        Debug.Log("点击了Use按钮，使用物品：" + selectedItem.itemName);

        // 消耗物品数量
        inventory.itemData[selectedItemIndex].itemHeld--;

        // 处理使用逻辑
        if (selectedItem.equip)
        {
            EventHandler.CallEquipItemEvent(selectedItem);
            Debug.Log("装备物品：" + selectedItem.itemName);
        }
        else
        {
            EventHandler.CallUseConsumableEvent(selectedItem);
            Debug.Log("使用消耗品：" + selectedItem.itemName);
        }

        // 刷新UI并清空选择
        RefreshUI(null);
        ClearSelection();
    }

    void HighlightSelectedSlot(int index)
    {
        foreach (var slot in slotList)
        {
            Image img = slot.GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.white;
            }
        }

        if (index >= 0 && index < slotList.Count)
        {
            Image selectedImg = slotList[index].GetComponent<Image>();
            if (selectedImg != null)
            {
                selectedImg.color = Color.yellow;
            }
        }
    }

    void ClearSelection()
    {
        selectedSlotIndex = -1;
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "This is for item descriptions.";
        }
        HighlightSelectedSlot(-1);
    }
}
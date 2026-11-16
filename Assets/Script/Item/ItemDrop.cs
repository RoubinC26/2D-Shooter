using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [Header("物品数据")]
    public ItemDetails item;

    // 设置掉落的物品
    public void SetItem(ItemDetails newItem)
    {
        item = newItem;
        // 给掉落物显示物品的图片
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && item != null)
        {
            sr.sprite = item.itemImage;
        }
    }

    // 玩家碰到自动拾取
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && item != null)
        {
            // 调用现有的拾取事件，自动进背包
            EventHandler.CallItemPickedEvent(item);
            Destroy(gameObject);
        }
    }
}
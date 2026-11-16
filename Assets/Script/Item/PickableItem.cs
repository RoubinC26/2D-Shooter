using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("绑定物品数据")]
    public ItemDetails itemDetails;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        EventHandler.CallItemPickedEvent(itemDetails);
        Destroy(gameObject);
    }
}

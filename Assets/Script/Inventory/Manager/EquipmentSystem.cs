using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [Header("武器挂载点")]
    public Transform weaponHolder;
    [Header("帽子挂载点")]
    public Transform hatHolder;

    private GameObject currentWeapon;
    private PlayerShoot playerShoot;

    private void Awake()
    {
        // 获取PlayerShoot组件
        playerShoot = GetComponentInParent<PlayerShoot>();
    }

    private void OnEnable()
    {
        EventHandler.EquipItemEvent += EquipItem;
    }

    private void OnDisable()
    {
        EventHandler.EquipItemEvent -= EquipItem;
    }

    private void EquipItem(ItemDetails item)
    {
        if (item.equipPrefab == null) return;

        // 处理武器
        if (item.itemType == ItemType.Weapon)
        {
            // 销毁旧武器
            foreach (Transform child in weaponHolder)
            {
                Destroy(child.gameObject);
            }

            // 生成新武器
            currentWeapon = Instantiate(item.equipPrefab, weaponHolder);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;

            //自动找到新枪的Firepoint并赋值给PlayerShoot
            Transform newFirepoint = currentWeapon.transform.Find("Firepoint");
            if (newFirepoint != null && playerShoot != null)
            {
                playerShoot.firePoint = newFirepoint;
            }
        }

        // 处理帽子
        if (item.itemType == ItemType.Hat)
        {
            foreach (Transform child in hatHolder)
            {
                Destroy(child.gameObject);
            }
            Instantiate(item.equipPrefab, hatHolder);
        }
    }
}
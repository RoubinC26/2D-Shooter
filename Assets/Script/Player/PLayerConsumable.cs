using UnityEngine;

public class PlayerConsumable : MonoBehaviour
{
    public static PlayerConsumable Instance;

    [Header("防御值")]
    public int defense = 1;

    [Header("武器伤害")]
    public int NORMAL_GUN_DAMAGE = 2;    // 普通枪固定2
    public int SUPER_GUN_DAMAGE = 5;     // 超级枪固定5
    public int currentWeaponDamage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //游戏一开始为2
        currentWeaponDamage = NORMAL_GUN_DAMAGE;
    }

    private void OnEnable()
    {
        EventHandler.EquipItemEvent += OnEquipItem;
        EventHandler.UseConsumableEvent += OnUseConsumable;
        EventHandler.ItemPickedEvent += OnItemPicked;
    }

    private void OnDisable()
    {
        EventHandler.EquipItemEvent -= OnEquipItem;
        EventHandler.UseConsumableEvent -= OnUseConsumable;
        EventHandler.ItemPickedEvent -= OnItemPicked;
    }

    private void OnItemPicked(ItemDetails item)
    {
        if (item.itemName.Contains("子弹") || item.itemName.Contains("Ammunition") || item.itemType == ItemType.Bullet)
        {
            if (PlayerStatBar.Instance != null)
            {
                PlayerStatBar.Instance.AddBullet(100);
                Debug.Log("拾取子弹：+100 发");
            }
        }
    }

    private void OnEquipItem(ItemDetails item)
    {
        if (item.itemType == ItemType.Hat)
        {
            defense = item.effectValue;
            Debug.Log("帽子防御：" + defense);
        }
        else if (item.itemType == ItemType.Weapon)
        {
            if (item.itemName == "SuperGun")
            {
                currentWeaponDamage = SUPER_GUN_DAMAGE;
                Debug.Log("超级枪 = 5 伤害");
            }
            else
            {
                currentWeaponDamage = NORMAL_GUN_DAMAGE;
                Debug.Log("普通枪 = 2 伤害");
            }
        }
    }

    private void OnUseConsumable(ItemDetails item)
    {
        if (item.itemName == "Ammunition")
        {
            PlayerStatBar.Instance?.AddBullet(100);
            return;
        }

        if (item.itemName == "SuperGun")
        {
            currentWeaponDamage = SUPER_GUN_DAMAGE;
            Debug.Log("使用超级枪 = 5");
            return;
        }

        // 【任何其他枪 = 2】
        if (item.itemType == ItemType.Weapon)
        {
            currentWeaponDamage = NORMAL_GUN_DAMAGE;
            Debug.Log("切回普通枪 = 2");
            return;
        }

        if (item.useType == ConsumableUseType.Heal)
        {
            int heal = item.effectValue;
            PlayerStatBar.Instance.currentHealth = Mathf.Min(PlayerStatBar.Instance.maxHealth, PlayerStatBar.Instance.currentHealth + heal);
            PlayerStatBar.Instance.UpdateAllUI();
        }
    }

    public void ForceNormalGun()
    {
        currentWeaponDamage = NORMAL_GUN_DAMAGE;
        Debug.Log("强制重置：普通枪 2 伤害");
    }
}
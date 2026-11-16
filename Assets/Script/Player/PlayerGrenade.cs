using UnityEngine;

public class PlayerGrenade : MonoBehaviour
{
    public static PlayerGrenade Instance;

    [Header("手雷预制体")]
    public GameObject grenadePrefab;
    [Header("爆炸特效")]
    public GameObject explosionEffect;

    [Header("手雷属性")]
    public int grenadeDamage = 50;
    public float explosionRadius = 3f;

    [Header("放雷冷却")]
    public float placeCD = 1f;
    private float cdTimer;

    public bool IsHoldingGrenade { get; private set; }

    void Awake()
    {
        Instance = this;
        IsHoldingGrenade = false;
    }

    void Update()
    {
        cdTimer -= Time.deltaTime;

        if (BagUIManager.IsBagOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && cdTimer <= 0)
        {
            //只加这一段，完全用你现有的结构
            foreach (var item in InventoryManager.Instance.inventoryData.itemData)
            {
                if (item.itemName == "Grenade" && item.itemHeld > 0)
                {
                    // 直接减数量（和你背包Use逻辑一样）
                    item.itemHeld--;

                    // 扔雷
                    PlaceGrenade();
                    cdTimer = placeCD;
                    return;
                }
            }
        }
    }

    void PlaceGrenade()
    {
        GameObject g = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        Grenade gr = g.GetComponent<Grenade>();
        gr.damage = grenadeDamage;
        gr.explosionRadius = explosionRadius;
        gr.explosionEffect = explosionEffect;
    }
}



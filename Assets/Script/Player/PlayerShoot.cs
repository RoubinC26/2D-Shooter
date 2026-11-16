using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("射击设置")]
    public Transform firePoint;
    public float fireRate = 0.08f;
    private float cooldown;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 游戏结束直接返回
        if (GameStateManager.Instance != null && GameStateManager.Instance.isGameOver)
        {
            moveDir = Vector2.zero;
            rb.velocity = Vector2.zero;
            return;
        }

        if (BagUIManager.IsBagOpen || (PlayerGrenade.Instance != null && PlayerGrenade.Instance.IsHoldingGrenade))
        {
            moveDir = Vector2.zero;
            rb.velocity = Vector2.zero;
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(h, v).normalized;

        cooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && cooldown <= 0)
        {
            if (PlayerStatBar.Instance != null && PlayerStatBar.Instance.HasBullets())
            {
                Shoot();
                PlayerStatBar.Instance.ShootBullet();
                cooldown = fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed;
    }

    void Shoot()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 dir = (mouseWorld - firePoint.position).normalized;

        //读取正确伤害
        int damage = PlayerConsumable.Instance.NORMAL_GUN_DAMAGE;

        if (PlayerConsumable.Instance != null)
            damage = PlayerConsumable.Instance.currentWeaponDamage;

        EventHandler.CallBulletFireEvent(
            ParticleEffectType.Bullet,
            firePoint.position,
            dir,
            damage
        );

        Debug.Log("当前子弹伤害：" + damage);
    }

    public void SetPlayerBullet(Bullet bullet)
    {
        if (bullet != null)
            bullet.isPlayerBullet = true;
    }

    public void TakeDamage(float damage)
    {
        if (PlayerStatBar.Instance != null)
        {
            PlayerStatBar.Instance.TakeDamage(damage);
        }
    }
}
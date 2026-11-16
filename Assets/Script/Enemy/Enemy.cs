using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("基础设置")]
    public float moveSpeed = 2f;
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public int bulletCount = 8;

    [Header("血量")]
    public float maxHealth = 60f;
    public float currentHealth;

    [Header("血条")]
    public EnemyHealthBar healthBarPrefab;
    public EnemyHealthBar healthBar;

    private Transform player;
    private bool isAlive;
    private float cooldownTimer;

    private List<AStarNode> currentPath;
    private Vector2 targetPos;
    public float pathUpdateCD = 0.3f;
    private float pathTimer;

    [Header("==== 敌人子弹伤害 ====")]
    public int bulletDamage = 10;

    [Header("==== 掉落物品 ====")]
    public List<GameObject> dropItems;

    public GameObject healthBarUI;
    private Vector3 originalScale;

    [Header("毒液设置")]
    public bool leavePoisonOnDeath = false;
    public GameObject poisonAreaPrefab;

    [Header("手榴弹掉落")]
    public bool canDropGrenade = false;
    public GameObject grenadePickupPrefab;
    [Range(0, 100)] public int grenadeDropChance = 30;

    public ParticleEffectType enemyBulletType;

    private Animator _anim;
    private SpriteRenderer sr;
    private Vector3 _lastPos;

    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        //强制把Prefab上的赋值固化到实例里
        if (healthBarPrefab == null)
        {
            Debug.LogError($"[{gameObject.name}] healthBarPrefab 为空！请检查Prefab赋值！");
        }
    }

    private void OnDisable()
    {
        // 敌人被回收时，彻底销毁血条，避免下次复用出问题
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
            healthBar = null;
        }
    }

    public void Init()
    {
        // 先销毁旧血条
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
            healthBar = null;
        }

        // 【兜底：如果实例的healthBarPrefab为空，直接从Prefab模板取】
        if (healthBarPrefab == null)
        {
            // 找当前敌人的Prefab模板（如果是从Prefab实例化的，会有这个引用）
#if UNITY_EDITOR
            // 找当前敌人的Prefab模板（如果是从Prefab实例化的，会有这个引用）
            Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefab != null)
            {
                healthBarPrefab = ((GameObject)prefab).GetComponent<Enemy>().healthBarPrefab;
                Debug.Log($"[{gameObject.name}] 从Prefab模板恢复了healthBarPrefab");
            }
#endif
        }

        // 重置血量和状态
        currentHealth = maxHealth;
        isAlive = true;
        pathTimer = 0;
        cooldownTimer = 0;
        currentPath = null;

        // 找玩家
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Enemy.Init()：找不到Player！");

        // 重置碰撞体和状态
        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);

        // 创建血条
        if (healthBarPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Enemy.Init()：场景里找不到Canvas！血条无法创建！");
                return;
            }

            healthBar = Instantiate(healthBarPrefab, canvas.transform);
            healthBar.SetEnemy(transform);
            healthBar.UpdateHp(currentHealth, maxHealth);
            healthBar.gameObject.SetActive(true);

            Debug.Log($"Enemy.Init()：成功创建血条！血量：{currentHealth}/{maxHealth}");
        }
        else
        {
            Debug.LogError($"Enemy.Init()：[{gameObject.name}] healthBarPrefab 仍然为空！请检查Prefab赋值！");
        }

        // 其他状态重置
        if (healthBarUI != null)
            healthBarUI.SetActive(true);

        if (_anim != null)
        {
            _anim.Play("Idle", 0, 0);
            _anim.SetFloat("Speed", 0);
        }

        _lastPos = transform.position;
    }


    void Update()
    {
        //游戏结束 → 敌人完全停止
        if (GameStateManager.Instance != null && GameStateManager.Instance.isGameOver)
            return;

        if (!isAlive || player == null) return;

        float dis = Vector2.Distance(transform.position, player.position);

        if (player.position.x > transform.position.x)
            sr.flipX = false;
        else
            sr.flipX = true;

        pathTimer += Time.deltaTime;
        if (pathTimer >= pathUpdateCD)
        {
            pathTimer = 0;
            currentPath = AStarManager.Instance.FindPath(transform.position, player.position);
        }

        MoveByPath();

        if (dis <= attackRange)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                FireSpreadBullets();
                cooldownTimer = attackCooldown;
            }
        }

        if (_anim != null)
        {
            float moveSpeedVal = (transform.position - _lastPos).magnitude / Time.deltaTime;
            _anim.SetFloat("Speed", moveSpeedVal);
            _lastPos = transform.position;
        }
    }

    void MoveByPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        targetPos = currentPath[0].worldPos;
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.2f)
            currentPath.RemoveAt(0);
    }

    void FireSpreadBullets()
    {
        if (_anim != null)
            _anim.Play("Attack", 0, 0);

        float step = 360f / bulletCount;
        float angle = 0;
        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            EventHandler.CallEnemyBulletFireEvent(enemyBulletType, transform.position, dir, bulletDamage);
            angle += step;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (healthBar != null)
            healthBar.UpdateHp(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (!isAlive) return;
        isAlive = false;

        if (_anim != null)
            _anim.Play("Death", 0, 0);

        GetComponent<Collider2D>().enabled = false;

        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (healthBarUI != null) healthBarUI.SetActive(false);

        if (WaveManager.Instance != null && !WaveManager.Instance.waveHasDropped && dropItems.Count > 0)
        {
            int rand = Random.Range(0, 2);
            if (rand == 1)
            {
                Instantiate(dropItems[Random.Range(0, dropItems.Count)], transform.position, Quaternion.identity);
                WaveManager.Instance.waveHasDropped = true;
            }
        }

        if (canDropGrenade && grenadePickupPrefab != null)
        {
            if (Random.Range(0, 100) < grenadeDropChance)
                Instantiate(grenadePickupPrefab, transform.position, Quaternion.identity);
        }

        EnemyShadowSplitter splitter = GetComponent<EnemyShadowSplitter>();
        if (splitter != null) splitter.Split();

        if (leavePoisonOnDeath && poisonAreaPrefab != null)
            Instantiate(poisonAreaPrefab, transform.position, Quaternion.identity);

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyDead(gameObject);

        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(0.8f);

        // 清空血条引用
        healthBar = null;

        gameObject.SetActive(false);
        if (PoolManager.Instance != null)
            PoolManager.Instance.ReleaseEnemy(gameObject);
    }


    /// <summary>
    /// 对象池复用前必须调用：重置所有状态（血量、死亡状态、血条）
    /// </summary>
    public void ResetForPool()
    {
        // 强制重置血量（解决0血出生）
        currentHealth = maxHealth;
        isAlive = true;

        // 重置碰撞体
        GetComponent<Collider2D>().enabled = true;

        // 清空旧路径，防止移动异常
        currentPath = null;

        // 重置冷却
        cooldownTimer = 0;
        pathTimer = 0;
    }
}


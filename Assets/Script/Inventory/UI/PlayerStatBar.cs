using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatBar : Singleton<PlayerStatBar>
{
    [Header("==== 玩家血量 ====")]
    public float maxHealth = 100;
    public float currentHealth;

    [Header("==== 初始子弹数量 ====")]
    public int maxBullet = 900;
    private int currentBullet;

    [Header("==== UI拖拽 ====")]
    public Image healthFill;
    public Image bulletFill;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI ammoText;

    //初始化锁，防止刚启动就触发失败
    private bool isGameReady = false;

    void Start()
    {
        // 1. 初始化数值
        currentHealth = maxHealth;
        currentBullet = maxBullet;

        // 2. 第一次更新UI（此时不触发失败判断）
        UpdateAllUI(false);

        // 3. 标记游戏准备完成，之后才允许触发失败
        isGameReady = true;
    }

    // 受伤（只在这里做减伤）
    public void TakeDamage(float damage)
    {
        int def = 0;
        if (PlayerConsumable.Instance != null)
        {
            def = PlayerConsumable.Instance.defense;
        }

        damage = Mathf.Max(1, damage - def);
        currentHealth = Mathf.Max(0, currentHealth - damage);

        UpdateAllUI();
    }

    // 单独的回血方法
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateAllUI();
    }

    public void ShootBullet()
    {
        if (currentBullet > 0)
        {
            currentBullet--;
            UpdateAllUI();
        }
    }

    public bool HasBullets() => currentBullet > 0;

    public void ReloadAmmo()
    {
        currentBullet = maxBullet;
        UpdateAllUI();
    }

    //增加子弹数量（上限为 maxBullet）
    public void AddBullet(int amount)
    {
        currentBullet = Mathf.Min(maxBullet, currentBullet + amount);
        UpdateAllUI();
    }

    // 重载：可以控制是否触发失败判断
    public void UpdateAllUI(bool triggerLoseCheck = true)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
        hpText.text = $"{currentHealth}/{maxHealth}";
        bulletFill.fillAmount = (float)currentBullet / maxBullet;
        ammoText.text = $"{currentBullet}/{maxBullet}";

        // 只有游戏准备完成、且允许触发检查时，才判断失败
        if (triggerLoseCheck && isGameReady && GameStateManager.Instance != null && !GameStateManager.Instance.isGameOver)
        {
            if (currentHealth <= 0 || currentBullet <= 0)
            {
                GameStateManager.Instance.GameLose();
            }
        }
    }
}
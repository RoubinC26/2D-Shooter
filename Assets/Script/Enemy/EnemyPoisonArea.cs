using UnityEngine;
using System.Collections;

public class EnemyPoisonArea : MonoBehaviour
{
    [Header("毒液设置")]
    public float existTime = 5f;
    public float damagePerTick = 5;    // 每次扣多少血
    public float tickInterval = 0.5f; // 每隔多久扣一次

    private bool isPlayerInPoison = false;

    void Start()
    {
        Destroy(gameObject, existTime);
        StartCoroutine(DamageLoop());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPoison = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPoison = false;
        }
    }

    IEnumerator DamageLoop()
    {
        while (true)
        {
            if (isPlayerInPoison)
            {
                // 强制找到玩家并扣血（最稳写法）
                PlayerShoot player = FindObjectOfType<PlayerShoot>();
                if (player != null)
                {
                    player.TakeDamage(damagePerTick);
                }
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }
}
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public int damage = 10;
    public float explosionRadius = 2f;
    public GameObject explosionEffect;

    private bool _alreadyBoom = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 已经炸过 直接返回
        if (_alreadyBoom) return;

        if (other.CompareTag("Enemy"))
        {
            BoomOnce(other);
        }
    }

    void BoomOnce(Collider2D enemyCol)
    {
        _alreadyBoom = true;

        // 爆炸特效自动销毁
        if (explosionEffect != null)
        {
            GameObject eff = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(eff, 2f);
        }

        // 只对碰到的这个敌人，扣10点，不走范围遍历
        Enemy e = enemyCol.GetComponent<Enemy>();
        if (e == null)
        {
            // 子碰撞体拿父物体
            e = enemyCol.GetComponentInParent<Enemy>();
        }

        if (e != null)
        {
            // 强行只减10，锁死，不会多扣
            e.currentHealth -= 10;

            // 限制血量最低0
            if (e.currentHealth < 0) e.currentHealth = 0;

            // 刷新血条
            if (e.healthBar != null)
            {
                e.healthBar.UpdateHp(e.currentHealth, e.maxHealth);
            }

            // 血量到0才死亡
            if (e.currentHealth <= 0)
            {
                e.Die();
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
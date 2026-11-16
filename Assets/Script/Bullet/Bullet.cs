using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("==== ×Óµ¯ÉËº¦ÉèÖÃ ====")]
    public int damage = 2;

    public bool isPlayerBullet;
    public float speed = 10;

    private Vector2 dir;

    void Update()
    {
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    public void SetDirection(Vector2 direction) => dir = direction;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<Enemy>(out var e))
                    e.TakeDamage(damage);

                gameObject.SetActive(false);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<PlayerShoot>(out var p))
                    p.TakeDamage(damage);

                gameObject.SetActive(false);
            }
        }
    }
}
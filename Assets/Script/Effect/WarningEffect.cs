using UnityEngine;

public class WarningEffect : MonoBehaviour
{
    [Header("设置")]
    public float lifeTime = 0.5f;
    public float flashSpeed = 10f;

    private SpriteRenderer sr;

    void Start()
    {
        // 只获取一次SpriteRenderer
        sr = GetComponent<SpriteRenderer>();
        // 保证一定会销毁
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (sr == null) return;

        // 实现闪烁效果（透明度在0-1之间来回）
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
        sr.color = new Color(1f, 0f, 0f, alpha);
    }
}
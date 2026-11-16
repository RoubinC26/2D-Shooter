using UnityEngine;

public class PlayerGrenadeAim : MonoBehaviour
{
    public static PlayerGrenadeAim Instance;

    [Header("手雷设置")]
    public GameObject grenadePrefab;
    public float throwPower = 8f;
    public float heightFactor = 3f;

    // 状态
    public bool isAiming = false;
    private Vector2 aimStartMouse;

    void Awake()
    {
        Instance = this;
    }

    // 背包Use调用：进入瞄准状态
    public void EnterAimMode()
    {
        isAiming = true;
        aimStartMouse = Input.mousePosition;
        Debug.Log("进入手雷瞄准，按住鼠标拖动瞄准，松开投掷");
    }

    void Update()
    {
        if (!isAiming) return;

        // 按下鼠标：记录起点
        if (Input.GetMouseButtonDown(0))
        {
            aimStartMouse = Input.mousePosition;
        }

        // 按住中：可以一直拖动调整方向
        if (Input.GetMouseButton(0))
        {
            // 这里后面可以给你加抛物线预览线
        }

        // 松开鼠标：正式投掷
        if (Input.GetMouseButtonUp(0))
        {
            ThrowByDrag();
            isAiming = false;
        }

        // 右键取消瞄准
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = false;
        }
    }

    void ThrowByDrag()
    {
        if (grenadePrefab == null)
        {
            Debug.LogError("赋值手雷预制体！");
            return;
        }

        Vector2 startWorld = Camera.main.ScreenToWorldPoint(aimStartMouse);
        Vector2 endWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = (endWorld - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(endWorld, transform.position);

        // 生成手雷
        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        // 弧形投掷力
        Vector2 force = dir * throwPower * distance * 0.2f + Vector2.up * heightFactor;
        rb.velocity = force;
    }
}
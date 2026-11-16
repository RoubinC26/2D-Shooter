using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    private Transform player;
    private Vector3 exitPos;

    [Tooltip("箭头和玩家的距离")]
    public float followDistance = 2.2f;

    [Tooltip("玩家离出口多近时，箭头消失")]
    public float reachDistance = 1.5f;

    // 初始化目标
    public void Init(Transform playerTransform, Vector3 exitPosition)
    {
        player = playerTransform;
        exitPos = exitPosition;
    }

    void Update()
    {
        if (player == null) return;

        // 玩家离出口足够近，直接销毁箭头
        float distToExit = Vector3.Distance(player.position, exitPos);
        if (distToExit < reachDistance)
        {
            Destroy(gameObject);
            return;
        }

        // 1. 计算玩家到出口的方向
        Vector3 dirToExit = exitPos - player.position;
        dirToExit.z = 0;
        dirToExit.Normalize();

        // 2. 箭头永远在玩家前方，且正对着出口
        transform.position = player.position + dirToExit * followDistance;

        // 3. 箭头面朝出口
        float angle = Mathf.Atan2(dirToExit.y, dirToExit.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
using UnityEngine;

public class EnemyShadowSplitter : MonoBehaviour
{
    [Header("分裂设置")]
    public float splitOffset = 0.6f;

    public void Split()
    {
        SpawnBug(Vector3.left * splitOffset);
        SpawnBug(Vector3.right * splitOffset);
    }

    void SpawnBug(Vector3 offset)
    {
        Vector3 pos = transform.position + offset;

        // 生成小虫
        EventHandler.CallEnemySpawnEvent(EnemyType.SmallInsect, pos);
    }
}
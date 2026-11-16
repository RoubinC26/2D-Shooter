using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public LevelConfig_SO levelConfigSO;
    public int currentLevelID;

    void Start()
    {
        // 自动找玩家,解决场景不匹配问题
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            WaveManager.Instance.player = player.transform;

        // 启动当前关卡
        WaveManager.Instance.StartLevel(currentLevelID, levelConfigSO);
    }
}
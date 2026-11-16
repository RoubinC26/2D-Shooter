using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerHelper : MonoBehaviour
{
    // 加载第二关的方法，你在第一关通关后调用它
    public void LoadSecondLevel()
    {
        // 1. 清空敌人对象池，防止第一关敌人串到第二关
        PoolManager.Instance.ClearEnemyPool();

        // 2. 重置波次管理器的状态
        WaveManager.Instance.ResetWaves();

        // 3. 加载第二关场景（场景名改成你自己的）
        SceneManager.LoadScene("Purple");
    }
}
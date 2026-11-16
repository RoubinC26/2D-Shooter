using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 防止重复触发
        if (hasTriggered) return;

        // 只有玩家碰到才触发
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("第二关通关！");

           /* // 弹出恭喜通关窗口
            if (GameEndPop.Instance != null)
            {
                PlayerShoot.isGameWin = true;
                GameEndPop.Instance.ShowWinPop();
            }*/
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    // 初始值必须设为 false，防止意外触发
    public bool isGameOver = false;

    private GameObject losePanel;
    private GameObject winPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void FindPanels()
    {
        if (losePanel == null) losePanel = FindAnyObjectByName("LosePanel");
        if (winPanel == null) winPanel = FindAnyObjectByName("WinPanel");
    }

    public void GameLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        FindPanels();
        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0;
        Debug.Log("游戏结束，弹出失败弹窗");
    }

    public void GameWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        FindPanels();
        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0;
        Debug.Log("通关成功，弹出胜利弹窗");
    }

    private GameObject FindAnyObjectByName(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform find = root.transform.Find(name);
                if (find != null)
                    return find.gameObject;
            }
        }
        return null;
    }
}
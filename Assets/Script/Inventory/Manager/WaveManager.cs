using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("通用设置")]
    public Transform player;
    public float waveDelay = 8f;
    public GameObject warningEffectPrefab;
    public float warningDelay = 0.5f;
    public GameObject guideArrowPrefab;

    private GameObject arrowObj;
    private LevelDataDetails currentLevelData;
    private int currentWave = 0;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    [HideInInspector] public bool waveHasDropped = false;
#pragma warning disable CS0414
    private bool isAllWaveDone;
#pragma warning restore CS0414

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartLevel(int levelID, LevelConfig_SO configSO)
    {
        if (configSO == null)
        {
            Debug.LogError("LevelConfig_SO 为空！");
            return;
        }

        currentLevelData = configSO.GetLevelDataByID(levelID);
        if (currentLevelData == null)
        {
            Debug.LogError($"找不到ID为 {levelID} 的关卡配置！");
            return;
        }

        if (player == null)
        {
            Debug.LogError("WaveManager里的player引用为空！");
            return;
        }

        ResetWaves();
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (currentWave < currentLevelData.totalWaves)
        {
            waveHasDropped = false;
            yield return StartCoroutine(SpawnWave());
            yield return new WaitUntil(() => aliveEnemies.Count <= 0);

            Debug.Log($"第 {currentWave} 波已清空");
            currentWave++;

            if (currentWave < currentLevelData.totalWaves)
                yield return new WaitForSeconds(waveDelay);
        }

        isAllWaveDone = true;
        Debug.Log("关卡通关！生成出口箭头");
        SpawnGuideArrow();

        //开启传送门
        if (X.Transition.Teleport.instance != null)
        {
            X.Transition.Teleport.instance.Show();
            Debug.Log("指令已发送：显示传送门");
        }
        else
        {
            Debug.LogError("找不到传送门，请检查场景里是否有Teleport脚本");
        }

        // 只有 Purple 关卡弹出通关窗口并暂停
        if (gameObject.scene.name == "Purple")
        {
            GameStateManager.Instance.GameWin();
        }
    }

    IEnumerator SpawnWave()
    {
        int count = currentLevelData.enemyCountPerWave[currentWave];

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetValidSpawnPos();

            if (warningEffectPrefab != null)
                Instantiate(warningEffectPrefab, pos, Quaternion.identity);

            yield return new WaitForSeconds(warningDelay);
            EventHandler.CallEnemySpawnEvent(currentLevelData.enemyType, pos);
            yield return new WaitForSeconds(0.2f);
        }
    }

    Vector3 GetValidSpawnPos()
    {
        Vector3 pos;
        int attempt = 0;

        do
        {
            Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(8, 14);
            pos = player.position + new Vector3(dir.x, dir.y, 0);
            attempt++;
        }
        while (
            attempt < 50 &&
            (pos.x < currentLevelData.mapMinX || pos.x > currentLevelData.mapMaxX ||
             pos.y < currentLevelData.mapMinY || pos.y > currentLevelData.mapMaxY ||
             !IsPositionSafe(pos))
        );

        return pos;
    }

    bool IsPositionSafe(Vector3 pos, float dist = 2.2f)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, dist, LayerMask.GetMask("Enemy"));
        return cols.Length == 0;
    }

    void SpawnGuideArrow()
    {
        if (guideArrowPrefab == null) return;
        if (arrowObj != null) Destroy(arrowObj);

        arrowObj = Instantiate(guideArrowPrefab);
        arrowObj.GetComponent<GuideArrow>().Init(player, currentLevelData.exitPosition);
    }

    public void AddEnemyToList(GameObject enemy)
    {
        if (!aliveEnemies.Contains(enemy))
            aliveEnemies.Add(enemy);
    }

    public void OnEnemyDead(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
            aliveEnemies.Remove(enemy);
    }

    public void ResetWaves()
    {
        StopAllCoroutines();
        currentWave = 0;
        aliveEnemies.Clear();
        isAllWaveDone = false;

        if (arrowObj != null)
        {
            Destroy(arrowObj);
            arrowObj = null;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    //子弹/特效池
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    //敌人池（合并在一起）
    public List<GameObject> enemyPrefabs;
    private List<ObjectPool<GameObject>> enemyPoolList = new List<ObjectPool<GameObject>>();

    //单例
    public static PoolManager Instance;

    public EnemyType enemyType;

    private GameObject lastSpawnedEnemy;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventHandler.BulletFireEvent += OnBulletFireEvent;
        EventHandler.EnemySpawnEvent += OnEnemySpawnEvent;       // 敌人生成
        EventHandler.EnemyBulletFireEvent += OnEnemyBulletFireEvent; // 敌人开枪
    }

    private void OnDisable()
    {
        EventHandler.BulletFireEvent -= OnBulletFireEvent;
        EventHandler.EnemySpawnEvent -= OnEnemySpawnEvent;
        EventHandler.EnemyBulletFireEvent -= OnEnemyBulletFireEvent;
    }

    private void Start()
    {
        CreatePool();       // 创建子弹池
        CreateEnemyPool();  // 创建敌人池
    }

    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            poolEffectList.Add(newPool);
        }
    }

    private void OnBulletFireEvent(ParticleEffectType effectType, Vector3 pos, Vector2 dir, int damage)
    {
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticleEffectType.Bullet => poolEffectList[0],
            _ => null
        };

        if (objPool == null) return;

        GameObject obj = objPool.Get();
        obj.transform.position = pos;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.SetDirection(dir);

        bullet.damage = damage;

        //自动设置为玩家子弹
        FindObjectOfType<PlayerShoot>().SetPlayerBullet(bullet);

        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(2f);

        //加空判断
        if (obj != null && pool != null)
        {
            pool.Release(obj);
        }
    }

    //敌人对象池

    private void CreateEnemyPool()
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            Transform parent = new GameObject(enemyPrefab.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                //实例化时强制用Prefab的原始设置
                () =>
                {
                    GameObject instance = Instantiate(enemyPrefab, parent);
                    //把Prefab的赋值同步到实例上
                    Enemy enemyScript = instance.GetComponent<Enemy>();
                    enemyScript.healthBarPrefab = enemyPrefab.GetComponent<Enemy>().healthBarPrefab;
                    return instance;
                },
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );

            enemyPoolList.Add(newPool);
        }
    }

    private void OnEnemySpawnEvent(EnemyType enemyType, Vector3 pos)
    {
        ObjectPool<GameObject> pool = null;

        switch (enemyType)
        {
            case EnemyType.BasicShooter:
                if (enemyPoolList.Count > 0) pool = enemyPoolList[0];
                break;
            case EnemyType.BigInsect:
                if (enemyPoolList.Count > 1) pool = enemyPoolList[1];
                break;
            case EnemyType.SmallInsect:
                if (enemyPoolList.Count > 2) pool = enemyPoolList[2];
                break;
        }

        if (pool == null) return;

        GameObject enemy = pool.Get();
        enemy.transform.position = pos;

        // 【关键：先Init，再ResetForPool】
        enemy.GetComponent<Enemy>().Init();
        enemy.GetComponent<Enemy>().ResetForPool();

        WaveManager.Instance.AddEnemyToList(enemy);
    }

    private void OnEnemyBulletFireEvent(ParticleEffectType effectType, Vector3 pos, Vector2 dir, int dmg)
    {
        ObjectPool<GameObject> objPool = null;

        // 根据子弹类型，取不同的池
        switch (effectType)
        {
            case ParticleEffectType.EnemyBullet:
                objPool = poolEffectList[1]; // 玩家/小敌人用的普通子弹
                break;
            case ParticleEffectType.InsectBullet:
                objPool = poolEffectList[2]; // 大敌人用的专属子弹
                break;
        }

        if (objPool == null) return;

        GameObject obj = objPool.Get();
        obj.transform.position = pos;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.SetDirection(dir);
        bullet.damage = dmg;
        bullet.isPlayerBullet = false; // 强制标记为敌人子弹，防止误伤玩家逻辑

        StartCoroutine(ReleaseRoutine(objPool, obj));
    }


    // 敌人回收（给Enemy脚本调用）

    public void ReleaseEnemy(GameObject enemy)
    {
        // 根据名字判断类型，回收到对应的池子里
        if (enemy.name.Contains("BigInsect"))
        {
            enemyPoolList[1].Release(enemy);
        }
        else if (enemy.name.Contains("SmallInsect"))
        {
            enemyPoolList[2].Release(enemy);
        }
        else
        {
            enemyPoolList[0].Release(enemy);
        }
    }

    public void Release()
    {
        if (PoolManager.Instance == null) return;

        if (enemyType == EnemyType.BasicShooter)
        {
            PoolManager.Instance.enemyPoolList[0].Release(gameObject);
        }
        else if (enemyType == EnemyType.BigInsect)
        {
            PoolManager.Instance.enemyPoolList[1].Release(gameObject);
        }
        else if (enemyType == EnemyType.SmallInsect)
        {
            PoolManager.Instance.enemyPoolList[2].Release(gameObject);
        }
    }



    //清空所有在场敌人

    public void ClearAllActiveEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>(includeInactive: false);
        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != null)
            {
                // 只隐藏血条
                if (enemy.healthBar != null)
                    enemy.healthBar.gameObject.SetActive(false);

                ReleaseEnemy(enemy.gameObject);
            }
        }
    }


    // 敌人池重置（关卡切换时调用）
    public void ClearEnemyPool()
    {
        foreach (var pool in enemyPoolList)
        {
            pool.Clear();
        }
        Debug.Log("敌人对象池已清空，准备下一关！");
    }


    // 加到 PoolManager 里任意位置
    public void ReleaseBullet(Bullet bullet)
    {
        if (bullet != null && bullet.gameObject != null)
        {
            poolEffectList[0].Release(bullet.gameObject);
        }
    }



    public void RegisterLastSpawnedEnemy(GameObject go)
    {
        lastSpawnedEnemy = go;
    }

    public GameObject GetLastSpawnedEnemy()
    {
        return lastSpawnedEnemy;
    }
}



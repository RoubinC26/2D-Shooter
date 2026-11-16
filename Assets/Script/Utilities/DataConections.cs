using System.Collections.Generic;
using UnityEngine;

// 标记此类为可序列化，以便在 Unity 编辑器中查看和编辑
[System.Serializable]
public class ItemDetails
{
    public string itemName;
    public Sprite itemImage;
    public int itemHeld;
    [TextArea]
    public string itemInfo;

    public bool equip;

    public ItemType itemType;       // 枚举
    public GameObject equipPrefab;  // 装备预制体（枪/帽子）

    public int effectValue; //效果数值
    public int bulletDamage; //武器伤害

    public ConsumableUseType useType;   // 使用类型
    public int grenadeDamage;           // 手雷伤害
    public float grenadeRadius;          // 爆炸范围
}


[System.Serializable]
public class WeaponDetails
{
    [Header("武器基础属性")]
    public WeaponOwnerType ownerType; // 区分玩家/敌人武器
    public float fireRate;
    public int damage;

    [Header("子弹设置")]
    public GameObject bulletPrefab;
    public Color bulletColor;
    public float bulletSpeed;

    [Header("特效")]
    public GameObject muzzleFlash;
}


[System.Serializable]
public class LevelDataDetails
{
    public int levelID;

    [Header("关卡设置")]
    public EnemyType enemyType;       // 这关生成的敌人类型
    public int totalWaves;             // 总波数
    public int[] enemyCountPerWave;    // 每波敌人数量

    [Header("地图边界")]
    public float mapMinX;
    public float mapMaxX;
    public float mapMinY;
    public float mapMaxY;

    [Header("出口设置")]
    public Vector3 exitPosition;
}

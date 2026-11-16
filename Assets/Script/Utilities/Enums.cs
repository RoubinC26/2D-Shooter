//拾取物品类型
public enum ItemType
{
    Weapon,   // 武器
    Hat,      // 帽子
    Bullet,
    Consumable// 消耗品（心、药水）
}

public enum ConsumableUseType
{
    Heal,    // 回血
    Grenade  // 手榴弹
}


//武器使用者类型（玩家/敌人共用）
public enum WeaponOwnerType
{
    Player,
    Enemy
}


// 子弹类型
public enum BulletType
{
    NormalBullet
}


//特效类型
public enum ParticleEffectType
{
    Bullet, // 把子弹加在这里
    EnemyBullet,
    InsectBullet
}


//敌人类型
public enum EnemyType
{
    BasicShooter,    // 第一关普通敌人
    BigInsect,      // 第二关大虫
    SmallInsect     // 第二关小虫
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    // 背包开关
    public static event Action ToggleBagUIEvent;
    public static void CallToggleBagUIEvent()
    {
        Debug.Log("EventHandler 收到了 ToggleBagUI 调用！");
        ToggleBagUIEvent?.Invoke();
    }


    // 物品拾取
    public static event Action<ItemDetails> ItemPickedEvent;
    public static void CallItemPickedEvent(ItemDetails item) => ItemPickedEvent?.Invoke(item);


    // 装备物品
    public static event Action<ItemDetails> EquipItemEvent;
    public static void CallEquipItemEvent(ItemDetails item) => EquipItemEvent?.Invoke(item);


    // 使用消耗品
    public static event Action<ItemDetails> UseConsumableEvent;
    public static void CallUseConsumableEvent(ItemDetails item) => UseConsumableEvent?.Invoke(item);


    public static event Action<ParticleEffectType, Vector3, Vector2, int> BulletFireEvent;
    public static void CallBulletFireEvent(ParticleEffectType type, Vector3 pos, Vector2 dir, int damage)
    {
        BulletFireEvent?.Invoke(type, pos, dir, damage);
    }


    // 敌人生成事件
    public static event Action<EnemyType, Vector3> EnemySpawnEvent;
    public static void CallEnemySpawnEvent(EnemyType enemyType, Vector3 pos)
    {
        EnemySpawnEvent?.Invoke(enemyType, pos);
    }


    public static event Action<ParticleEffectType, Vector3, Vector2, int> EnemyBulletFireEvent;
    public static void CallEnemyBulletFireEvent(ParticleEffectType effectType, Vector3 pos, Vector2 dir, int damage)
    {
        EnemyBulletFireEvent?.Invoke(effectType, pos, dir, damage);
    }


    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }


    public static event Action BeforeSceneOnloadEvent;
    public static void CallBeforeSceneOnloadEvent()
    {
        BeforeSceneOnloadEvent?.Invoke();
    }


    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }


    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }
}
# 2D 俯视角射击 ARPG Demo（Unity）

一个 Unity 2D 俯视角射击游戏：玩家操控角色移动、开枪、扔手雷，击杀一波波来袭的敌人（A* 寻路追击），掉落装备进背包、穿装备强化，清完波次进下一关。

> 面向秋招 U3D 客户端开发岗位的学习项目。

**演示视频 + 完整工程（含全部美术资源）：**
网盘链接：https://pan.baidu.com/s/16SkklQdll5fjz_PCeQUNhA

---

## 一、功能清单

| 模块 | 说明 |
|---|---|
| 玩家控制 | 移动 / 开枪 / 手雷（抛物线瞄准）/ 消耗品 |
| 敌人 AI | A* 寻路追击、毒区、分身分裂 |
| 关卡系统 | 分波次刷怪，清完进下一关（Green → Purple） |
| 背包 & 装备 | 打怪掉装备、拾取、背包管理、穿装备 |
| 对象池 | 子弹 / 敌人统一复用，减少 GC 和卡顿 |
| 转场 | 关卡传送、场景过渡动画 |

---

## 二、技术栈

- 引擎：Unity（2D，UGUI）
- 语言：C#
- 数据：ScriptableObject 配置武器 / 物品 / 关卡数据

---

## 三、技术点说明

| 技术点 | 一句话说明 |
|---|---|
| A* 寻路 | 敌人用网格 A* 寻路追玩家，不是傻乎乎直线走 |
| 对象池 | PoolManager 统一管子弹 / 敌人，频繁生成销毁改为复用 |
| ScriptableObject | 武器、物品、关卡数据用 SO 配置，策划 / 自己调参数方便 |
| 事件中心 | EventHandler 解耦 UI 和逻辑，界面不直接调游戏代码 |
| 单例基类 | Singleton 基类统一管全局管理器，避免到处写单例 |

---

## 四、如何运行

用 Unity Hub 打开本目录（`Assets` / `ProjectSettings` / `Packages` 都在根目录）→ 打开 `Assets/Scenes` 下的场景 → 点 ▶ Play。

> 需要装好 Unity（项目用的 2D 模板，自带 URP / TextMesh Pro 依赖已随 Packages 清单恢复）。

---

## 五、目录结构

```
├── Assets/
│   ├── Script/              # 全部 C# 代码
│   │   ├── Player/          # 玩家移动 / 射击 / 手雷 / 消耗品
│   │   ├── Enemy/           # 敌人 AI、毒区、分裂
│   │   ├── Gun/             # 枪械瞄准
│   │   ├── Bullet/          # 子弹
│   │   ├── Inventory/       # 背包 / 装备 / 物品 / 波次管理
│   │   ├── Pool/            # 对象池
│   │   ├── Item/            # 掉落物 / 拾取
│   │   └── Utilities/       # 框架工具（单例 / 事件中心 / A* / 枚举）
│   ├── Scenes/              # Green / Purple / UI / PersistentScene
│   ├── Prefabs/             # 子弹 / 敌人 / 装备等预制体
│   └── ...
├── ProjectSettings/
├── Packages/
└── README.md
```

---

## 六、关于资源

本项目的美术资源（角色 / 怪物 / UI 图集 / 字体等）来自第三方资源包，体积较大，未上传到本仓库，已通过 `.gitignore` 排除（`Assets/Art Assets`、`Assets/Fonts`、`Assets/TextMesh Pro/Examples & Extras` 等）。

- **完整工程（含全部资源 + 演示视频）**：见顶部网盘链接
- **本仓库聚焦**：玩家 / 敌人 / 背包 / 关卡 / 对象池等逻辑代码

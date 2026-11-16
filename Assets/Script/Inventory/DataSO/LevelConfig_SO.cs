using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Config", menuName = "Level/Config")]
public class LevelConfig_SO : ScriptableObject
{
    public List<LevelDataDetails> levelDataList;

    // 根据关卡ID 获取对应数据
    public LevelDataDetails GetLevelDataByID(int levelID)
    {
        foreach (var data in levelDataList)
        {
            if (data.levelID == levelID)
                return data;
        }

        Debug.LogError("找不到关卡ID：" + levelID);
        return null;
    }



}
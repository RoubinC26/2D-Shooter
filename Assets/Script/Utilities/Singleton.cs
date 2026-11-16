using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 静态实例
    private static T _instance;

    // 实例属性
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                // 如果场景中没有实例，则输出错误信息
                if (_instance == null)
                {
                    Debug.LogError($"Singleton of type {typeof(T)} not found in the scene.");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        //UI物体不做DontDestroyOnLoad
        if (GetComponent<RectTransform>() == null)
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
    }
}
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    void Awake()
    {
        //给根物体调用DontDestroyOnLoad
        DontDestroyOnLoad(gameObject);
    }
}
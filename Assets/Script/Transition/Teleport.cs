using UnityEngine;

namespace X.Transition
{
    public class Teleport : MonoBehaviour
    {
        public string sceneToGo;
        public Vector3 positionToGo;

        // 静态引用
        public static Teleport instance;

        private void Awake()
        {
            instance = this;
            // 开局隐藏
            gameObject.SetActive(false);
        }

        // 公开显示方法
        public void Show()
        {
            gameObject.SetActive(true);
            Debug.Log("传送门已开启！");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}
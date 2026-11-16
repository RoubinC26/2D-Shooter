using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI hpText;
    private Transform enemy;
    private Camera mainCam;
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    public void SetEnemy(Transform target)
    {
        enemy = target;
        gameObject.SetActive(true);
    }

    public void UpdateHp(float current, float max)
    {
        if (fill != null)
            fill.fillAmount = current / max;

        if (hpText != null)
            hpText.text = $"{Mathf.Round(current)}/{Mathf.Round(max)}";
    }

    void LateUpdate()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 wpos = enemy.position + new Vector3(0, 0.8f, 0);
        Vector3 spos = mainCam.WorldToScreenPoint(wpos);
        rt.position = spos;
    }
}







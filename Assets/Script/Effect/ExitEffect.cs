using UnityEngine;

public class ExitEffect : MonoBehaviour
{
    public float flashSpeed = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sr != null)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
            sr.color = new Color(1, 1, 1, 0.5f + alpha * 0.5f);
        }
    }
}
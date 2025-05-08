using UnityEngine;

public class DashAfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    public float fadeSpeed = 5f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Color c = sr.color;
        c.a -= fadeSpeed * Time.deltaTime;  // Only change the alpha
        sr.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}

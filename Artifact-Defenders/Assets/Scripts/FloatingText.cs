using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public float moveUpSpeed = 30f;
    public float fadeSpeed = 1f;
    private Text text;
    private Color originalColor;

    void Start()
    {
        text = GetComponent<Text>();
        originalColor = text.color;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                               text.color.a - fadeSpeed * Time.deltaTime);
    }
}

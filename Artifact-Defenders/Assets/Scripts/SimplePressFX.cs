using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimplePressFX : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rt.anchoredPosition += new Vector2(0, -2);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rt.anchoredPosition += new Vector2(0, 2);
    }
}

using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class StageLinkUI : MonoBehaviour
{
    public RectTransform from;
    public RectTransform to;
    public Image img;

    public float thickness = 6f;
    public float endPadding = 18f;

    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.25f);

    RectTransform rt;

    void OnEnable() => rt = (RectTransform)transform;

    void Update()
    {
        if (!from || !to || !rt) return;

        Vector2 a = from.anchoredPosition;
        Vector2 b = to.anchoredPosition;

        Vector2 dir = b - a;
        float dist = dir.magnitude;
        if (dist < 0.01f) return;

        Vector2 n = dir / dist;
        a += n * endPadding;
        b -= n * endPadding;

        Vector2 mid = (a + b) * 0.5f;
        Vector2 d2 = (b - a);
        float len = d2.magnitude;

        rt.anchoredPosition = mid;
        rt.sizeDelta = new Vector2(len, thickness);

        float ang = Mathf.Atan2(d2.y, d2.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, ang);
    }

    public void SetActive(bool on)
    {
        if (img) img.color = on ? activeColor : inactiveColor;
    }
}
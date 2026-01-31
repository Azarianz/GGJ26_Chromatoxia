using TMPro;
using UnityEngine;

public class TMP_NeonPulseText : MonoBehaviour
{
    public TMP_Text text;

    [Header("Animation")]
    public float speed = 1.5f;
    public float charPhaseOffset = 0.15f;

    [Header("Neon Colors")]
    public Color neonRed = new Color(1f, 0.1f, 0.1f);
    public Color neonPurple = new Color(0.8f, 0.2f, 1f);
    public Color neonBlue = new Color(0.2f, 0.6f, 1f);

    void Reset() => text = GetComponent<TMP_Text>();

    void Update()
    {
        if (!text) return;

        text.ForceMeshUpdate();
        var ti = text.textInfo;

        float t = Time.unscaledTime * speed;

        for (int i = 0; i < ti.characterCount; i++)
        {
            var c = ti.characterInfo[i];
            if (!c.isVisible) continue;

            float phase = Mathf.Repeat(t + i * charPhaseOffset, 1f);

            Color col;
            if (phase < 0.33f)
                col = Color.Lerp(neonRed, neonPurple, phase / 0.33f);
            else if (phase < 0.66f)
                col = Color.Lerp(neonPurple, neonBlue, (phase - 0.33f) / 0.33f);
            else
                col = Color.Lerp(neonBlue, neonRed, (phase - 0.66f) / 0.34f);

            int matIndex = c.materialReferenceIndex;
            int vertIndex = c.vertexIndex;
            var colors = ti.meshInfo[matIndex].colors32;
            Color32 c32 = col;

            colors[vertIndex + 0] = c32;
            colors[vertIndex + 1] = c32;
            colors[vertIndex + 2] = c32;
            colors[vertIndex + 3] = c32;
        }

        for (int m = 0; m < ti.meshInfo.Length; m++)
        {
            ti.meshInfo[m].mesh.colors32 = ti.meshInfo[m].colors32;
            text.UpdateGeometry(ti.meshInfo[m].mesh, m);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortraitDamageFX : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image portrait;

    [Tooltip("Assign a child RectTransform to shake (recommended). If empty, shakes the portrait itself.")]
    [SerializeField] private RectTransform shakeTarget;

    [Header("Flash")]
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private int flashBlinks = 2;
    [Range(0f, 1f)]
    [SerializeField] private float redStrength = 0.85f;

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakeAmplitude = 10f; // UI pixels
    [SerializeField] private int shakeFrequency = 30;

    Coroutine fxRoutine;
    Color baseColor;

    void Awake()
    {
        if (!portrait) portrait = GetComponent<Image>();
        if (portrait) baseColor = portrait.color;

        if (!shakeTarget && portrait) shakeTarget = portrait.rectTransform;

        // IMPORTANT: If you follow the recommended hierarchy, set shakeTarget in Inspector
        // to the child "PortraitShake" so we never fight layout.
    }

    void OnDisable()
    {
        ResetFX();
    }

    public void Play()
    {
        if (!portrait || !shakeTarget) return;

        if (fxRoutine != null) StopCoroutine(fxRoutine);
        fxRoutine = StartCoroutine(Fx());
    }

    IEnumerator Fx()
    {
        // Re-capture base color each hit (in case something else tinted it)
        baseColor = portrait.color;

        float totalFlash = flashDuration * Mathf.Max(1, flashBlinks);
        float total = Mathf.Max(totalFlash, shakeDuration);

        float elapsed = 0f;
        float shakeStep = (shakeFrequency <= 0) ? 0.033f : (1f / shakeFrequency);
        float shakeTimer = 0f;

        // Always shake as an offset from ZERO (so it can’t drift)
        shakeTarget.anchoredPosition = Vector2.zero;

        try
        {
            while (elapsed < total)
            {
                float dt = Time.unscaledDeltaTime;
                elapsed += dt;

                // ---- FLASH ----
                if (elapsed < totalFlash)
                {
                    float blinkTime = elapsed / flashDuration;
                    float phase = blinkTime - Mathf.Floor(blinkTime); // 0..1
                    float pingpong = 1f - Mathf.Abs(phase * 2f - 1f); // 0..1..0

                    Color red = new Color(1f, 1f - redStrength, 1f - redStrength, 1f);
                    portrait.color = Color.Lerp(Color.white, red, pingpong);
                }
                else
                {
                    portrait.color = Color.white;
                }

                // ---- SHAKE ----
                if (elapsed < shakeDuration)
                {
                    shakeTimer += dt;
                    if (shakeTimer >= shakeStep)
                    {
                        shakeTimer = 0f;
                        float angle = Random.value * Mathf.PI * 2f;
                        float radius = Random.value * shakeAmplitude;
                        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

                        // offset from zero
                        shakeTarget.anchoredPosition = offset;
                    }
                }
                else
                {
                    shakeTarget.anchoredPosition = Vector2.zero;
                }

                yield return null;
            }
        }
        finally
        {
            ResetFX();
            fxRoutine = null;
        }
    }

    void ResetFX()
    {
        if (portrait)
            portrait.color = Color.white;   // HARD reset to white

        if (shakeTarget)
            shakeTarget.anchoredPosition = Vector2.zero;
    }

}

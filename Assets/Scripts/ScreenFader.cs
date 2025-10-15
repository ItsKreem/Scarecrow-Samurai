using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeOutDuration = 1.5f; // seconds for fading to black
    public float fadeInDuration = 0.8f;  // seconds for fading back in

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // optional, keeps fader across scenes
    }

    /// <summary>
    /// Fades the screen from transparent to black.
    /// </summary>
    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f, fadeOutDuration);
    }

    /// <summary>
    /// Fades the screen from black to transparent.
    /// </summary>
    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f, fadeInDuration);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadeImage == null)
        {
            Debug.LogWarning("ScreenFader: fadeImage not assigned!");
            yield break;
        }

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}
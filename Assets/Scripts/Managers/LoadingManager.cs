using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class LoadingManager : MonoBehaviour
{
    [Tooltip("Slider UI element representing load progress (0 to 1).")]
    public Slider progressBar;

    [Tooltip("Optional text field showing percentage progress.")]
    public TMP_Text progressText;

    [Tooltip("Optional CanvasGroup for fade-out effect. Ensure this is attached to the prefab.")]
    public CanvasGroup canvasGroup;

    /// <summary>
    /// Updates the progress indicator.
    /// </summary>
    public void SetProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{(progress * 100f):F0}%";
        }
    }

    /// <summary>
    /// Waits a few seconds to show 100%, fades out the UI, then deactivates the GameObject.
    /// </summary>
    public async UniTask Hide()
    {
        const float waitTimeSeconds = 1f;
        await UniTask.Delay(TimeSpan.FromSeconds(waitTimeSeconds));

        if (canvasGroup != null)
        {
            const float fadeDuration = 1f;
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                await UniTask.Yield();
            }
            canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
    }
}
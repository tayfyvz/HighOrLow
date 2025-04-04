using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class LoadingManager : MonoBehaviour
    {
        public Slider progressBar;
        public TMP_Text progressText;
        public CanvasGroup canvasGroup;

        public void SetProgress(float progress)
        {
            if (progressBar != null) progressBar.value = progress;
            if (progressText != null) progressText.text = $"{(progress * 100f):F0}%";
        }

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
}
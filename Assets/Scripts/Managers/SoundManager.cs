using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Logger = Utils.Logger;

namespace Managers.Managers
{
    public enum SoundType
    {
        ButtonClick,
        DrawCard,
        RoundEnd,
        BetWin,
        BetLose,
        GameEnd
    }
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private readonly Dictionary<SoundType, string> soundKeyMapping = new Dictionary<SoundType, string>
        {
            { SoundType.ButtonClick, "ButtonClickSound" },
            { SoundType.DrawCard, "DrawCardSound" },
            { SoundType.RoundEnd, "RoundEndSound" },
            { SoundType.BetWin, "BetWinSound" },
            { SoundType.BetLose, "BetLoseSound" },
            { SoundType.GameEnd, "GameEndSound" }
        };

        private readonly Dictionary<SoundType, AudioClip> soundCache = new Dictionary<SoundType, AudioClip>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async UniTask<AudioClip> LoadSoundAsync(SoundType type)
        {
            if (soundCache.ContainsKey(type)) return soundCache[type];

            if (!soundKeyMapping.TryGetValue(type, out string key))
            {
                Logger.Log($"SoundType {type} is not mapped to any sound key.", LogType.Error);
                return null;
            }

            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = handle.Result;
                soundCache.Add(type, clip);
                return clip;
            }

            Logger.Log($"Failed to load sound for key: {key}", LogType.Error);
            return null;
        }

        private async UniTask PlaySoundAsync(SoundType type)
        {
            AudioClip clip = await LoadSoundAsync(type);
            if (clip != null) AudioManager.Instance.PlaySfx(clip);
        }

        public void PlaySound(SoundType type)
        {
            if (soundCache.ContainsKey(type))
            {
                AudioManager.Instance.PlaySfx(soundCache[type]);
            }
            else
            {
                PlaySoundAsync(type).Forget();
            }
        }
    }
}

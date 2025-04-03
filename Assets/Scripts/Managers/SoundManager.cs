using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Managers.Managers
{
    public enum SoundType
    {
        ButtonClick,
        RoundEnd,
        BetWin,
        BetLose,
        GameEnd
    }
    
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        // Map each SoundType to its Addressables key.
        private readonly Dictionary<SoundType, string> soundKeyMapping = new Dictionary<SoundType, string>()
        {
            { SoundType.ButtonClick, "ButtonClickSound" },
            { SoundType.RoundEnd, "RoundEndSound" },
            { SoundType.BetWin, "BetWinSound" },
            { SoundType.BetLose, "BetLoseSound" },
            { SoundType.GameEnd, "GameEndSound" }
        };

        // Cache for loaded AudioClips.
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

        /// <summary>
        /// Asynchronously loads the AudioClip associated with the given SoundType from Addressables,
        /// caching it for future use.
        /// </summary>
        public async UniTask<AudioClip> LoadSoundAsync(SoundType type)
        {
            if (soundCache.ContainsKey(type))
            {
                return soundCache[type];
            }

            if (!soundKeyMapping.TryGetValue(type, out string key))
            {
                Debug.LogError($"SoundType {type} is not mapped to any sound key.");
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
            else
            {
                Debug.LogError($"Failed to load sound for key: {key}");
                return null;
            }
        }

        /// <summary>
        /// Asynchronously plays the sound effect corresponding to the provided SoundType.
        /// </summary>
        public async UniTask PlaySoundAsync(SoundType type)
        {
            AudioClip clip = await LoadSoundAsync(type);
            if (clip != null)
            {
                // Use AudioManager's dedicated SFX AudioSource for playing sound effects.
                AudioManager.Instance.PlaySfx(clip);
            }
        }

        /// <summary>
        /// Plays the sound immediately. If the AudioClip is not cached yet, it will be loaded asynchronously.
        /// </summary>
        public void PlaySound(SoundType type)
        {
            if (soundCache.ContainsKey(type))
            {
                AudioManager.Instance.PlaySfx(soundCache[type]);
            }
            else
            {
                // Fire and forget asynchronous loading if needed.
                PlaySoundAsync(type).Forget();
            }
        }
    }
}
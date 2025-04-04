using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Logger = Utils.Logger;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        public AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                if (sfxSource == null)
                {
                    sfxSource = gameObject.AddComponent<AudioSource>();
                    sfxSource.loop = false;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async UniTask LoadBackgroundMusicAsync()
        {
            string musicKey = "BackgroundMusic";
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(musicKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = handle.Result;
                if (clip == null)
                {
                    Logger.Log("Loaded AudioClip is null.", LogType.Error);
                    return;
                }

                if (musicSource == null)
                {
                    musicSource = gameObject.AddComponent<AudioSource>();
                    musicSource.loop = true;
                }

                musicSource.clip = clip;
                Logger.Log($"Playing background music: {clip.name}", LogType.Log);
                musicSource.Play();
            }
            else
            {
                Logger.Log("Failed to load background music.", LogType.Error);
            }

            Addressables.Release(handle);
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null)
            {
                Logger.Log("SFX clip is null.", LogType.Warning);
                return;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.PlayOneShot(clip);
        }

        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        public void ResumeMusic()
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
}

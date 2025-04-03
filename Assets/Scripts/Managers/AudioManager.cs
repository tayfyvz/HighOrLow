using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        // AudioSource for background music.
        public AudioSource musicSource;
        
        // AudioSource for sound effects. You can assign this via the Inspector,
        // or if it’s null, we create one at runtime.
        [SerializeField] private AudioSource sfxSource;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Ensure we have a SFX AudioSource.
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

        /// <summary>
        /// Loads and plays background music from Addressables.
        /// </summary>
        public async UniTask LoadBackgroundMusicAsync()
        {
            string musicKey = "BackgroundMusic"; // Use your Addressable key.
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(musicKey);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AudioClip clip = handle.Result;
                if (clip == null)
                {
                    Debug.LogError("Loaded AudioClip is null.");
                    return;
                }

                if (musicSource == null)
                {
                    musicSource = gameObject.AddComponent<AudioSource>();
                    musicSource.loop = true;
                }

                musicSource.clip = clip;
                Debug.Log("Playing background music: " + clip.name);
                musicSource.Play();
            }
            else
            {
                Debug.LogError("Failed to load background music.");
            }

            Addressables.Release(handle);
        }

        /// <summary>
        /// Plays a sound effect via the SFX AudioSource.  
        /// Using PlayOneShot prevents interrupting any audio already playing.
        /// </summary>
        /// <param name="clip">The AudioClip to play as a sound effect.</param>
        public void PlaySfx(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("SFX clip is null.");
                return;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.PlayOneShot(clip);
        }
    }
}

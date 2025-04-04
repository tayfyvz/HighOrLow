using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Utils;
using Logger = Utils.Logger;

namespace Managers
{
    public class GameInitializer : MonoBehaviour
    {
        public static GameInitializer Instance { get; private set; }

        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private bool enableLogging = true;
        
        private async void Start()
        {
            Screen.SetResolution(1920, 1080, true);
            Logger.EnableLogging(enableLogging);
            await InitializeGameAsync();
        }

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
        
        private void OnValidate()
        {
            Logger.EnableLogging(enableLogging);
        }

        private async UniTask InitializeGameAsync()
        {
            await TransitionSceneAsync(Constants.SCENE_LOBBY, Constants.ADDRESS_LOADING_PREFAB);
        }

        public async UniTaskVoid LoadLobbySceneWithLoadingAsync()
        {
            await TransitionSceneAsync(Constants.SCENE_LOBBY, Constants.ADDRESS_LOADING_PREFAB);
        }

        public async UniTaskVoid LoadGameSceneWithLoadingAsync()
        {
            await TransitionSceneAsync(Constants.SCENE_GAME, Constants.ADDRESS_LOADING_PREFAB);
        }

        private async UniTask TransitionSceneAsync(string targetSceneName, string loadingPrefabKey)
        {
            if (targetCanvas == null)
            {
                Logger.Log("Target Canvas is not assigned. Please ensure a Canvas is set in the Inspector.", LogType.Error);
                return;
            }

            Logger.Log($"Loading {loadingPrefabKey} from Addressables...");
            AsyncOperationHandle<GameObject> loadingHandle = Addressables.LoadAssetAsync<GameObject>(loadingPrefabKey);
            await loadingHandle.Task;

            if (loadingHandle.Status != AsyncOperationStatus.Succeeded || loadingHandle.Result == null)
            {
                Logger.Log($"Failed to load {loadingPrefabKey} from Addressables.", LogType.Error);
                return;
            }

            GameObject loadingPrefab = loadingHandle.Result;
            GameObject loadingInstance = Instantiate(loadingPrefab, targetCanvas.transform);
            Addressables.Release(loadingHandle);

            LoadingManager loadingUI = loadingInstance.GetComponent<LoadingManager>();
            if (loadingUI == null)
            {
                Logger.Log("LoadingManager component not found on the instantiated prefab.", LogType.Error);
                return;
            }

            float totalProgress = 0f;
            loadingUI.SetProgress(totalProgress);

            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Main")
            {
                Logger.Log("Main scene is persistent and will not be unloaded.");
            }
            else
            {
                Logger.Log($"Unloading current scene: {currentScene.name}...");
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
                while (!unloadOp.isDone)
                {
                    await UniTask.Yield();
                }

                totalProgress += Constants.UI_LOADING_WEIGHT;
                loadingUI.SetProgress(totalProgress);
            }

            Logger.Log($"Loading UI Atlas for {targetSceneName} scene...");
            await UIManager.Instance.UIAtlasLoader.LoadAsync();
            totalProgress += Constants.UI_LOADING_WEIGHT;
            loadingUI.SetProgress(totalProgress);

            Logger.Log($"Loading Background Music for {targetSceneName} scene...");
            await AudioManager.Instance.LoadBackgroundMusicAsync();
            totalProgress += Constants.AUDIO_LOADING_WEIGHT;
            loadingUI.SetProgress(totalProgress);

            Logger.Log($"Loading {targetSceneName} scene additively...");
            AsyncOperation targetLoadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            targetLoadOp.allowSceneActivation = false;

            while (targetLoadOp.progress < 0.9f)
            {
                float sceneProgress = Mathf.Clamp01(targetLoadOp.progress / 0.9f);
                float combinedProgress = totalProgress + sceneProgress * Constants.SCENE_LOADING_WEIGHT;
                loadingUI.SetProgress(combinedProgress);
                await UniTask.Yield();
            }

            Logger.Log($"{targetSceneName} scene fully loaded in the background. Waiting for Loading UI to hide...");
            await loadingUI.Hide();
            Logger.Log("Loading UI hidden. Activating scene...");

            targetLoadOp.allowSceneActivation = true;
            while (!targetLoadOp.isDone)
            {
                await UniTask.Yield();
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                SceneManager.SetActiveScene(targetScene);
                Logger.Log($"{targetSceneName} scene activated.");
            }
        }
    }
}

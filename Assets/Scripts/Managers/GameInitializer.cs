using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameInitializer : MonoBehaviour
    {
        public static GameInitializer Instance { get; private set; }

        [SerializeField]
        private Canvas targetCanvas;

        private const float uiLoadingWeight = 0.30f;
        private const float audioLoadingWeight = 0.30f;
        private const float sceneLoadingWeight = 0.40f;

        private async void Start()
        {
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

        private async UniTask InitializeGameAsync()
        {
            await TransitionSceneAsync("Lobby", "LoadingPrefab");
        }

        public async UniTaskVoid LoadLobbySceneWithLoadingAsync()
        {
            await TransitionSceneAsync("Lobby", "LoadingPrefab");
        }

        public async UniTaskVoid LoadGameSceneWithLoadingAsync()
        {
            await TransitionSceneAsync("Game", "LoadingPrefab");
        }

        private async UniTask TransitionSceneAsync(string targetSceneName, string loadingPrefabKey)
        {
            if (targetCanvas == null)
            {
                Debug.LogError("Target Canvas is not assigned. Please ensure a Canvas is set in the Inspector.");
                return;
            }

            Debug.Log($"Loading {loadingPrefabKey} from Addressables...");
            AsyncOperationHandle<GameObject> loadingHandle = Addressables.LoadAssetAsync<GameObject>(loadingPrefabKey);
            await loadingHandle.Task;

            if (loadingHandle.Status != AsyncOperationStatus.Succeeded || loadingHandle.Result == null)
            {
                Debug.LogError($"Failed to load {loadingPrefabKey} from Addressables.");
                return;
            }

            GameObject loadingPrefab = loadingHandle.Result;
            GameObject loadingInstance = Instantiate(loadingPrefab, targetCanvas.transform);
            Addressables.Release(loadingHandle);

            LoadingManager loadingUI = loadingInstance.GetComponent<LoadingManager>();
            if (loadingUI == null)
            {
                Debug.LogError("LoadingManager component not found on the instantiated prefab.");
                return;
            }

            float totalProgress = 0f;
            loadingUI.SetProgress(totalProgress);

            Scene currentScene = SceneManager.GetActiveScene();
            
            if (currentScene.name == "Main")
            {
                Debug.Log("Main scene is persistent and will not be unloaded.");
            }
            else
            {
                Debug.Log($"Unloading current scene: {currentScene.name}...");
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
                while (!unloadOp.isDone)
                {
                    await UniTask.Yield();
                }
                totalProgress += uiLoadingWeight;
                loadingUI.SetProgress(totalProgress);
            }

            Debug.Log($"Loading UI Atlas for {targetSceneName} scene...");
            await UIManager.Instance.UIAtlasLoader.LoadAsync();
            totalProgress += uiLoadingWeight;
            loadingUI.SetProgress(totalProgress);

            Debug.Log($"Loading Background Music for {targetSceneName} scene...");
            await AudioManager.Instance.LoadBackgroundMusicAsync();
            totalProgress += audioLoadingWeight;
            loadingUI.SetProgress(totalProgress);

            Debug.Log($"Loading {targetSceneName} scene additively...");
            AsyncOperation targetLoadOp = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            targetLoadOp.allowSceneActivation = false; // Delay scene activation.

            while (targetLoadOp.progress < 0.9f)
            {
                float sceneProgress = Mathf.Clamp01(targetLoadOp.progress / 0.9f);
                float combinedProgress = totalProgress + sceneProgress * sceneLoadingWeight;
                loadingUI.SetProgress(combinedProgress);
                await UniTask.Yield();
            }

            Debug.Log($"{targetSceneName} scene fully loaded in the background. Waiting for Loading UI to hide...");
            await loadingUI.Hide();
            Debug.Log("Loading UI hidden. Activating scene...");

            targetLoadOp.allowSceneActivation = true;
            while (!targetLoadOp.isDone)
            {
                await UniTask.Yield();
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                SceneManager.SetActiveScene(targetScene);
                Debug.Log($"{targetSceneName} scene activated.");
            }
        }
    }
}

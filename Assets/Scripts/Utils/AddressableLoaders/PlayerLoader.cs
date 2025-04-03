using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Views;

namespace Utils.AddressableLoaders
{
    public class PlayerLoader : BaseAddressableLoader<PlayerView>
    {
        private const string AssetAddress = "PlayerPrefabAddress";

        public override async UniTask<PlayerView> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle = Addressables.InstantiateAsync(AssetAddress);
            handle = gameObjHandle;

            GameObject instance = await gameObjHandle.Task;

            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate player prefab at address: {AssetAddress}");
                return null;
            }

            PlayerView playerView = instance.GetComponent<PlayerView>();

            if (playerView == null)
            {
                Debug.LogError("Player prefab is missing the PlayerView component.");
            }

            return playerView;
        }

        public override void Release()
        {
            // Check if the handle is valid
            if (handle.IsValid())
            {
                // Check if the result is a valid GameObject before releasing it
                if (handle.Result != null)
                {
                    Addressables.ReleaseInstance((GameObject)handle.Result);
                    Debug.Log("Player prefab instance released successfully.");
                }
                else
                {
                    Debug.LogWarning("Attempting to release a null Addressable instance.");
                }
            }
            else
            {
                Debug.LogWarning("Attempting to release an invalid Addressable handle.");
            }
        }
    }
}
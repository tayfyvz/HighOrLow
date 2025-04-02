using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Views;

namespace Utils
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
            if (handle.IsValid())
            {
                Addressables.ReleaseInstance((GameObject)handle.Result);
            }
        }
    }
}
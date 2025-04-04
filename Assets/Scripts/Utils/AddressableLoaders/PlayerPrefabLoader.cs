using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Views;

namespace Utils.AddressableLoaders
{
    public class PlayerPrefabLoader : BaseAddressableLoader<PlayerView>
    {
        public override async UniTask<PlayerView> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle =
                Addressables.InstantiateAsync(Constants.PLAYER_PREFAB_ADDRESS);
            handle = gameObjHandle;

            GameObject instance = await gameObjHandle.Task;

            if (instance == null)
            {
                Logger.Log($"Failed to instantiate player prefab at address: {Constants.PLAYER_PREFAB_ADDRESS}",
                    LogType.Error);
                return null;
            }

            PlayerView playerView = instance.GetComponent<PlayerView>();

            if (playerView == null)
            {
                Logger.Log("Player prefab is missing the PlayerView component.", LogType.Error);
            }

            return playerView;
        }

        public override void Release()
        {
            if (handle.IsValid())
            {
                if (handle.Result != null)
                {
                    Addressables.ReleaseInstance((GameObject)handle.Result);
                    Logger.Log("Player prefab instance released successfully.");
                }
                else
                {
                    Logger.Log("Attempting to release a null Addressable instance.", LogType.Warning);
                }
            }
            else
            {
                Logger.Log("Attempting to release an invalid Addressable handle.", LogType.Warning);
            }
        }
    }
}
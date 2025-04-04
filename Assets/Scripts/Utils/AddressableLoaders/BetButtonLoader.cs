using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Utils.AddressableLoaders
{
    public class BetButtonLoader : BaseAddressableLoader<Button>
    {
        public override async UniTask<Button> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle =
                Addressables.InstantiateAsync(Constants.BET_BUTTON_PREFAB_ADDRESS);
            handle = gameObjHandle;

            GameObject instance = await gameObjHandle.Task;
            if (instance == null)
            {
                Logger.Log($"Failed to instantiate bet button prefab at address: {Constants.BET_BUTTON_PREFAB_ADDRESS}",
                    LogType.Error);
                return null;
            }

            Button betButton = instance.GetComponent<Button>();
            if (betButton == null)
            {
                Logger.Log("The instantiated bet button prefab is missing a Button component.", LogType.Error);
            }

            return betButton;
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
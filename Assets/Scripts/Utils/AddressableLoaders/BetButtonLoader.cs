using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Views;

namespace Utils.AddressableLoaders
{
    public class BetButtonLoader : BaseAddressableLoader<BetButtonView>
    {
        public override async UniTask<BetButtonView> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle =
                Addressables.InstantiateAsync(Constants.BET_BUTTON_PREFAB_ADDRESS);
            handle = gameObjHandle;

            GameObject instance = await gameObjHandle.Task;
            if (instance == null)
            {
                Logger.Log($"Failed to instantiate bet button view prefab at address: {Constants.BET_BUTTON_PREFAB_ADDRESS}",
                    LogType.Error);
                return null;
            }

            BetButtonView betButton = instance.GetComponent<BetButtonView>();
            if (betButton == null)
            {
                Logger.Log("The instantiated bet button view prefab is missing a BetButtonView component.", LogType.Error);
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
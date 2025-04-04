using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Views;

namespace Utils.AddressableLoaders
{
    public class CardPrefabLoader : BaseAddressableLoader<CardView>
    {
        public override async UniTask<CardView> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle =
                Addressables.InstantiateAsync(Constants.CARD_PREFAB_ADDRESS);
            handle = gameObjHandle;

            GameObject instance = await gameObjHandle.Task;

            if (instance == null)
            {
                Logger.Log($"Failed to instantiate card prefab at address: {Constants.CARD_PREFAB_ADDRESS}",
                    LogType.Error);
                return null;
            }

            CardView cardView = instance.GetComponent<CardView>();

            if (cardView == null)
            {
                Logger.Log("Card prefab is missing the CardView component.", LogType.Error);
            }

            return cardView;
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
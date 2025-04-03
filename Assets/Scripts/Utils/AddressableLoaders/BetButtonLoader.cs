using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Utils.AddressableLoaders
{
    public class BetButtonLoader : BaseAddressableLoader<Button>
    {
        private const string AssetAddress = "BetButtonPrefabAddress";

        public override async UniTask<Button> LoadAsync()
        {
            AsyncOperationHandle<GameObject> gameObjHandle = Addressables.InstantiateAsync(AssetAddress);
            handle = gameObjHandle;
            
            GameObject instance = await gameObjHandle.Task;
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate bet button prefab at address: {AssetAddress}");
                return null;
            }
            
            Button betButton = instance.GetComponent<Button>();
            if (betButton == null)
            {
                Debug.LogError("The instantiated bet button prefab is missing a Button component.");
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
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils.AddressableLoaders.Utils
{
    public class UIAtlasLoader : BaseAddressableLoader<SpriteAtlas>
    {
        private const string AssetAddress = "UIAtlas";
    
        private SpriteAtlas uiAtlas;
    
        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas> atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(AssetAddress);
            handle = atlasHandle;
            uiAtlas = await atlasHandle.Task;
    
            if (uiAtlas == null)
            {
                Debug.LogError($"Failed to load UI Atlas at address: {AssetAddress}");
            }
            else
            {
                Debug.Log("UI Atlas loaded successfully!");
            }
            return uiAtlas;
        }
    
        public override void Release()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    
        public Sprite GetSprite(string spriteName)
        {
            if (uiAtlas == null)
            {
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }
    
            Sprite sprite = uiAtlas.GetSprite(spriteName);
            if (sprite == null)
            {
                Debug.LogWarning($"Sprite not found: {spriteName}");
            }
            return sprite;
        }
    }
}
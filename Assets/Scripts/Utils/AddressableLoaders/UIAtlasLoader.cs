using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils.AddressableLoaders
{
    public class UIAtlasLoader : BaseAddressableLoader<SpriteAtlas>
    {
        private SpriteAtlas _uiAtlas;

        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas>
                atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(Constants.UI_ATLAS);
            handle = atlasHandle;
            _uiAtlas = await atlasHandle.Task;

            if (_uiAtlas == null)
            {
                Logger.Log($"Failed to load UI Atlas at address: {Constants.UI_ATLAS}", LogType.Error);
            }
            else
            {
                Logger.Log("UI Atlas loaded successfully!");
            }

            return _uiAtlas;
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
            if (_uiAtlas == null)
            {
                Logger.Log("Atlas is not loaded yet.", LogType.Warning);
                return null;
            }

            Sprite sprite = _uiAtlas.GetSprite(spriteName);
            if (sprite == null)
            {
                Logger.Log($"Sprite not found: {spriteName}", LogType.Warning);
            }

            return sprite;
        }
    }
}
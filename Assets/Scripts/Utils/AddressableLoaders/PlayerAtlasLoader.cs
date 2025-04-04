using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils.AddressableLoaders
{
    public class PlayerAtlasLoader : BaseAddressableLoader<SpriteAtlas>
    {
        private SpriteAtlas _playerAtlas;

        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas> atlasHandle =
                Addressables.LoadAssetAsync<SpriteAtlas>(Constants.PLAYERS_ATLAS);
            handle = atlasHandle;
            _playerAtlas = await atlasHandle.Task;

            if (_playerAtlas == null)
            {
                Logger.Log($"Failed to load Sprite Atlas at address: {Constants.PLAYERS_ATLAS}");
            }
            else
            {
                Logger.Log("Players Sprite Atlas loaded successfully!");
            }

            return _playerAtlas;
        }

        public override void Release()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        public Sprite GetPlayerSprite(int playerId)
        {
            if (_playerAtlas == null)
            {
                Logger.Log("Atlas is not loaded yet.", LogType.Warning);
                return null;
            }

            string spriteName = $"player_{playerId}";
            Sprite sprite = _playerAtlas.GetSprite(spriteName);

            if (sprite == null)
            {
                Logger.Log($"Sprite not found: {spriteName}", LogType.Warning);
            }
            else
            {
                Logger.Log($"Sprite found: {spriteName}");
            }

            return sprite;
        }
    }
}
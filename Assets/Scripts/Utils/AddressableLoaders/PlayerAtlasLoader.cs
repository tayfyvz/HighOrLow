using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils.AddressableLoaders
{
    public class PlayerAtlasLoader : BaseAddressableLoader<SpriteAtlas>
    {
        private const string AssetAddress = "PlayersAtlas";

        private SpriteAtlas _playerAtlas;

        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas> atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(AssetAddress);
            handle = atlasHandle;
            _playerAtlas = await atlasHandle.Task;

            if (_playerAtlas == null)
            {
                Debug.LogError($"Failed to load Sprite Atlas at address: {AssetAddress}");
            }
            else
            {
                Debug.Log("Players Sprite Atlas loaded successfully!");
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
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }

            string spriteName = $"player_{playerId}";
            Sprite sprite = _playerAtlas.GetSprite(spriteName);

            if (sprite == null)
            {
                Debug.LogWarning($"Sprite not found: {spriteName}");
            }
            else
            {
                Debug.Log($"Sprite found: {spriteName}");
            }

            return sprite;
        }
    }
}
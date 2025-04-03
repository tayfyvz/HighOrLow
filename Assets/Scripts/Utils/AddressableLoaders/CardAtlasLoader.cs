using Cysharp.Threading.Tasks;
using Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils.AddressableLoaders
{
    public class CardAtlasLoader : BaseAddressableLoader<SpriteAtlas>
    {
        private const string AssetAddress = "CardAtlas";
        private const string CardBackAddress = "CardBack";

        private SpriteAtlas _cardAtlas;

        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas> atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(AssetAddress);
            handle = atlasHandle;
            _cardAtlas = await atlasHandle.Task;

            if (_cardAtlas == null)
            {
                Debug.LogError($"Failed to load Sprite Atlas at address: {AssetAddress}");
            }
            else
            {
                Debug.Log("Cards Sprite Atlas loaded successfully!");
            }
            
            return _cardAtlas;
        }

        public override void Release()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        public Sprite GetCardSprite(Card card)
        {
            if (_cardAtlas == null)
            {
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }

            string spriteName = GetSpriteName(card);
            Sprite sprite = _cardAtlas.GetSprite(spriteName);
            
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

        public Sprite GetCardBackSprite()
        {
            if (_cardAtlas == null)
            {
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }

            Sprite sprite = _cardAtlas.GetSprite(CardBackAddress);
            if (sprite == null)
            {
                Debug.LogWarning($"Card back sprite not found: {CardBackAddress}");
            }
            else
            {
                Debug.Log($"Card back sprite found: {CardBackAddress}");
            }
            
            return sprite;
        }
        
        private string GetSpriteName(Card card)
        {
            int index = ((int)card.Suit - 1) * 13 + (int)card.Rank;
            string indexStr = index.ToString("D2");

            string rankStr = GetRankString(card.Rank);
            string suitStr = GetSuitString(card.Suit);

            return $"{indexStr}_{rankStr}_{suitStr}";
        }

        private string GetRankString(Ranks rank)
        {
            switch (rank)
            {
                case Ranks.Ace: return "A";
                case Ranks.Two: return "2";
                case Ranks.Three: return "3";
                case Ranks.Four: return "4";
                case Ranks.Five: return "5";
                case Ranks.Six: return "6";
                case Ranks.Seven: return "7";
                case Ranks.Eight: return "8";
                case Ranks.Nine: return "9";
                case Ranks.Ten: return "10";
                case Ranks.Jack: return "J";
                case Ranks.Queen: return "Q";
                case Ranks.King: return "K";
                default: return "";
            }
        }

        private string GetSuitString(Suits suit)
        {
            switch (suit)
            {
                case Suits.Clubs: return "C";
                case Suits.Diamonds: return "D";
                case Suits.Hearts: return "H";
                case Suits.Spades: return "S";
                default: return "";
            }
        }
    }
}

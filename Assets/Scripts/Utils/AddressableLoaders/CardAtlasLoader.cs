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
        private SpriteAtlas _cardAtlas;

        public override async UniTask<SpriteAtlas> LoadAsync()
        {
            AsyncOperationHandle<SpriteAtlas> atlasHandle =
                Addressables.LoadAssetAsync<SpriteAtlas>(Constants.CARD_ATLAS);
            handle = atlasHandle;
            _cardAtlas = await atlasHandle.Task;

            if (_cardAtlas == null)
            {
                Logger.Log($"Failed to load Sprite Atlas at address: {Constants.CARD_ATLAS}", LogType.Error);
            }
            else
            {
                Logger.Log("Cards Sprite Atlas loaded successfully!");
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
                Logger.Log("Atlas is not loaded yet.", LogType.Warning);
                return null;
            }

            string spriteName = GetSpriteName(card);
            Sprite sprite = _cardAtlas.GetSprite(spriteName);

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

        public Sprite GetCardBackSprite()
        {
            if (_cardAtlas == null)
            {
                Logger.Log("Atlas is not loaded yet.", LogType.Warning);
                return null;
            }

            Sprite sprite = _cardAtlas.GetSprite(Constants.CARD_BACK_ADDRESS);
            if (sprite == null)
            {
                Logger.Log($"Card back sprite not found: {Constants.CARD_BACK_ADDRESS}", LogType.Warning);
            }
            else
            {
                Logger.Log($"Card back sprite found: {Constants.CARD_BACK_ADDRESS}");
            }

            return sprite;
        }

        private string GetSpriteName(Card card)
        {
            string rankStr = GetRankString(card.Rank);
            string suitStr = GetSuitString(card.Suit);

            string indexStr = ((int)card.Rank).ToString("D2");

            return $"{indexStr}_{rankStr}_{suitStr}";
        }

        private string GetRankString(Ranks rank)
        {
            return rank switch
            {
                Ranks.Ace => "A",
                Ranks.Two => "2",
                Ranks.Three => "3",
                Ranks.Four => "4",
                Ranks.Five => "5",
                Ranks.Six => "6",
                Ranks.Seven => "7",
                Ranks.Eight => "8",
                Ranks.Nine => "9",
                Ranks.Ten => "10",
                Ranks.Jack => "J",
                Ranks.Queen => "Q",
                Ranks.King => "K",
                _ => ""
            };
        }

        private string GetSuitString(Suits suit)
        {
            return suit switch
            {
                Suits.Clubs => "C",
                Suits.Diamonds => "D",
                Suits.Hearts => "H",
                Suits.Spades => "S",
                _ => ""
            };
        }
    }
}
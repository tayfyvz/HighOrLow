using Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Utils
{
    /// <summary>
    /// Loads a Sprite Atlas via Addressables and provides a helper method to retrieve a specific card sprite.
    /// </summary>
    public class CardAtlasLoader : MonoBehaviour
    {
        [Tooltip("The unique address of the Sprite Atlas as set in the Addressables system.")]
        [SerializeField]
        private string atlasAddress = "CardAtlas";

        private SpriteAtlas cardAtlas;

        /// <summary>
        /// Initiates the loading of the Sprite Atlas.
        /// </summary>
        public void LoadCardAtlas()
        {
            Addressables.LoadAssetAsync<SpriteAtlas>(atlasAddress)
                .Completed += OnAtlasLoaded;
        }

        /// <summary>
        /// Callback when the Sprite Atlas has finished loading.
        /// </summary>
        /// <param name="handle">The async operation's handle.</param>
        private void OnAtlasLoaded(AsyncOperationHandle<SpriteAtlas> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                cardAtlas = handle.Result;
                Debug.Log("Sprite Atlas loaded successfully!");
            }
            else
            {
                Debug.LogError("Failed to load the Sprite Atlas.");
            }
        }

        /// <summary>
        /// Retrieves the sprite corresponding to the given card.
        /// </summary>
        /// <param name="card">The card for which to retrieve the sprite.</param>
        /// <returns>The matching sprite if found; otherwise, null.</returns>
        public Sprite GetCardSprite(Card card)
        {
            if (cardAtlas == null)
            {
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }

            string spriteName = GetSpriteName(card); // Generate the expected sprite name.
            Sprite sprite = cardAtlas.GetSprite(spriteName);
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
            if (cardAtlas == null)
            {
                Debug.LogWarning("Atlas is not loaded yet.");
                return null;
            }
    
            string spriteName = "CardBack";
            Sprite sprite = cardAtlas.GetSprite(spriteName);
            if (sprite == null)
                Debug.LogWarning($"Card back sprite not found: {spriteName}");
            else
                Debug.Log($"Card back sprite found: {spriteName}");
    
            return sprite;
        }


        /// <summary>
        /// Constructs the sprite name based on the card's properties.
        /// </summary>
        /// <param name="card">The card model.</param>
        /// <returns>The formatted sprite name, e.g., "01_A_C".</returns>
        private string GetSpriteName(Card card)
        {
            int index = ((int)card.Suit - 1) * 13 + (int)card.Rank;
            string indexStr = index.ToString("D2");

            string rankAbbrev = GetRankAbbreviation(card.Rank);
            string suitAbbrev = GetSuitAbbreviation(card.Suit);

            return $"{indexStr}_{rankAbbrev}_{suitAbbrev}";
        }

        /// <summary>
        /// Returns the abbreviation for the given card rank.
        /// </summary>
        /// <param name="rank">The card rank.</param>
        /// <returns>The rank abbreviation.</returns>
        private string GetRankAbbreviation(Ranks rank)
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

        /// <summary>
        /// Returns the abbreviation for the given suit.
        /// </summary>
        /// <param name="suit">The card suit.</param>
        /// <returns>The suit abbreviation.</returns>
        private string GetSuitAbbreviation(Suits suit)
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

        private void Start()
        {
            LoadCardAtlas();
        }
    }
}

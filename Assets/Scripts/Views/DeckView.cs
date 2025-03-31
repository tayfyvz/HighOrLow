using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class DeckView : MonoBehaviour, IDeckView, IView
    {
        [SerializeField]
        private Text deckCountText; // Assign this via the Inspector to a Text UI element.

        /// <summary>
        /// Updates the deck count display.
        /// </summary>
        /// <param name="count">Number of cards remaining in the deck.</param>
        public void UpdateDeckCount(int count)
        {
            if (deckCountText != null)
            {
                deckCountText.text = "Cards remaining: " + count;
            }
        }
    }
}

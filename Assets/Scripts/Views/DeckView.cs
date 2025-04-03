using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class DeckView : MonoBehaviour, IDeckView, IView
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _deckCountText;
        [SerializeField] private Image deckCountBackgroundImage; // background for deck count

        public void UpdateDeckCount(int count)
        {
            if (_deckCountText != null)
            {
                _deckCountText.text = "Cards remaining: " + count;
            }
        }
    }
}

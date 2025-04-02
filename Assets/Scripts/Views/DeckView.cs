using TMPro;
using UnityEngine;

namespace Views
{
    public class DeckView : MonoBehaviour, IDeckView, IView
    {
        [SerializeField] private TextMeshProUGUI _deckCountText;

        public void UpdateDeckCount(int count)
        {
            if (_deckCountText != null)
            {
                _deckCountText.text = "Cards remaining: " + count;
            }
        }
    }
}

using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

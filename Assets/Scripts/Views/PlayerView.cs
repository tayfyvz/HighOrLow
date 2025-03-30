using Models;
using TMPro;
using UnityEngine;

namespace Views
{
    public class PlayerView : MonoBehaviour
    {
        public TextMeshProUGUI PlayerNameText; // Reference assigned via prefab
        public TextMeshProUGUI HandText;       // Reference assigned via prefab

        public void UpdateView(Player player)
        {
            if (player == null) return;

            if (PlayerNameText != null)
                PlayerNameText.text = player.Name;

            if (HandText != null)
            {
                HandText.text = "";
                foreach (var card in player.Hand)
                {
                    HandText.text += card.ToString() + "\n";
                }
            }
        }
    }
}
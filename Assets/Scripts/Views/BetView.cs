using TMPro;
using UnityEngine;

namespace Views
{
    public class BetView : MonoBehaviour, IBetView, IView
    {
        public TextMeshProUGUI ScoreText;  // Assigned via Inspector.

        /// <summary>
        /// Updates the UI score text.
        /// </summary>
        public void UpdateScore(int score)
        {
            if (ScoreText != null)
                ScoreText.text = "Score: " + score;
        }
    }
}
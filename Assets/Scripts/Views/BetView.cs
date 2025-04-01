using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Views
{
    public class BetView : MonoBehaviour, IBetView, IView
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        public void UpdateScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = "Score: " + score;
        }

        public void ResetView()
        {
            if (_scoreText != null)
                _scoreText.text = "Score: 0";
        }
    }
}
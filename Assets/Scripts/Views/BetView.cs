using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BetView : MonoBehaviour, IBetView, IView
    {
        [Header("Bet Button Prefab")]
        [SerializeField] private Button _betButtonPrefab;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void InstantiateBetButtons(Vector2[] playersPositions, Action<int> onBetButtonClicked)
        {
            for (int i = 0; i < playersPositions.Length; i++)
            {
                Vector2 offsetPos = new Vector2(playersPositions[i].x, playersPositions[i].y - 120f);
                Button betButton = Instantiate(_betButtonPrefab, offsetPos, Quaternion.identity, _transform);
                int index = i;
                betButton.onClick.AddListener(() => onBetButtonClicked(index));
            }
        }

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
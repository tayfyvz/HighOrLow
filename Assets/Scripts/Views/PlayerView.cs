using System;
using Controllers;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Views
{
    public class PlayerView : MonoBehaviour, IPlayerView, IView
    {
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private TextMeshProUGUI _handText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void SetPlayerName(string name)
        {
            if (_playerNameText != null)
                _playerNameText.text = name;
        }

        public void UpdateHand(Card card)
        {
            if (_handText == null)
            {
                return;
            }

            _handText.text = card.ToString() + "\n";
            
            // Add Visual 
        }

        public void SetPosition(Vector2 position)
        {
            _transform.position = position;
        }

        public void SetScore(int score)
        {
            if (_scoreText == null)
            {
                return;
            }

            _scoreText.text = score.ToString();
        }

        public void WinSession()
        {
            Debug.LogError("WIN", this);
        }

        public void ResetView()
        {
            SetScore(0);
            _handText.text = String.Empty;
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.AddressableLoaders;

namespace Views
{
    public class BetView : MonoBehaviour, IBetView, IView
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private BetButtonLoader _betButtonLoader;
        private Transform _transform;
        
        private void Start()
        {
            _betButtonLoader = new BetButtonLoader();
            _transform = transform;
        }
        
        public async UniTask InstantiateBetButtons(Vector2[] playersPositions, Action<int> onBetButtonClicked)
        {
            for (int i = 0; i < playersPositions.Length; i++)
            {
                Vector2 offsetPos = new Vector2(playersPositions[i].x, playersPositions[i].y - 120f);
                Button betButton = await _betButtonLoader.LoadAsync();
                
                if (betButton != null)
                {
                    betButton.transform.SetParent(_transform, false);
                    betButton.transform.position = offsetPos;
                    
                    int index = i;
                    betButton.onClick.AddListener(() => onBetButtonClicked(index));
                }
            }
        }

        public void UpdateScore(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = "Score: " + score;
            }
        }

        public void ResetView()
        {
            if (_scoreText != null)
            {
                _scoreText.text = "Score: 0";
            }
        }

        private void OnDestroy()
        {
            if (_betButtonLoader == null)
            {
                return;
            }
            
            _betButtonLoader.Release();
            _betButtonLoader = null;
        }
    }
}
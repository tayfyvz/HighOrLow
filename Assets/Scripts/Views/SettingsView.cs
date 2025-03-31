using System;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Views
{
    public class SettingsView : MonoBehaviour, IView
    {
        [Header("Settings UI Elements")]
        [Range(2, 4)]
        [SerializeField] private int _numberOfPlayers = 2; 
        // Set dropdown menu
        
        [SerializeField] private CardsWeightConfig _defaultCardsWeightConfig;
        
        [SerializeField] private Button _applyButton;
        
        private CardsWeightConfig _tempCardsWeightConfig;

        private void Awake()
        {
            _tempCardsWeightConfig = Instantiate(_defaultCardsWeightConfig);
        }

        private void OnEnable()
        {
            ResetToDefault();
            _applyButton.onClick.AddListener(OnApplyButtonClicked);
        }
        
        private void OnDisable()
        {
            _applyButton.onClick.RemoveListener(OnApplyButtonClicked);
        }

        private void OnApplyButtonClicked()
        {
            _applyButton.onClick.RemoveListener(OnApplyButtonClicked);
            ApplyChanges();
        }
        
        private void ResetToDefault()
        {
            _tempCardsWeightConfig = _defaultCardsWeightConfig;
            ApplyChanges();
        }

        private void ApplyChanges()
        {
            SettingsController.Instance.SetSettings(_numberOfPlayers,
                _tempCardsWeightConfig.DefaultWeight,
                _tempCardsWeightConfig.SuitOverrides,
                _tempCardsWeightConfig.CardOverrides);
        }
    }
}
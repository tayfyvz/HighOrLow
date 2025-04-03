using System;
using System.Collections.Generic;
using Controllers;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Views
{
    public class SettingsView : MonoBehaviour, IView
    {
        [Header("Modular Settings References")]
        [Tooltip("Reference to the PlayerCountSettings component.")]
        [SerializeField] private PlayerCountSettings _playerCountSettings;
        
        [Tooltip("Reference to the SuitWeightSettings component.")]
        [SerializeField] private SuitWeightSettings _suitWeightSettings;
        
        [Tooltip("Reference to the RankWeightSettings component.")]
        [SerializeField] private RankWeightSettings _rankWeightSettings;
        
        [Header("Default Config Reference")]
        [Tooltip("Default CardsWeightConfig asset (used as the baseline).")]
        [SerializeField] private CardsWeightConfig _defaultCardsWeightConfig;
        
        [Header("Buttons")]
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _resetButton;
        
        // Temporary configuration that will be updated from the UI modules.
        private CardsWeightConfig _tempCardsWeightConfig;
        
        private void Awake()
        {
            if (_defaultCardsWeightConfig == null)
            {
                Debug.LogError("Default CardsWeightConfig is not assigned in SettingsView!");
            }
            else
            {
                // Create a copy of the default configuration.
                _tempCardsWeightConfig = Instantiate(_defaultCardsWeightConfig);
            }
        }
        
        private void OnEnable()
        {
            // Only initialize if required modular components are assigned.
            if (_playerCountSettings != null && _suitWeightSettings != null && _rankWeightSettings != null)
            {
                // Initialize the modular components with values from the default config.
                ResetViewToDefault();
            }
            else
            {
                Debug.LogError("One or more modular settings components are not assigned in SettingsView!");
            }
            
            if (_applyButton != null)
            {
                _applyButton.onClick.AddListener(OnApplyButtonClicked);
            }
            else
            {
                Debug.LogError("Apply Button is not assigned in SettingsView!");
            }
            
            if (_resetButton != null)
            {
                _resetButton.onClick.AddListener(OnResetButtonClicked);
            }
            else
            {
                Debug.LogError("Reset Button is not assigned in SettingsView!");
            }
        }
        
        private void OnDisable()
        {
            // Use null checks for all button references.
            if (_applyButton != null)
            {
                _applyButton.onClick.RemoveListener(OnApplyButtonClicked);
            }
            if (_resetButton != null)
            {
                _resetButton.onClick.RemoveListener(OnResetButtonClicked);
            }
        }
        
        /// <summary>
        /// Reinitializes the sub-components to display default values from _defaultCardsWeightConfig.
        /// Because _defaultCardsWeightConfig already contains the desired default settings—
        /// (player count = 2, Hearts weight = 2.3, and Ace of Spades weight = 3.1)—
        /// these values will appear in the UI.
        /// </summary>
        private void ResetViewToDefault()
        {
            if (_defaultCardsWeightConfig == null)
            {
                Debug.LogError("Default CardsWeightConfig is missing.");
                return;
            }
            
            // Recreate the temporary config.
            _tempCardsWeightConfig = Instantiate(_defaultCardsWeightConfig);
            
            // Initialize each modular component with their default values.
            _playerCountSettings.Initialize(2); // e.g., sets player count to 2.
            _suitWeightSettings.Initialize(_tempCardsWeightConfig.SuitOverrides);
            _rankWeightSettings.Initialize(_tempCardsWeightConfig.CardOverrides);
        }
        
        /// <summary>
        /// When the Apply button is clicked, each module is checked:
        /// - If a module’s HasChanged flag is true, the new settings from that component are written 
        ///   into _tempCardsWeightConfig.
        /// - Then SettingsManager.Instance.SetSettings(...) is called with the updated values.
        /// Afterwards, the components’ change flags are reset.
        /// </summary>
        private void OnApplyButtonClicked()
        {
            if (_playerCountSettings == null || _suitWeightSettings == null || _rankWeightSettings == null)
            {
                Debug.LogError("One or more settings components are not assigned.");
                return;
            }

            // Always update the player count.
            int currentPlayerCount = _playerCountSettings.GetCurrentPlayerCount();
            
            // Update suit overrides only if changes were made.
            if (_suitWeightSettings.HasChanged)
            {
                _tempCardsWeightConfig.SuitOverrides.Clear();
                _tempCardsWeightConfig.SuitOverrides.AddRange(_suitWeightSettings.GetCurrentSuitOverrides());
            }
            
            // Update rank (card) overrides only if changes were made.
            if (_rankWeightSettings.HasChanged)
            {
                _tempCardsWeightConfig.CardOverrides.Clear();
                _tempCardsWeightConfig.CardOverrides.AddRange(_rankWeightSettings.GetCurrentCardOverrides());
            }
            
            // Pass these values to the SettingsManager.
            SettingsManager.Instance.SetSettings(
                currentPlayerCount,
                _tempCardsWeightConfig.DefaultWeight,
                _tempCardsWeightConfig.SuitOverrides,
                _tempCardsWeightConfig.CardOverrides);
            
            Debug.Log("Applied settings:");
            Debug.Log("Player Count: " + currentPlayerCount);
            foreach (var suitData in _tempCardsWeightConfig.SuitOverrides)
            {
                Debug.Log("Suit " + suitData.Suit + " Weight: " + suitData.Weight);
            }
            foreach (var cardData in _tempCardsWeightConfig.CardOverrides)
            {
                Debug.Log("Card " + cardData.Suit + " " + cardData.Rank + " Weight: " + cardData.Weight);
            }
            
            // Reset the change flags on each component.
            _playerCountSettings.MarkAsNotChanged();
            _suitWeightSettings.MarkAsNotChanged();
            _rankWeightSettings.MarkAsNotChanged();
            
            if (GameInitializer.Instance != null)
            {
                _ = GameInitializer.Instance.LoadGameSceneWithLoadingAsync();
            }
            else
            {
                Debug.LogError("GameInitializer instance not found. Falling back to direct scene load.");
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }
        
        /// <summary>
        /// Resets all modules to their default values and applies those settings.
        /// </summary>
        private void OnResetButtonClicked()
        {
            ResetViewToDefault();
            OnApplyButtonClicked();
        }
    }
}

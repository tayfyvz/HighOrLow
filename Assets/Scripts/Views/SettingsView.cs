using Managers;
using Managers.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Views.Settings;
using Logger = Utils.Logger;

namespace Views
{
    public class SettingsView : MonoBehaviour, IView
    {
        [Header("Modular Settings References")]
        [SerializeField] private PlayerCountSettings _playerCountSettings;
        [SerializeField] private SuitWeightSettings _suitWeightSettings;
        [SerializeField] private RankWeightSettings _rankWeightSettings;

        [Header("Default Config Reference")]
        [SerializeField] private CardsWeightConfig _defaultCardsWeightConfig;

        [Header("Buttons")]
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _resetButton;

        private CardsWeightConfig _tempCardsWeightConfig;

        private void Awake()
        {
            if (_defaultCardsWeightConfig == null)
            {
                Logger.Log("Default CardsWeightConfig is not assigned in SettingsView!", LogType.Error);
            }
            else
            {
                _tempCardsWeightConfig = Instantiate(_defaultCardsWeightConfig);
            }
        }

        private void OnEnable()
        {
            if (_playerCountSettings != null && _suitWeightSettings != null && _rankWeightSettings != null)
            {
                ResetViewToDefault();
            }
            else
            {
                Logger.Log("One or more modular settings components are not assigned in SettingsView!", LogType.Error);
            }

            _applyButton?.onClick.AddListener(OnApplyButtonClicked);
            if (_applyButton == null) Logger.Log("Apply Button is not assigned in SettingsView!", LogType.Error);

            _resetButton?.onClick.AddListener(OnResetButtonClicked);
            if (_resetButton == null) Logger.Log("Reset Button is not assigned in SettingsView!", LogType.Error);

            if (_suitWeightSettings != null && _rankWeightSettings != null)
            {
                _suitWeightSettings.OnSuitWeightChanged += _rankWeightSettings.UpdateRankWeights;
            }
        }

        private void OnDisable()
        {
            _applyButton?.onClick.RemoveListener(OnApplyButtonClicked);
            _resetButton?.onClick.RemoveListener(OnResetButtonClicked);

            if (_suitWeightSettings != null && _rankWeightSettings != null)
            {
                _suitWeightSettings.OnSuitWeightChanged -= _rankWeightSettings.UpdateRankWeights;
            }
        }

        private void ResetViewToDefault()
        {
            if (_defaultCardsWeightConfig == null)
            {
                Logger.Log("Default CardsWeightConfig is missing.", LogType.Error);
                return;
            }

            _tempCardsWeightConfig = Instantiate(_defaultCardsWeightConfig);

            _playerCountSettings.Initialize(2);
            _suitWeightSettings.Initialize(_tempCardsWeightConfig.SuitOverrides);
            _rankWeightSettings.Initialize(_tempCardsWeightConfig.CardOverrides);
        }

        private void OnApplyButtonClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);

            if (_playerCountSettings == null || _suitWeightSettings == null || _rankWeightSettings == null)
            {
                Logger.Log("One or more settings components are not assigned.", LogType.Error);
                return;
            }

            int currentPlayerCount = _playerCountSettings.GetCurrentPlayerCount();

            if (_suitWeightSettings.HasChanged)
            {
                _tempCardsWeightConfig.SuitOverrides.Clear();
                _tempCardsWeightConfig.SuitOverrides.AddRange(_suitWeightSettings.GetCurrentSuitOverrides());
            }

            if (_rankWeightSettings.HasChanged)
            {
                _tempCardsWeightConfig.CardOverrides.Clear();
                _tempCardsWeightConfig.CardOverrides.AddRange(_rankWeightSettings.GetCurrentCardOverrides());
            }

            SettingsManager.Instance.SetSettings(
                currentPlayerCount,
                _tempCardsWeightConfig.DefaultWeight,
                _tempCardsWeightConfig.SuitOverrides,
                _tempCardsWeightConfig.CardOverrides
            );

            Logger.Log("Applied settings:", LogType.Log);
            Logger.Log($"Player Count: {currentPlayerCount}", LogType.Log);
            foreach (var suitData in _tempCardsWeightConfig.SuitOverrides)
            {
                Logger.Log($"Suit {suitData.Suit} Weight: {suitData.Weight}", LogType.Log);
            }
            foreach (var cardData in _tempCardsWeightConfig.CardOverrides)
            {
                Logger.Log($"Card {cardData.Suit} {cardData.Rank} Weight: {cardData.Weight}", LogType.Log);
            }

            _playerCountSettings.MarkAsNotChanged();
            _suitWeightSettings.MarkAsNotChanged();
            _rankWeightSettings.MarkAsNotChanged();

            if (GameInitializer.Instance != null)
            {
                _ = GameInitializer.Instance.LoadGameSceneWithLoadingAsync();
            }
            else
            {
                Logger.Log("GameInitializer instance not found. Falling back to direct scene load.", LogType.Error);
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }

        private void OnResetButtonClicked()
        {
            SoundManager.Instance.PlaySound(SoundType.ButtonClick);
            ResetViewToDefault();
        }
    }
}

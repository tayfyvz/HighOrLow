using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Utils.Logger;

namespace Views.Settings
{
    public class RankWeightSettings : MonoBehaviour
    {
        [Header("Rank Weight UI")]
        [SerializeField] private TMP_Dropdown rankSuitDropdown;
        [SerializeField] private TMP_InputField[] rankWeightInputFields;
        [SerializeField] private TMP_Text[] rankLabels;
        [Header("Layout Groups")]
        [SerializeField] private VerticalLayoutGroup[] _layoutGroups;

        private List<CardOverrideData> _cardOverrides = new List<CardOverrideData>();
        private Suits[] _allSuits;
        private Ranks[] _allRanks;
        public bool HasChanged { get; private set; } = false;

        private void OnEnable()
        {
            if (_layoutGroups == null) return;
            foreach (var layoutGroup in _layoutGroups)
                HandleLayoutGroup(layoutGroup, 0.15f).Forget();
        }

        private async UniTaskVoid HandleLayoutGroup(VerticalLayoutGroup layoutGroup, float delay)
        {
            if (layoutGroup == null) return;
            layoutGroup.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            layoutGroup.enabled = false;
        }

        public void Initialize(List<CardOverrideData> defaultOverrides)
        {
            _allSuits = (Suits[])Enum.GetValues(typeof(Suits));
            _allRanks = (Ranks[])Enum.GetValues(typeof(Ranks));
            _cardOverrides = new List<CardOverrideData>(defaultOverrides);

            var options = new List<string>();
            foreach (Suits s in _allSuits) options.Add(s.ToString());
            rankSuitDropdown.ClearOptions();
            rankSuitDropdown.AddOptions(options);
            rankSuitDropdown.onValueChanged.RemoveAllListeners();
            rankSuitDropdown.onValueChanged.AddListener(OnRankSuitDropdownChanged);

            if (rankWeightInputFields.Length != _allRanks.Length || rankLabels.Length != _allRanks.Length)
            {
                Logger.Log($"There must be exactly {_allRanks.Length} rank input fields and labels.", LogType.Error);
                return;
            }

            for (int i = 0; i < _allRanks.Length; i++)
            {
                rankLabels[i].text = _allRanks[i].ToString();
                int index = i;
                rankWeightInputFields[i].onEndEdit.RemoveAllListeners();
                rankWeightInputFields[i].onEndEdit.AddListener(input => OnRankWeightInputChanged(index, input));
            }

            rankSuitDropdown.value = 0;
            UpdateInputFieldsForSelectedSuit();
            HasChanged = false;
        }

        private void OnRankSuitDropdownChanged(int index) => UpdateInputFieldsForSelectedSuit();

        private void UpdateInputFieldsForSelectedSuit()
        {
            var selectedSuit = _allSuits[rankSuitDropdown.value];
            for (int i = 0; i < _allRanks.Length; i++)
                rankWeightInputFields[i].text = GetWeightForCard(selectedSuit, _allRanks[i]).ToString("F2");
        }

        private float GetWeightForCard(Suits suit, Ranks rank)
        {
            foreach (var data in _cardOverrides)
                if (data.Suit == suit && data.Rank == rank) return data.Weight;
            return 1f;
        }

        private void OnRankWeightInputChanged(int rankIndex, string input)
        {
            if (float.TryParse(input, out float newWeight))
            {
                var selectedSuit = _allSuits[rankSuitDropdown.value];
                var rank = _allRanks[rankIndex];
                bool found = false;
                for (int i = 0; i < _cardOverrides.Count; i++)
                {
                    if (_cardOverrides[i].Suit == selectedSuit && _cardOverrides[i].Rank == rank)
                    {
                        if (Math.Abs(_cardOverrides[i].Weight - newWeight) > 0.001f)
                        {
                            var updated = _cardOverrides[i];
                            updated.Weight = newWeight;
                            _cardOverrides[i] = updated;
                            HasChanged = true;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _cardOverrides.Add(new CardOverrideData { Suit = selectedSuit, Rank = rank, Weight = newWeight });
                    HasChanged = true;
                }
            }
            else
            {
                var selectedSuit = _allSuits[rankSuitDropdown.value];
                var rank = _allRanks[rankIndex];
                rankWeightInputFields[rankIndex].text = GetWeightForCard(selectedSuit, rank).ToString("F2");
            }
        }

        public List<CardOverrideData> GetCurrentCardOverrides() => new List<CardOverrideData>(_cardOverrides);

        public void ResetToDefault()
        {
            HasChanged = false;
            UpdateInputFieldsForSelectedSuit();
        }

        public void MarkAsNotChanged() => HasChanged = false;

        public void UpdateRankWeights(Suits updatedSuit, float newSuitWeight)
        {
            foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
            {
                bool found = false;
                for (int i = 0; i < _cardOverrides.Count; i++)
                {
                    if (_cardOverrides[i].Suit == updatedSuit && _cardOverrides[i].Rank == rank)
                    {
                        var updatedCard = _cardOverrides[i];
                        updatedCard.Weight = newSuitWeight;
                        _cardOverrides[i] = updatedCard;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _cardOverrides.Add(new CardOverrideData { Suit = updatedSuit, Rank = rank, Weight = newSuitWeight });
                }
            }
            UpdateInputFieldsForSelectedSuit();
        }
    }
}

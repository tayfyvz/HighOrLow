using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using Models;
using UnityEngine.UI;

namespace Views
{
    public class RankWeightSettings : MonoBehaviour
    {
        [Header("Rank Weight UI")]
        [SerializeField] private TMP_Dropdown rankSuitDropdown;
        [SerializeField] private TMP_InputField[] rankWeightInputFields;
        [SerializeField] private TMP_Text[] rankLabels;
        
        [Header("Layout Groups")]
        [SerializeField] private VerticalLayoutGroup[] _layoutGroups;
        
        // Internal storage for card (rank) overrides.
        private List<CardOverrideData> cardOverrides = new List<CardOverrideData>();
        private Suits[] allSuits;
        private Ranks[] allRanks;
        
        // Change flag.
        public bool HasChanged { get; private set; } = false;

        private void OnEnable()
        {
            if (_layoutGroups == null)
            {
                return;
            }
            
            foreach (var layoutGroup in _layoutGroups)
            {
                HandleLayoutGroup(layoutGroup, 0.15f).Forget();
            }
        }
        
        private async UniTaskVoid HandleLayoutGroup(VerticalLayoutGroup layoutGroup, float delay)
        {
            if (layoutGroup != null)
            {
                layoutGroup.enabled = true;
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
                layoutGroup.enabled = false;
            }
        }

        public void Initialize(List<CardOverrideData> defaultOverrides)
        {
            allSuits = (Suits[])Enum.GetValues(typeof(Suits));
            allRanks = (Ranks[])Enum.GetValues(typeof(Ranks));
            
            // Copy defaults.
            cardOverrides = new List<CardOverrideData>(defaultOverrides);
            
            // Populate the suit dropdown.
            List<string> options = new List<string>();
            foreach (Suits s in allSuits)
            {
                options.Add(s.ToString());
            }
            rankSuitDropdown.ClearOptions();
            rankSuitDropdown.AddOptions(options);
            rankSuitDropdown.onValueChanged.RemoveAllListeners();
            rankSuitDropdown.onValueChanged.AddListener(OnRankSuitDropdownChanged);
            
            // Setup the rank input fields and labels.
            if (rankWeightInputFields.Length != allRanks.Length || rankLabels.Length != allRanks.Length)
            {
                Debug.LogError("There must be exactly " + allRanks.Length + " rank input fields and labels.");
                return;
            }
            for (int i = 0; i < allRanks.Length; i++)
            {
                rankLabels[i].text = allRanks[i].ToString();
                int index = i; // capture local copy for lambda.
                rankWeightInputFields[i].onEndEdit.RemoveAllListeners();
                rankWeightInputFields[i].onEndEdit.AddListener((string input) => OnRankWeightInputChanged(index, input));
            }
            
            // Set the default suit selection.
            rankSuitDropdown.value = 0;
            UpdateInputFieldsForSelectedSuit();
            
            HasChanged = false;
        }
        
        private void OnRankSuitDropdownChanged(int index)
        {
            UpdateInputFieldsForSelectedSuit();
        }
        
        /// <summary>
        /// Updates the rank input fields to show current weights for the selected suit.
        /// </summary>
        private void UpdateInputFieldsForSelectedSuit()
        {
            Suits selectedSuit = allSuits[rankSuitDropdown.value];
            for (int i = 0; i < allRanks.Length; i++)
            {
                float weight = GetWeightForCard(selectedSuit, allRanks[i]);
                rankWeightInputFields[i].text = weight.ToString("F2");
            }
        }
        
        /// <summary>
        /// Retrieves the stored weight for a given suit and rank.
        /// </summary>
        private float GetWeightForCard(Suits suit, Ranks rank)
        {
            foreach (var data in cardOverrides)
            {
                if (data.Suit == suit && data.Rank == rank)
                    return data.Weight;
            }
            return 1f; // default fallback
        }
        
        private void OnRankWeightInputChanged(int rankIndex, string input)
        {
            if (float.TryParse(input, out float newWeight))
            {
                Suits selectedSuit = allSuits[rankSuitDropdown.value];
                Ranks rank = allRanks[rankIndex];
                bool found = false;
                for (int i = 0; i < cardOverrides.Count; i++)
                {
                    if (cardOverrides[i].Suit == selectedSuit && cardOverrides[i].Rank == rank)
                    {
                        if (Math.Abs(cardOverrides[i].Weight - newWeight) > 0.001f)
                        {
                            CardOverrideData updated = cardOverrides[i];
                            updated.Weight = newWeight;
                            cardOverrides[i] = updated;
                            HasChanged = true;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    cardOverrides.Add(new CardOverrideData { Suit = selectedSuit, Rank = rank, Weight = newWeight });
                    HasChanged = true;
                }
            }
            else
            {
                // Revert to stored value on parsing failure.
                Suits selectedSuit = allSuits[rankSuitDropdown.value];
                Ranks rank = allRanks[rankIndex];
                rankWeightInputFields[rankIndex].text = GetWeightForCard(selectedSuit, rank).ToString("F2");
            }
        }
        
        /// <summary>
        /// Returns a copy of the current card overrides.
        /// </summary>
        public List<CardOverrideData> GetCurrentCardOverrides()
        {
            return new List<CardOverrideData>(cardOverrides);
        }
        
        public void ResetToDefault()
        {
            HasChanged = false;
            // Re-display current values (assumed to be the defaults already loaded).
            UpdateInputFieldsForSelectedSuit();
        }
        
        public void MarkAsNotChanged()
        {
            HasChanged = false;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Models;

namespace Views
{
    public class SuitWeightSettings : MonoBehaviour
    {
        [Header("Suit Weight UI")]
        [SerializeField] private TMP_Dropdown suitDropdown;
        [SerializeField] private TMP_InputField suitWeightInputField;
        
        // Internal storage for the suit overrides.
        private List<SuitOverrideData> suitOverrides = new List<SuitOverrideData>();
        private Suits[] allSuits;
        
        // The change flag.
        public bool HasChanged { get; private set; } = false;
        
        /// <summary>
        /// Initializes the UI with the default override list.
        /// </summary>
        public void Initialize(List<SuitOverrideData> defaultOverrides)
        {
            allSuits = (Suits[])Enum.GetValues(typeof(Suits));
            // Copy defaults into internal storage.
            suitOverrides = new List<SuitOverrideData>(defaultOverrides);
            
            // Populate the dropdown using enum names.
            List<string> options = new List<string>();
            foreach (Suits s in allSuits)
            {
                options.Add(s.ToString());
            }
            suitDropdown.ClearOptions();
            suitDropdown.AddOptions(options);
            suitDropdown.onValueChanged.RemoveAllListeners();
            suitDropdown.onValueChanged.AddListener(OnDropdownChanged);
            
            // Set initial selection to the first suit.
            suitDropdown.value = 0;
            
            // Set up the input field.
            suitWeightInputField.onEndEdit.RemoveAllListeners();
            suitWeightInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
            suitWeightInputField.text = GetWeightForSuit(allSuits[0]).ToString("F2");
            
            HasChanged = false;
        }
        
        /// <summary>
        /// Returns the stored weight for a given suit.
        /// </summary>
        private float GetWeightForSuit(Suits suit)
        {
            foreach (var data in suitOverrides)
            {
                if (data.Suit == suit)
                    return data.Weight;
            }
            return 1f; // fallback default
        }
        
        private void OnDropdownChanged(int index)
        {
            Suits selected = allSuits[index];
            suitWeightInputField.text = GetWeightForSuit(selected).ToString("F2");
        }
        
        private void OnInputFieldEndEdit(string input)
        {
            if (float.TryParse(input, out float newWeight))
            {
                int index = suitDropdown.value;
                Suits selected = allSuits[index];
                bool found = false;
                for (int i = 0; i < suitOverrides.Count; i++)
                {
                    if (suitOverrides[i].Suit == selected)
                    {
                        // Update only if the value is different.
                        if (Math.Abs(suitOverrides[i].Weight - newWeight) > 0.001f)
                        {
                            SuitOverrideData updated = suitOverrides[i];
                            updated.Weight = newWeight;
                            suitOverrides[i] = updated;
                            HasChanged = true;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    suitOverrides.Add(new SuitOverrideData { Suit = selected, Weight = newWeight });
                    HasChanged = true;
                }
            }
            else
            {
                // Revert to stored value if parsing fails.
                int index = suitDropdown.value;
                Suits selected = allSuits[index];
                suitWeightInputField.text = GetWeightForSuit(selected).ToString("F2");
            }
        }
        
        /// <summary>
        /// Returns a copy of the current suit overrides.
        /// </summary>
        public List<SuitOverrideData> GetCurrentSuitOverrides()
        {
            return new List<SuitOverrideData>(suitOverrides);
        }
        
        /// <summary>
        /// Resets the change flag (if resetting is done externally).
        /// </summary>
        public void ResetToDefault()
        {
            HasChanged = false;
            // The UI reset should be handled via Initialize() from the composite view.
        }
        
        public void MarkAsNotChanged()
        {
            HasChanged = false;
        }
    }
}

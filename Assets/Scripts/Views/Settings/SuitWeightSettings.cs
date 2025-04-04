using System;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;

namespace Views.Settings
{
    public class SuitWeightSettings : MonoBehaviour
    {
        public event Action<Suits, float> OnSuitWeightChanged;

        [Header("Suit Weight UI")]
        [SerializeField] private TMP_Dropdown suitDropdown;
        [SerializeField] private TMP_InputField suitWeightInputField;

        private List<SuitOverrideData> _suitOverrides = new List<SuitOverrideData>();
        private Suits[] _allSuits;
        public bool HasChanged { get; private set; } = false;

        public void Initialize(List<SuitOverrideData> defaultOverrides)
        {
            _allSuits = (Suits[])Enum.GetValues(typeof(Suits));
            _suitOverrides = new List<SuitOverrideData>(defaultOverrides);

            var options = new List<string>();
            foreach (Suits s in _allSuits) options.Add(s.ToString());
            suitDropdown.ClearOptions();
            suitDropdown.AddOptions(options);
            suitDropdown.onValueChanged.RemoveAllListeners();
            suitDropdown.onValueChanged.AddListener(OnDropdownChanged);

            suitDropdown.value = 0;

            suitWeightInputField.onEndEdit.RemoveAllListeners();
            suitWeightInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
            suitWeightInputField.text = GetWeightForSuit(_allSuits[0]).ToString("F2");
            HasChanged = false;
        }

        private float GetWeightForSuit(Suits suit)
        {
            foreach (var data in _suitOverrides)
                if (data.Suit == suit) return data.Weight;
            return 1f;
        }

        private void OnDropdownChanged(int index)
        {
            var selected = _allSuits[index];
            suitWeightInputField.text = GetWeightForSuit(selected).ToString("F2");
        }

        private void OnInputFieldEndEdit(string input)
        {
            if (float.TryParse(input, out float newWeight))
            {
                var index = suitDropdown.value;
                var selected = _allSuits[index];
                bool found = false;

                for (int i = 0; i < _suitOverrides.Count; i++)
                {
                    if (_suitOverrides[i].Suit == selected)
                    {
                        if (Math.Abs(_suitOverrides[i].Weight - newWeight) > 0.001f)
                        {
                            var updated = _suitOverrides[i];
                            updated.Weight = newWeight;
                            _suitOverrides[i] = updated;
                            HasChanged = true;
                            OnSuitWeightChanged?.Invoke(selected, newWeight);
                        }
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _suitOverrides.Add(new SuitOverrideData { Suit = selected, Weight = newWeight });
                    HasChanged = true;
                    OnSuitWeightChanged?.Invoke(selected, newWeight);
                }
            }
            else
            {
                var index = suitDropdown.value;
                var selected = _allSuits[index];
                suitWeightInputField.text = GetWeightForSuit(selected).ToString("F2");
            }
        }

        public List<SuitOverrideData> GetCurrentSuitOverrides() => new List<SuitOverrideData>(_suitOverrides);

        public void ResetToDefault() => HasChanged = false;

        public void MarkAsNotChanged() => HasChanged = false;
    }
}

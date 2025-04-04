using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Settings
{
    public class PlayerCountSettings : MonoBehaviour
    {
        [Header("Player Count UI")]
        [SerializeField] private Slider playerCountSlider;
        [SerializeField] private TMP_Text playerCountText;

        private int _defaultPlayerCount = 2;
        public bool HasChanged { get; private set; } = false;

        private void Awake()
        {
            playerCountSlider.wholeNumbers = true;
        }

        public void Initialize(int defaultCount)
        {
            _defaultPlayerCount = defaultCount;
            playerCountSlider.minValue = 2;
            playerCountSlider.maxValue = 4;
            playerCountSlider.value = _defaultPlayerCount;
            playerCountText.text = _defaultPlayerCount.ToString();
            HasChanged = false;

            playerCountSlider.onValueChanged.RemoveAllListeners();
            playerCountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            int count = (int)value;
            playerCountText.text = count.ToString();
            HasChanged = (count != _defaultPlayerCount);
        }

        public int GetCurrentPlayerCount()
        {
            return (int)playerCountSlider.value;
        }

        public void ResetToDefault()
        {
            playerCountSlider.value = _defaultPlayerCount;
            playerCountText.text = _defaultPlayerCount.ToString();
            HasChanged = false;
        }

        public void MarkAsNotChanged()
        {
            HasChanged = false;
        }
    }
}
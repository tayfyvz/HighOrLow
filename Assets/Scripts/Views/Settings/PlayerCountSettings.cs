using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Views
{
    public class PlayerCountSettings : MonoBehaviour
    {
        [Header("Player Count UI")]
        [SerializeField] private Slider playerCountSlider;
        [SerializeField] private TMP_Text playerCountText;

        private int defaultPlayerCount = 2;
        
        // Flag that is set to true if the user changes the slider.
        public bool HasChanged { get; private set; } = false;
        
        private void Awake()
        {
            playerCountSlider.wholeNumbers = true;
        }
        
        /// <summary>
        /// Initializes the UI to the provided default count.
        /// </summary>
        public void Initialize(int defaultCount)
        {
            defaultPlayerCount = defaultCount;
            playerCountSlider.minValue = 2;
            playerCountSlider.maxValue = 4;
            playerCountSlider.value = defaultPlayerCount;
            playerCountText.text = defaultPlayerCount.ToString();
            HasChanged = false;
            
            playerCountSlider.onValueChanged.RemoveAllListeners();
            playerCountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        private void OnSliderValueChanged(float value)
        {
            int count = (int)value;
            playerCountText.text = count.ToString();
            
            // Mark as changed if the value does not equal the default.
            HasChanged = (count != defaultPlayerCount);
        }
        
        /// <summary>
        /// Returns the current player count.
        /// </summary>
        public int GetCurrentPlayerCount()
        {
            return (int)playerCountSlider.value;
        }
        
        /// <summary>
        /// Resets the UI to the default player count.
        /// </summary>
        public void ResetToDefault()
        {
            playerCountSlider.value = defaultPlayerCount;
            playerCountText.text = defaultPlayerCount.ToString();
            HasChanged = false;
        }
        
        /// <summary>
        /// Clears the change flag.
        /// </summary>
        public void MarkAsNotChanged()
        {
            HasChanged = false;
        }
    }
}

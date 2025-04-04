using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class BetButtonView : MonoBehaviour, IView
    {
        public Transform buttonTransform;
        public Button betButton;
        public CanvasGroup canvasGroup;

        private GameObject _gameObject;
        
        private void Awake()
        {
            buttonTransform = transform;
            _gameObject = gameObject;
        }

        public void SetInteractable(bool interactable)
        {
            betButton.interactable = interactable;
        }

        public void SetActive(bool active)
        {
            _gameObject.SetActive(active);
        }
    }
}
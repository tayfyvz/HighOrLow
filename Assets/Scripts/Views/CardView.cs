using UnityEngine;
using UnityEngine.UI;
using Logger = Utils.Logger;

namespace Views
{
    public class CardView : MonoBehaviour, IView
    {
        [SerializeField] private Image _image;

        private Transform _transform;
        private GameObject _gameObject;
        private Sprite _frontSprite;
        private Sprite _backSprite;

        private void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        public void SetPosition(Vector2 deckCardsPosition)
        {
            _transform.position = deckCardsPosition;
        }

        public void SetActive(bool active)
        {
            _gameObject.SetActive(active);
        }

        public void SetBackSprite(Sprite cardSprite)
        {
            if (cardSprite == null)
            {
                Logger.Log("No Card Sprite provided for back side.", LogType.Error);
                return;
            }
            _backSprite = cardSprite;
        }

        public void SetFrontSprite(Sprite cardSprite)
        {
            if (cardSprite == null)
            {
                Logger.Log("No Card Sprite provided for front side.", LogType.Error);
                return;
            }
            _frontSprite = cardSprite;
        }

        public void ShowFront()
        {
            if (_image == null) return;
            _image.sprite = _frontSprite;
        }

        public void ShowBack()
        {
            if (_image == null) return;
            _image.sprite = _backSprite;
        }
    }
}
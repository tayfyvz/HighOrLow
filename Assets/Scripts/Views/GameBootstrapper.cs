using Controllers;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Utils.AddressableLoaders;
using Transform = UnityEngine.Transform;

namespace Views
{
    public class GameBootstrapper : MonoBehaviour
    {
        [Header("Game Session Manager")]
        [SerializeField] private GameSessionManager _gameSessionManager;
        
        [Header("View References")]
        [SerializeField] private DeckView _deckView;
        [SerializeField] private BetView _betView;

        [Header("Player References")]
        [SerializeField] private Transform _playerViewsContainer;
        [SerializeField] private Transform[] _playerSitPoints;
        
        private IGameController _gameController;
        private PlayerLoader _playerViewLoader;
        private PlayerAtlasLoader _playerAtlasLoader;

        private void Start()
        {
            _ = Initialize();
        }

        private void OnDestroy()
        {
            if (_playerViewLoader == null)
            {
                Debug.LogWarning("PlayerViewLoader is null in OnDestroy.");
                return;
            }

            Debug.Log("Releasing PlayerView instances...");
            _playerViewLoader.Release();
            _playerViewLoader = null;

            if (_playerAtlasLoader != null)
            {
                _playerAtlasLoader.Release();
                _playerAtlasLoader = null;
            }
        }

        private async UniTask Initialize()
        {
            int playerCount = SettingsManager.Instance.GetPlayerCount();

            PlayerView[] playerViews = new PlayerView[playerCount];
            _playerViewLoader = new PlayerLoader();
            _playerAtlasLoader = new PlayerAtlasLoader();

            // Load the player atlas
            await _playerAtlasLoader.LoadAsync();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerView playerView = await _playerViewLoader.LoadAsync();
                
                if (playerView != null)
                {
                    playerView.transform.SetParent(_playerViewsContainer);

                    // Assign the correct sprite based on player ID
                    Sprite playerSprite = _playerAtlasLoader.GetPlayerSprite(i);
                    if (playerSprite != null)
                    {
                        playerView.SetSprite(playerSprite); // Assume PlayerView has a SetSprite method
                    }
                }

                playerViews[i] = playerView;
            }
            
            Vector2[] playerSitPoints = new Vector2[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                playerSitPoints[i] = _playerSitPoints[i].position;
            }

            IAnimationController animationController = new AnimationController(playerViews, _deckView, _betView);
            
            _gameController = new GameController(_gameSessionManager, animationController);
            
            _gameController.PassView(playerViews, playerSitPoints);
            _gameController.PassView(_deckView);
            _gameController.PassView(_betView);
            _gameController.InitializeBetSystem();
            
            _gameSessionManager.Initialize(_gameController);
        }
    }
}

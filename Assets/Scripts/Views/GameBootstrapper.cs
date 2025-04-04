using Controllers;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Utils.AddressableLoaders;
using Logger = Utils.Logger;

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
        private PlayerPrefabLoader _playerPrefabViewLoader;
        private PlayerAtlasLoader _playerAtlasLoader;

        private void Awake() => _ = Initialize();

        private void OnDestroy()
        {
            if (_playerPrefabViewLoader == null)
            {
                Logger.Log("PlayerViewLoader is null in OnDestroy.", LogType.Warning);
                return;
            }

            Logger.Log("Releasing PlayerView instances...", LogType.Log);
            _playerPrefabViewLoader.Release();
            _playerPrefabViewLoader = null;

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
            _playerPrefabViewLoader = new PlayerPrefabLoader();
            _playerAtlasLoader = new PlayerAtlasLoader();

            await _playerAtlasLoader.LoadAsync();

            for (int i = 0; i < playerCount; i++)
            {
                PlayerView playerView = await _playerPrefabViewLoader.LoadAsync();
                
                if (playerView != null)
                {
                    playerView.transform.SetParent(_playerViewsContainer);
                    Sprite playerSprite = _playerAtlasLoader.GetPlayerSprite(i);
                    if (playerSprite != null) playerView.SetSprite(playerSprite);
                }

                playerViews[i] = playerView;
            }
            
            Vector2[] playerSitPoints = new Vector2[playerCount];
            for (int i = 0; i < playerCount; i++) playerSitPoints[i] = _playerSitPoints[i].position;

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

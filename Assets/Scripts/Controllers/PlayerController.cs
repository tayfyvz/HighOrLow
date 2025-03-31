using System;
using System.Collections.Generic;
using Models;
using Views;

namespace Controllers
{
    /// <summary>
    /// Controls a player's behavior by wrapping the Player model and coordinating with the view.
    /// </summary>
    public class PlayerController : BaseController<IPlayerView>, IPlayerController
    {
        private readonly List<Player> _players = new List<Player>();
        private readonly Dictionary<int, List<IPlayerView>> _playerViews = new Dictionary<int, List<IPlayerView>>();

        /// <summary>
        /// Returns the number of players being controlled.
        /// </summary>
        public int PlayerCount => _players.Count;

        /// <summary>
        /// Adds a new player and registers an initial view for that player.
        /// </summary>
        public void AddPlayer(Player player, IPlayerView view)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _players.Add(player);
            if (!_playerViews.ContainsKey(player.Id))
                _playerViews[player.Id] = new List<IPlayerView>();

            _playerViews[player.Id].Add(view);
            // Update the view with the initial state.
            view.UpdateView(player);
        }

        public int GetPlayerCount()
        {
            return _players.Count;
        }
        /// <summary>
        /// Registers an additional view for the specified player.
        /// </summary>
        public void RegisterView(int playerId, IPlayerView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (!_playerViews.ContainsKey(playerId))
            {
                _playerViews[playerId] = new List<IPlayerView>();
            }

            if (!_playerViews[playerId].Contains(view))
            {
                _playerViews[playerId].Add(view);
                // Immediately update the new view with the current player state.
                var player = _players.Find(p => p.Id == playerId);
                if (player != null)
                {
                    view.UpdateView(player);
                }
            }
        }

        /// <summary>
        /// Removes a registered view for the specified player.
        /// </summary>
        public void RemoveView(int playerId, IPlayerView view)
        {
            if (view == null)
                return;
            if (_playerViews.ContainsKey(playerId))
            {
                _playerViews[playerId].Remove(view);
            }
        }

        /// <summary>
        /// Adds a card to the specified player's hand and updates that player's views.
        /// </summary>
        public void ReceiveCard(int playerId, Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            Player player = _players.Find(p => p.Id == playerId);
            if (player == null)
                throw new Exception($"Player with ID {playerId} not found.");

            player.AddCard(card);
            UpdatePlayerViews(player);
        }

        /// <summary>
        /// Updates all registered views of the given player.
        /// </summary>
        private void UpdatePlayerViews(Player player)
        {
            if (player == null) return;
            if (_playerViews.TryGetValue(player.Id, out var views))
            {
                foreach (var view in views)
                {
                    view.UpdateView(player);
                }
            }
        }

        protected override void UpdateView()
        {
            throw new NotImplementedException();
        }

        public void RemoveView(IPlayerView view)
        {
            throw new NotImplementedException();
        }

        public void ReceiveCard(Card card)
        {
            throw new NotImplementedException();
        }
    }
}
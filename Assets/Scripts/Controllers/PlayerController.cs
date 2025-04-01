using System;
using System.Collections.Generic;
using Models;
using UnityEngine;
using Views;

namespace Controllers
{
    /// <summary>
    /// Controls a player's behavior by wrapping the Player model and coordinating with the view.
    /// </summary>
    public class PlayerController : BaseController<IPlayerView>, IPlayerController
    {
        private readonly List<Player> _players = new List<Player>();
        private readonly Dictionary<int, IPlayerView> _playerViews = new Dictionary<int,IPlayerView>();

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
            {
                _playerViews[player.Id] = view;
            }

            view.SetPlayerName(player.Name);
        }

        public void SetPlayersPosition(Vector2[] positions)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_playerViews.TryGetValue(i, out var playerView))
                {
                    playerView.SetPosition(positions[i]);
                }
            }
        }

        public void MarkRoundWinner(int winningIndex)
        {
            if (_players.Count <= winningIndex)
            {
                return;
            }
            
            Player player = _players[winningIndex];
            player.AddScore(1);
            
            if (_playerViews.TryGetValue(winningIndex, out var playerView))
            {
                playerView.SetScore(player.Score);
            }
        }

        public void MarkSessionWinner()
        {
            int maxScore = 0;
            int winnerIndex = -1;
            foreach (Player player in _players)
            {
                if (player.Score > maxScore)
                {
                    maxScore = player.Score;
                    winnerIndex = player.Id;
                }
            }

            if (_playerViews.TryGetValue(winnerIndex, out var playerView))
            {
                playerView.WinSession();
            }
        }

        public void ResetPlayers()
        {
            foreach (Player player in _players)
            {
                player.Reset();
                int id = player.Id;

                if (_playerViews.TryGetValue(id, out var playerView))
                {
                    playerView.ResetView();
                }
                
            }
        }

        public int GetPlayerCount()
        {
            return _players.Count;
        }
        
        public void ReceiveCard(int playerId, Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            Player player = _players.Find(p => p.Id == playerId);
            if (player == null)
                throw new Exception($"Player with ID {playerId} not found.");

            player.AddCard(card);
            UpdatePlayerView(player);
        }
        
        private void UpdatePlayerView(Player player)
        {
            if (player == null)
            {
                return;
            }
            
            if (!_playerViews.TryGetValue(player.Id, out var view))
            {
                return;
            }
            
            Card lastCard = player.GetLastCard();
            view.UpdateHand(lastCard);
        }

        protected override void UpdateView()
        {
            throw new NotImplementedException();
        }
    }
}
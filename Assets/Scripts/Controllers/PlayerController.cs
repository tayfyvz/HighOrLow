using System;
using System.Collections.Generic;
using Models;
using Views;

namespace Controllers
{
    /// <summary>
    /// Controls a player's behavior by wrapping the Player model and coordinating with the view.
    /// </summary>
    public class PlayerController
    {
        private readonly Player _player;
        private readonly List<PlayerView> _playerViews;

        public PlayerController(Player player)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _playerViews = new List<PlayerView>();
        }
        
        public void RegisterView(PlayerView view)
        {
            if (view != null && !_playerViews.Contains(view))
            {
                _playerViews.Add(view);
                //view.UpdateView(PlayerModel);
            }
        }

        public void UnregisterView(PlayerView view)
        {
            if (view != null)
            {
                _playerViews.Remove(view);
            }
        }
        
        public void ReceiveCard(Card card)
        {
            _player.AddCard(card);
            UpdateViews();
        }

        private void UpdateViews()
        {
            foreach (var view in _playerViews)
            {
                //view.UpdateView(_player);
            }
        }
    }
}
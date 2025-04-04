using System.Collections.Generic;
using Models;

namespace Managers
{
    namespace Managers
    {
    }

    public class SettingsManager
    {
        private static SettingsManager _instance;
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsManager();
                }
                return _instance;
            }
        }
        
        private readonly Settings _settings = new();

        public void SetSettings(int numOfPlayers, float defaultWeight, List<SuitOverrideData> suitOverrides, List<CardOverrideData> cardOverrides)
        {
            _settings.Set(numOfPlayers, defaultWeight, suitOverrides, cardOverrides);
        }

        public int GetPlayerCount()
        {
            int numOfPlayer = _settings.NumberOfPlayer;

            return numOfPlayer switch
            {
                < 2 => 2,
                > 4 => 4,
                _ => numOfPlayer
            };
        }

        public float GetCardWeight(Suits suit, Ranks rank)
        {
            return _settings.GetCardWeight(suit, rank);
        }
    }
}
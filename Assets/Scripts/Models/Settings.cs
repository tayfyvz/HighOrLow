using System.Collections.Generic;

namespace Models
{
    public class Settings
    {
        public int NumberOfPlayer => _numOfPlayers;
        
        private int _numOfPlayers;
        private float _defaultWeight;

        private List<SuitOverrideData> _suitOverrides;
        private List<CardOverrideData> _cardOverrides;
        
        private Dictionary<Suits, float> _suitOverridesDict;
        private Dictionary<Suits, Dictionary<Ranks, float>> _cardOverridesDict;
        
        public float GetCardWeight(Suits suit, Ranks rank)
        {
            if (_cardOverridesDict != null && 
                _cardOverridesDict.TryGetValue(suit, out Dictionary<Ranks, float> rankOverrides) &&
                rankOverrides != null &&
                rankOverrides.TryGetValue(rank, out float weight))
            {
                return weight;
            }
            if (_suitOverridesDict != null && 
                _suitOverridesDict.TryGetValue(suit, out weight))
            {
                return weight;
            }

            return _defaultWeight;
        }

        public void Set(int numOfPlayers, float defaultWeight, List<SuitOverrideData> suitOverrides, List<CardOverrideData> cardOverrides)
        {
            _numOfPlayers = numOfPlayers;
            _defaultWeight = defaultWeight;
            _suitOverrides = suitOverrides;
            _cardOverrides = cardOverrides;
            
            BuildDictionaries();
        }
        
        private void BuildDictionaries()
        {
            _suitOverridesDict = new Dictionary<Suits, float>();
            foreach (SuitOverrideData data in _suitOverrides)
            {
                _suitOverridesDict[data.Suit] = data.Weight;
            }

            _cardOverridesDict = new Dictionary<Suits, Dictionary<Ranks, float>>();
            foreach (CardOverrideData data in _cardOverrides)
            {
                if (!_cardOverridesDict.TryGetValue(data.Suit, out Dictionary<Ranks, float> rankDict))
                {
                    rankDict = new Dictionary<Ranks, float>();
                    _cardOverridesDict[data.Suit] = rankDict;
                }
                rankDict[data.Rank] = data.Weight;
            }
        }
    }
}
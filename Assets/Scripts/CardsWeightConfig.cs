using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SuitOverrideData
{
    public Suits Suit;
    public float Weight;
}

[Serializable]
public struct CardOverrideData
{
    public Suits Suit;
    public Ranks Rank;
    public float Weight;
}

[CreateAssetMenu(fileName = "Weights", menuName = "Configs/CardsWeights", order = 1)]
public class CardsWeightConfig : ScriptableObject
{
    [Header("Default Weight")]
    [SerializeField] private float _defaultWeight = 1f;

    [Header("Override by Suit")]
    [SerializeField] private List<SuitOverrideData> _suitOverrides;

    [Header("Override by Specific Card")]
    [SerializeField] private List<CardOverrideData> _cardOverrides;

    private Dictionary<Suits, float> _suitOverridesDict;
    private Dictionary<Suits, Dictionary<Ranks, float>> _cardOverridesDict;

    private void OnEnable()
    {
        BuildDictionaries();
    }

    private void BuildDictionaries()
    {
        _suitOverridesDict = new Dictionary<Suits, float>();
        foreach (SuitOverrideData data in _suitOverrides)
        {
            // If there are duplicate suit overrides, this will replace earlier entries.
            _suitOverridesDict[data.Suit] = data.Weight;
        }

        _cardOverridesDict = new Dictionary<Suits, Dictionary<Ranks, float>>();
        foreach (CardOverrideData data in _cardOverrides)
        {
            // If no dictionary exists for this suit yet, create one.
            if (!_cardOverridesDict.TryGetValue(data.Suit, out Dictionary<Ranks, float> rankDict))
            {
                rankDict = new Dictionary<Ranks, float>();
                _cardOverridesDict[data.Suit] = rankDict;
            }
            rankDict[data.Rank] = data.Weight;
        }
    }

    public float GetCardWeight(Suits suit, Ranks rank)
    {
        // Check for a card specific override.
        if (_cardOverridesDict != null && 
            _cardOverridesDict.TryGetValue(suit, out Dictionary<Ranks, float> rankOverrides) &&
            rankOverrides != null &&
            rankOverrides.TryGetValue(rank, out float weight))
        {
            return weight;
        }
        // Check for a suit override.
        if (_suitOverridesDict != null && 
            _suitOverridesDict.TryGetValue(suit, out weight))
        {
            return weight;
        }

        return _defaultWeight;
    }
}


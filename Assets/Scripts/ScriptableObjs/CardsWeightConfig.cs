using System;
using System.Collections.Generic;
using Models;
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
    public float DefaultWeight = 1f;

    [Header("Override by Suit")]
    public  List<SuitOverrideData> SuitOverrides = new List<SuitOverrideData>();

    [Header("Override by Specific Card")]
    public List<CardOverrideData> CardOverrides = new List<CardOverrideData>();
}


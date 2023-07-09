using Assets.Native;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultySetting : ScriptableObject
{
    public ChunkTypes chunkType;

    public List<Transform> platformPrefabs;
    public List<float> platformWeights;

    public List<Transform> powerUpPrefabs;
    public List<Vector2> powerUpAmountMinMax;

    public int platformCount;
}

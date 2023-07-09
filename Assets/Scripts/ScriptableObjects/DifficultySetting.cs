using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DifficultySetting : ScriptableObject
{
    public List<Transform> platformPrefabs;
    public List<Transform> powerUpPrefabs;
}

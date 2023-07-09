using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Native;
using UnityEngine;

public class ContentChunk : MonoBehaviour
{
    Vector3 chunkCenter;
    Transform cloudPrefab;
    public float minX = 0.0f;
    public float maxX = 0.0f;
    public float minY = 0.0f;
    public float maxY = 0.0f;
    List<Transform> chunkObjects = new List<Transform>();
    DifficultySetting settings;

    void Start()
    {
        switch (settings.chunkType)
        {
            case ChunkTypes.JustBlocks:
                this.GenerateChunkTypeJustBlocks();
                break;
            default:
                throw new NotImplementedException();
        }

        GenerateClouds();
    }

    public void GenerateChunkTypeJustBlocks()
    {
        var totalWeight = settings.platformWeights.Sum();
        var probabilityArray = settings.platformWeights.Select(weight => weight / totalWeight).ToList();

        for (var i = 0; i < 10; i++)
        {
            Transform selectedPrefab = settings.platformPrefabs[0];
            var prefabDice = UnityEngine.Random.Range(0f, 1f);
            for (var j = 0; j < probabilityArray.Count; j++)
            {
                if (prefabDice < probabilityArray[j])
                {
                    selectedPrefab = settings.platformPrefabs[j];
                }
            }

            var x = float.NaN;
            var y = float.NaN;
            var collides = false;

            x = UnityEngine.Random.Range(minX, maxX);
            y = UnityEngine.Random.Range(minY, maxY);

            // 5 tentativas de colocar a plataforma
            for (var j = 0; j < 5; j++)
            {
                x = UnityEngine.Random.Range(minX, maxX);
                y = UnityEngine.Random.Range(minY, maxY);

                collides = false;

                foreach (var chunkObject in chunkObjects)
                {
                    if (Platform.Intersects(chunkObject.position, new Vector3(x, y, 0)))
                    {
                        collides = true;
                    }
                }

                if (!collides)
                {
                    var newPlat = Instantiate(selectedPrefab, transform);
                    newPlat.transform.position = new Vector3(x, y, 0);
                    chunkObjects.Add(newPlat.transform);
                    break;
                }
            }
        }
    }

    public void GenerateClouds()
    {

        for (var i = 0; i < Constants.CloudCountPerChunk / 2; i++)
        {
            var minOffset = Constants.PlayableAreaHeight / 2.0f + Constants.CloudPlayableAreaDistance;
            GenerateCloud(minOffset, minOffset + 30);
        }

        for (var i = 0; i < Constants.CloudCountPerChunk / 2; i++)
        {
            var maxOffset = -Constants.PlayableAreaHeight / 2.0f - Constants.CloudPlayableAreaDistance;
            GenerateCloud(maxOffset - 30, maxOffset);
        }
    }

    public void GenerateCloud(float minYOffset, float maxYOffset)
    {
        var x = UnityEngine.Random.Range(minX, maxX);
        var y = x * Constants.PlayableAreaInclineFactor + UnityEngine.Random.Range(minYOffset, maxYOffset);

        var newCloud = Instantiate(this.cloudPrefab, transform);
        newCloud.position = new Vector3(x, y, 0);
    }

    public void InitializeParams(
        Vector3 chunkCenter,
        Transform cloudPrefab,
        DifficultySetting difficultySetting)
    {
        this.chunkCenter = chunkCenter;
        this.cloudPrefab = cloudPrefab;
        this.settings = difficultySetting;

        minX = chunkCenter.x - Constants.ChunkWidth / 2;
        maxX = chunkCenter.x + Constants.ChunkWidth / 5;
        minY = Constants.ChunkMinY + Utilities.GetYCenterAt(chunkCenter.x);
        maxY = Constants.ChunkMaxY + Utilities.GetYCenterAt(chunkCenter.x);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Native;
using UnityEngine;

public class ContentChunk : MonoBehaviour
{
    [SerializeField] MovingPlatform movingPlatformPrefab;
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
            case ChunkTypes.CirculatingPlatforms:
                this.GenerateChunkTypeCirculatingPlatforms();
                break;
            default:
                throw new NotImplementedException();
        }

        GenerateClouds();
    }

    public void GenerateChunkTypeCirculatingPlatforms()
    {
        var path = new List<Vector3>()
        {
            new Vector3(minX + 10, Utilities.GetYCenterAt(minX + 10) + Constants.PlayableAreaHeight / 2 - 5),
            new Vector3(maxX - 10, Utilities.GetYCenterAt(maxX - 10) + Constants.PlayableAreaHeight / 2 - 5),
            new Vector3(maxX - 10, Utilities.GetYCenterAt(maxX - 10) - Constants.PlayableAreaHeight / 2 + 5),
            new Vector3(minX + 10, Utilities.GetYCenterAt(minX + 10) - Constants.PlayableAreaHeight / 2 + 5),
            new Vector3(minX + 10, Utilities.GetYCenterAt(minX + 10) + Constants.PlayableAreaHeight / 2 - 5),
        };

        var movingPlatforms = settings.platformCount;

        for (int i = 0; i < movingPlatforms; i++)
        {
            var newPlatform = Instantiate(movingPlatformPrefab, transform);
            newPlatform.InitializeParams(path, 12, (float)i / (float)movingPlatforms);
        }
    }

    public void GenerateChunkTypeJustBlocks()
    {
        var totalWeight = settings.platformWeights.Sum();
        var probabilityArray = settings.platformWeights.Select(weight => weight / totalWeight).ToList();

        for (var i = 0; i < settings.platformCount; i++)
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

            var collides = false;

            // 5 tentativas de colocar a plataforma
            for (var j = 0; j < 5; j++)
            {
                var pos = RandomPosInChunk();
                collides = false;

                foreach (var chunkObject in chunkObjects)
                {
                    if (Platform.Intersects(chunkObject.position, pos))
                    {
                        collides = true;
                    }
                }

                if (!collides)
                {
                    var newPlat = Instantiate(selectedPrefab, transform);
                    newPlat.transform.position = pos;
                    chunkObjects.Add(newPlat.transform);
                    break;
                }
            }
        }

        GeneratePowerups();
    }

    public void GeneratePowerups()
    {
        for (int i = 0; i < settings.powerUpAmountMinMax.Count; i++)
        {
            var countRange = settings.powerUpAmountMinMax[i];
            var powerupCount = (int)UnityEngine.Random.Range(countRange.x, (float)(Math.Floor((double)countRange.y + 1) - 0.01f));

            for (var j = 0; j < powerupCount; j++)
            {
                var powerup = Instantiate(settings.powerUpPrefabs[i], transform);
                powerup.position = RandomPosInChunk();
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

    public Vector3 RandomPosInChunk()
    {
        var x = UnityEngine.Random.Range(minX, maxX);
        var y = UnityEngine.Random.Range(-Constants.PlayableAreaHeight / 2.0f, Constants.PlayableAreaHeight / 2.0f) + Utilities.GetYCenterAt(x);
        return new Vector3(x, y);
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
        maxX = chunkCenter.x + Constants.ChunkWidth / 2;
        minY = Constants.ChunkMinY + Utilities.GetYCenterAt(chunkCenter.x);
        maxY = Constants.ChunkMaxY + Utilities.GetYCenterAt(chunkCenter.x);
    }
}

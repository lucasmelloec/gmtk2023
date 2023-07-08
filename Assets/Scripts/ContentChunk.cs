using Assets.Native;
using System.Collections.Generic;
using UnityEngine;

public class ContentChunk : MonoBehaviour
{
    Vector3 chunkCenter;
    Platform platformPrefab;
    public float minX = 0.0f;
    public float maxX = 0.0f;
    public float minY = 0.0f;
    public float maxY = 0.0f;
    List<Transform> chunkObjects = new List<Transform>();

    void Start()
    {
        for (var i = 0; i < 10; i++)
        {
            var x = float.NaN;
            var y = float.NaN;
            var collides = false;

            x = Random.Range(minX, maxX);
            y = Random.Range(minY, maxY);

            // 5 tentativas de colocar a plataforma
            for (var j = 0; j < 5; j++)
            {
                x = Random.Range(minX, maxX);
                y = Random.Range(minY, maxY);

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
                    var newPlat = Instantiate(platformPrefab, transform);
                    newPlat.transform.position = new Vector3(x, y, 0);
                    chunkObjects.Add(newPlat.transform);
                    break;
                }
            }
        }
    }

    public void InitializeParams(Vector3 chunkCenter, Platform platformPrefab)
    {
        this.chunkCenter = chunkCenter;
        this.platformPrefab = platformPrefab;

        minX = chunkCenter.x - Constants.ChunkWidth / 2;
        maxX = chunkCenter.x + Constants.ChunkWidth / 5;
        minY = Constants.ChunkMinY + chunkCenter.x / 2;
        maxY = Constants.ChunkMaxY + chunkCenter.x / 2;
    }
}

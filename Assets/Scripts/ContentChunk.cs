using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentChunk : MonoBehaviour
{
    Vector3 chunkCenter;
    Transform customCamera;
    Platform platformPrefab;
    public float minX = 0.0f;
    public float maxX = 0.0f;
    public float minY = 0.0f;
    public float maxY = 0.0f;
    List<Transform> chunkObjects; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeParams(Vector3 chunkCenter, Transform customCamera, Platform platformPrefab)
    {
        this.chunkCenter = chunkCenter;
        this.customCamera = customCamera;
        this.platformPrefab = platformPrefab;

        minX = chunkCenter.x - Constants.ChunkWidth / 2;
        maxX = chunkCenter.x + Constants.ChunkWidth / 2;
        minY = Constants.ChunkMinY;
        maxY = Constants.ChunkMaxY;

        StartCoroutine(Populate());
    }

    public IEnumerator Populate()
    {
        yield return null;

        for (var i = 0; i < 10; i++)
        {
            var x = Random.Range(minX, maxX);
            var y = Random.Range(minY, maxY);

            var newPlat = Instantiate(platformPrefab, transform);
            newPlat.transform.position = new Vector3(x, y, 0);
        }
    }
}

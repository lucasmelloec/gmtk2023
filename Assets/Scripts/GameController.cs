using System.Collections.Generic;
using UnityEngine;
using Assets.Native;

public class GameController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ContentChunk contentChunkPrefab;
    [SerializeField] private Platform platformPrefab;
    [SerializeField] private FallingPlatform fallingPlatformPrefab;
    [SerializeField] private MovingPlatform movingPlatformPrefab;
    [SerializeField] private Transform cloudPrefab;

    private float leftmostContentX = -10f;
    private const float contentBufferWidthX = 30;
    private const float yUpperBound = 20;
    private const float yLowerBound = -20;
    private CameraController cameraController;

    private static Queue<ContentChunk> contentChunks = new Queue<ContentChunk>();

    void Start() {
        cameraController = CameraController.singleton;

        var tempPlat = Instantiate(movingPlatformPrefab);
        tempPlat.InitializeParams(new List<Vector3>()
        {
            new Vector3(10, -5, 0),
            new Vector3(-5, -5, 0),
            new Vector3(-100, -50, 0)
        },
        5);
    }

    void Update()
    {
        PopulateWithContent();
    }

    void PopulateWithContent()
    {
        while (GetCameraBoundStart().x < leftmostContentX + Constants.ChunkWidth)
        {
            var newCenter = new Vector3(leftmostContentX - Constants.ChunkWidth / 2, 0);
            var newChunk = Instantiate(contentChunkPrefab);
            newChunk.InitializeParams(newCenter, platformPrefab, fallingPlatformPrefab, cloudPrefab, Utilities.Clamp((-newCenter.x)/150, 0f, 0.6f));

            leftmostContentX = newChunk.minX;
            contentChunks.Enqueue(newChunk);
        }

        if (contentChunks.Count > 0)
        {
            var oldestChunk = contentChunks.Peek();

            while (oldestChunk.minX - 60 > GetCameraBoundEnd().x)
            {
                contentChunks.Dequeue();
                Destroy(oldestChunk.gameObject);

                if (contentChunks.Count == 0) break;
                oldestChunk = contentChunks.Peek();
            }
        }
    }

    Vector3 GetCameraBoundStart()
    {
        // set up correct bound later;
          return cameraController.GetCurrentPosition() - new Vector3(cameraController.cameraWidth / 2, -cameraController.cameraHeight / 2);
    }

    Vector3 GetCameraBoundEnd()
    {
        // set up correct bound later;
        return cameraController.GetCurrentPosition() + new Vector3(cameraController.cameraWidth / 2, -cameraController.cameraHeight / 2);
    }
}

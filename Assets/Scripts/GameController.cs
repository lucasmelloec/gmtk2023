using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private ContentChunk contentChunkPrefab;
    [SerializeField] private Platform platformPrefab;

    private float leftmostContentX = -10f;

    private const float contentBufferWidthX = 30;
    private const float yUpperBound = 20;
    private const float yLowerBound = -20;

    private static Queue<ContentChunk> contentChunks = new Queue<ContentChunk>();

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
            newChunk.InitializeParams(newCenter, platformPrefab);

            leftmostContentX = newChunk.minX;
            contentChunks.Enqueue(newChunk);
        }

        if (contentChunks.Count > 0)
        {
            var oldestChunk = contentChunks.Peek();

            while (oldestChunk.minX > GetCameraBoundEnd().x)
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
          return CameraController.singleton.GetCurrentPosition() - new Vector3(21.5f, -10);
    }

    Vector3 GetCameraBoundEnd()
    {
        // set up correct bound later;
        return CameraController.singleton.GetCurrentPosition() + new Vector3(21.5f, -10);
    }
}

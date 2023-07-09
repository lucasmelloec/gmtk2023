using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Native;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    public enum State
    {
        GamePlaying,
        GameOver,
    }
    public static GameController singleton { get; private set; }
    public event Action<State> onGameStateChanged;

    [SerializeField] private Player player;
    [SerializeField] private ContentChunk contentChunkPrefab;
    [SerializeField] private Platform platformPrefab;
    [SerializeField] private FallingPlatform fallingPlatformPrefab;
    [SerializeField] private MovingPlatform movingPlatformPrefab;
    [SerializeField] private Transform cloudPrefab;
    [SerializeField] private Transform scoreCounter;

    [SerializeField] private List<DifficultySetting> chunks;

    private float leftmostContentX = -10f;
    private const float contentBufferWidthX = 30;
    private const float yUpperBound = 20;
    private const float yLowerBound = -20;
    float score = 0.0f;
    private CameraController cameraController;
    private State _gameState = State.GamePlaying;
    private State gameState
    {
        get => _gameState;
        set
        {
            _gameState = value;
            onGameStateChanged?.Invoke(_gameState);
        }
    }

    private static Queue<ContentChunk> contentChunks = new Queue<ContentChunk>();

    void Awake()
    {
        Assert.IsNull(singleton);
        singleton = this;
    }

    void Start()
    {
        cameraController = CameraController.singleton;

        var tempPlat = Instantiate(movingPlatformPrefab);
        tempPlat.InitializeParams(new List<Vector3>()
        {
            new Vector3(10, -5, 0),
            new Vector3(-5, -5, 0),
            new Vector3(-100, -50, 0)
        },
        5, 0.0f);
    }

    void Update()
    {
        switch (gameState)
        {
            case State.GamePlaying:
                PopulateWithContent();
                UpdateScore();
                if (player.isArrested || player.isDead)
                {
                    Time.timeScale = 0f;
                    gameState = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    void UpdateScore()
    {
        var currentScore = Math.Abs(player.transform.position.x);
        currentScore = (float)(int)currentScore;

        if (currentScore > score)
        {
            score = currentScore;
        }

        var textMesh = scoreCounter.GetComponent<TextMeshProUGUI>();
        textMesh.text = $"Score: {score}";
    }

    public DifficultySetting SelectChunk(float x)
    {
        x = -x;

        var cumulativeProbabilities = new List<float>(chunks.Count);
        for (int i = 0; i < chunks.Count; i++)
        {
            var currentWeight = GetChunkWeight(chunks[i], x);
            cumulativeProbabilities.Add(currentWeight);
        }

        var totalWeight = cumulativeProbabilities.Sum();
        var runningProbability = 0.0f;
        for (int i = 0; i < chunks.Count; i++)
        {
            runningProbability += cumulativeProbabilities[i] / totalWeight;
            cumulativeProbabilities[i] = runningProbability;
        }


        var dice = UnityEngine.Random.Range(0f, 1f);
        for (int i = 0; i < chunks.Count; i++)
        {
            if (dice <= cumulativeProbabilities[i])
            {
                return chunks[i];
            }
        }

        return chunks.Last();
    }

    public float GetChunkWeight(DifficultySetting chunk, float x)
    {
        if (chunk.weightSequence.Count == 0) return 0.0f;
        if (x > chunk.weightSequence.Last().x) return chunk.weightSequence.Last().y;

        for (int i = chunk.weightSequence.Count - 2; i >= 0; i--)
        {
            var currentPoint = chunk.weightSequence[i];
            var nextPoint = chunk.weightSequence[i + 1];
            if (x >= currentPoint.x)
            {
                return (x - currentPoint.x) / (nextPoint.x - currentPoint.x) * (nextPoint.y - currentPoint.y) + currentPoint.y;
            }
        }

        return 0.0f;
    }

    void PopulateWithContent()
    {
        while (GetCameraBoundStart().x < leftmostContentX + Constants.ChunkWidth)
        {
            var newCenter = new Vector3(leftmostContentX - Constants.ChunkWidth / 2, 0);
            var newChunk = Instantiate(contentChunkPrefab);
            var selectedDifficulty = SelectChunk(newCenter.x);
            Debug.Log($"Selected a {selectedDifficulty.name}");
            newChunk.InitializeParams(newCenter, cloudPrefab, selectedDifficulty);

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

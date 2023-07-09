using System;
using System.Collections.Generic;
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

    [SerializeField] private DifficultySetting basicChunk;

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
        5);
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

    void PopulateWithContent()
    {
        while (GetCameraBoundStart().x < leftmostContentX + Constants.ChunkWidth)
        {
            var newCenter = new Vector3(leftmostContentX - Constants.ChunkWidth / 2, 0);
            var newChunk = Instantiate(contentChunkPrefab);
            newChunk.InitializeParams(newCenter, cloudPrefab, basicChunk);

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

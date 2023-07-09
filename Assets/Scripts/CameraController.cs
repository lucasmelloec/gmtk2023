using System;
using System.Collections;
using Assets.Native;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public static CameraController singleton { get; private set; }
    public float cameraWidth { get; private set; }
    public float cameraHeight { get; private set; }
    public Vector2 position { get => transform.position; }

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float autoMoveCameraTimer = 2f;
    [SerializeField] private float maxCameraSpeed = 15f;
    [SerializeField] private float cameraSpeedupFactor = 0.05f;
    private float leftmostCamCenterX = 0.0f;
    private const float fixedCamZ = -10;
    private bool autoMoveCamera;
    private DateTime gameStartTime = DateTime.Now;

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    private void Awake()
    {
        Assert.IsNull(singleton);
        singleton = this;
        Camera mainCamera = Camera.main;
        cameraHeight = mainCamera.orthographicSize * 2;
        cameraWidth = mainCamera.aspect * mainCamera.orthographicSize * 2;
    }

    private void Start()
    {
        StartCoroutine(StartAutoCameraMovement());
    }

    private void LateUpdate()
    {
        var playerPos = playerTransform.position;
        if (playerPos.x < leftmostCamCenterX)
        {
            leftmostCamCenterX = playerPos.x;
        }
        if (autoMoveCamera)
        {
            var elapsedSeconds = (float)(DateTime.Now - gameStartTime).TotalSeconds;
            var dynamicCameraSpeed = Utilities.Clamp(cameraSpeed + cameraSpeedupFactor * elapsedSeconds, 0, maxCameraSpeed);
            leftmostCamCenterX -= dynamicCameraSpeed * Time.deltaTime;
        }

        var playableCenterY = Utilities.GetYCenterAt(leftmostCamCenterX);
        var yPosition = Utilities.Clamp(
            playerTransform.position.y,
            playableCenterY - Constants.PlayableAreaHeight / 2.0f,
            playableCenterY + Constants.PlayableAreaHeight / 2.0f);
        transform.position = new Vector3(leftmostCamCenterX, yPosition, fixedCamZ);
    }

    private IEnumerator StartAutoCameraMovement()
    {
        gameStartTime = DateTime.Now;
        yield return new WaitForSeconds(autoMoveCameraTimer);
        autoMoveCamera = true;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public static CameraController singleton { get; private set; }
    public float cameraWidth { get; private set; }
    public float cameraHeight { get; private set; }

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private float autoMoveCameraTimer = 2f;
    private float leftmostCamCenterX = 0.0f;
    private const float fixedCamZ = -10;
    private bool autoMoveCamera;

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
            leftmostCamCenterX -= cameraSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(leftmostCamCenterX, playerTransform.position.y, fixedCamZ);
    }

    private IEnumerator StartAutoCameraMovement()
    {
        yield return new WaitForSeconds(autoMoveCameraTimer);
        autoMoveCamera = true;
    }
}

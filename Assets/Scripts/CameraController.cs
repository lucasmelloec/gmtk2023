using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    public static CameraController singleton { get; set; }

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

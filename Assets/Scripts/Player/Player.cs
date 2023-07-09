using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player singleton { get; private set; }
    public event Action<bool> onIsGlidingChanged;
    public bool isArrested { get; private set; }
    public bool isDead { get; private set; }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float glidingFactor = 0.4f; // 1 = total glide, 0 = no glide
    [SerializeField] private float jumpSpeed = 9f;
    [SerializeField] private float terminalSpeed = 50f;
    [SerializeField] private float fallSpeed = 10f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask murderLayerMask;
    [SerializeField] private float coyoteTime = 0.3f;
    [SerializeField] private float perfectGlideTime = 0.5f;
    [SerializeField] private float taperedGlideTime = 2f;
    [SerializeField] private BoxCollider2D playerCollider;
    private PlayerInputActions playerInputActions;
    private Rigidbody2D rigidbody2d;
    private float storedGlideTime = 0.0f;
    private DateTime glideStartTime = DateTime.Now - TimeSpan.FromDays(1);
    private SpriteRenderer spriteRenderer;
    private float timeElapsedSinceGrounded;
    private bool isCurrentlyGrounded;
    private Vector2 inputVector;
    private bool _isGliding = false;
    private bool isGliding
    {
        get => _isGliding;
        set
        {
            _isGliding = value;
            onIsGlidingChanged?.Invoke(_isGliding);
        }
    }
    private CameraController cameraController;

    private void Awake()
    {
        Assert.IsNull(singleton);
        singleton = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        storedGlideTime = perfectGlideTime + taperedGlideTime;
    }

    private void Start()
    {
        cameraController = CameraController.singleton;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Glide.started += PlayerInputActions_Glide_Started;
        playerInputActions.Player.Glide.canceled += PlayerInputActions_Glide_Canceled;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleGlide();
    }

    private void Update()
    {
        inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        isCurrentlyGrounded = IsGrounded();
        if (isCurrentlyGrounded)
        {
            timeElapsedSinceGrounded = 0f;
            storedGlideTime = perfectGlideTime + taperedGlideTime;
        }
        else
        {
            timeElapsedSinceGrounded += Time.deltaTime;
        }
        transform.rotation = Quaternion.identity;
        HandleMurder();
        HandleDeath();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Glide.started -= PlayerInputActions_Glide_Started;
        playerInputActions.Player.Glide.canceled -= PlayerInputActions_Glide_Canceled;
    }

    private void HandleMovement()
    {
        Vector2 newVelocity = new Vector2(inputVector.x * moveSpeed, rigidbody2d.velocity.y);
        if (!isGliding && inputVector.y != 0 && !isCurrentlyGrounded)
        {
            newVelocity.y += inputVector.y * fallSpeed;
        }
        if (Mathf.Abs(newVelocity.y) > terminalSpeed)
        {
            newVelocity.y = newVelocity.y < 0 ? -terminalSpeed : terminalSpeed;
        }
        rigidbody2d.velocity = newVelocity;
    }

    private void HandleGlide()
    {
        if (!isGliding || rigidbody2d.velocity.y >= 0 || IsGrounded()) return;
        var now = DateTime.Now;
        var secondsGliding = (float)(now - glideStartTime).TotalSeconds;
        if (secondsGliding > perfectGlideTime + taperedGlideTime)
        {
            isGliding = false;
            storedGlideTime = 0f;
            return;
        }

        float dynamicGlideFactor;
        if (secondsGliding < perfectGlideTime)
        {
            dynamicGlideFactor = glidingFactor;
        }
        else
        {
            dynamicGlideFactor = (1.0f - (secondsGliding - perfectGlideTime) / taperedGlideTime) * glidingFactor;
        }
        Vector2 glidingVelocity = rigidbody2d.velocity;
        glidingVelocity.y *= (1.0f - dynamicGlideFactor);
        rigidbody2d.velocity = glidingVelocity;
    }

    private void HandleJump()
    {
        if (IsGrounded() || (timeElapsedSinceGrounded <= coyoteTime && rigidbody2d.velocity.y <= 0))
        {
            rigidbody2d.velocity = new Vector2(0f, jumpSpeed);
        }
    }

    private void PlayerInputActions_Glide_Canceled(InputAction.CallbackContext context)
    {
        isGliding = false;
        storedGlideTime = storedGlideTime - (float)(DateTime.Now - glideStartTime).TotalSeconds;
    }

    private void PlayerInputActions_Glide_Started(InputAction.CallbackContext context)
    {
        HandleJump();
        if (storedGlideTime > 0)
        {
            isGliding = true;
            glideStartTime = DateTime.Now + TimeSpan.FromSeconds(storedGlideTime - perfectGlideTime - taperedGlideTime);
        }
    }

    private bool IsGrounded()
    {
        float extraGroundDistance = 0.1f;
        var raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, extraGroundDistance, groundLayerMask);
        /* Color rayColor; */
        /* if (raycastHit.collider != null) */
        /* { */
        /*     rayColor = Color.green; */
        /* } */
        /* else */
        /* { */
        /*     rayColor = Color.red; */
        /* } */
        /* Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraGroundDistance), rayColor); */
        /* Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraGroundDistance), rayColor); */
        /* Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + extraGroundDistance), Vector2.right * 2 * playerCollider.bounds.extents.x, rayColor); */
        return raycastHit.collider != null;
    }

    private void HandleMurder()
    {
        float extraGroundDistance = 0.01f;
        var raycastHit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + extraGroundDistance, murderLayerMask);
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.TryGetComponent<Enemy>(out var enemy))
            {
                if (rigidbody2d.velocity.y <= 0)
                {
                    isArrested = true;
                }
            }
        }
    }

    private void HandleDeath()
    {
        float playerHeadPos = transform.position.y + playerCollider.bounds.extents.y;
        float cameraBottomPos = cameraController.position.y - cameraController.cameraHeight / 2;
        float extraDistance = 10f;
        if (playerHeadPos < cameraBottomPos - extraDistance)
        {
            isDead = true;
        }
    }
}

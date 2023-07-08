using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private const float moveSpeed = 7f;
    [SerializeField] private const float glidingFactor = 0.6f;
    [SerializeField] private const float jumpSpeed = 9.0f;
    [SerializeField] private LayerMask groundLayerMask;
    private PlayerInputActions playerInputActions;
    private Rigidbody2D rigidbody2d;
    private bool canJump = false;
    private bool isGliding = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D playerCollider;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
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
        if (isGliding)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
        transform.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Glide.started -= PlayerInputActions_Glide_Started;
        playerInputActions.Player.Glide.canceled -= PlayerInputActions_Glide_Canceled;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        rigidbody2d.velocity = new Vector2(inputVector.x * moveSpeed, rigidbody2d.velocity.y);
    }

    private void HandleGlide()
    {
        if (!isGliding || rigidbody2d.velocity.y >= 0 || IsGrounded()) return;
        Vector2 glidingVelocity = rigidbody2d.velocity;
        glidingVelocity.y *= glidingFactor;
        rigidbody2d.velocity = glidingVelocity;
    }

    private void HandleJump()
    {
        rigidbody2d.velocity = new Vector2(0f, jumpSpeed);
    }

    private void PlayerInputActions_Glide_Canceled(InputAction.CallbackContext context)
    {
        isGliding = false;
    }

    private void PlayerInputActions_Glide_Started(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            HandleJump();
        }
        isGliding = true;
    }

    private bool IsGrounded()
    {
        float groundDistance = 0.1f;
        var raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, groundDistance, groundLayerMask);
        /* Color rayColor; */
        /* if (raycastHit.collider != null) */
        /* { */
        /*     rayColor = Color.green; */
        /* } */
        /* else */
        /* { */
        /*     rayColor = Color.red; */
        /* } */
        /* Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + groundDistance), rayColor); */
        /* Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + groundDistance), rayColor); */
        /* Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + groundDistance), Vector2.right * 2 * playerCollider.bounds.extents.x, rayColor); */
        return raycastHit.collider != null;
    }
}

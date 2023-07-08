using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private float glidingFactor = 0.6f;
  private PlayerInputActions playerInputActions;
  private Rigidbody2D rigidbody2d;
  private bool canJump = false;
  private bool isGliding = false;

  private void Awake() {
    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();
    playerInputActions.Player.Glide.started += PlayerInputActions_Glide_Started;
    playerInputActions.Player.Glide.canceled += PlayerInputActions_Glide_Canceled;
    rigidbody2d = GetComponent<Rigidbody2D>();
  }

  private void FixedUpdate() {
    HandleMovement();
    HandleGlide();
  }

  private void HandleMovement() {
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    rigidbody2d.velocity = new Vector2(inputVector.x * moveSpeed, rigidbody2d.velocity.y);
  }

  private void HandleGlide() {
    if (!isGliding || rigidbody2d.velocity.y >= 0) return;
    Vector2 glidingVelocity = rigidbody2d.velocity;
    glidingVelocity.y *= glidingFactor;
    rigidbody2d.velocity = glidingVelocity;
  }

  private void HandleJump() {
    if (canJump) {
      rigidbody2d.velocity = new Vector2(0f, 3f);
      canJump = false;
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    canJump = true;
  }

  void OnCollisionExit2D(Collision2D collision) {
    canJump = false;
  }

  private void PlayerInputActions_Glide_Canceled(InputAction.CallbackContext context) {
    isGliding = false;
  }

  private void PlayerInputActions_Glide_Started(InputAction.CallbackContext context) {
    if (canJump) {
      HandleJump();
    }
    isGliding = true;
  }
}


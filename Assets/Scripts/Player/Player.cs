using UnityEngine;

public class Player : MonoBehaviour {
  [SerializeField]
  private float moveSpeed = 7f;
  private PlayerInputActions playerInputActions;

  private void Awake() {
    playerInputActions = new PlayerInputActions();
    playerInputActions.PlayerInput.Enable();
  }

  private void Update() {
    HandleMovement();
  }

  private void HandleMovement() {
    Vector2 inputVector = playerInputActions.PlayerInput.Move.ReadValue<Vector2>();
    inputVector = inputVector.normalized;
    float moveDistance = moveSpeed * Time.deltaTime;
    transform.Translate(moveDistance * inputVector);
  }
}

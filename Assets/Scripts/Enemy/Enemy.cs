using UnityEngine;

public class Enemy : MonoBehaviour, IInteractable
{
    public float moveSpeed = 5f;

    [SerializeField] private Platform platform;
    private BoxCollider2D enemyCollider;
    private int movingDirection;
    private Rigidbody2D rigidbody2d;

    public void Initialize(Platform platform)
    {
        this.platform = platform;
    }

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();
        float randomDirection = Random.Range(0f, 1f);
        if (randomDirection <= 0.5f)
        {
            movingDirection = -1;
        }
        else
        {
            movingDirection = 1;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (platform == null) return;
        Bounds platformBounds = platform.bounds;
        Bounds enemyBounds = enemyCollider.bounds;
        float enlargmentFactor = 1.1f;
        float leftMostPlatformBound = platformBounds.center.x - platformBounds.extents.x;
        float rightMostPlatformBound = platformBounds.center.x + platformBounds.extents.x;
        float leftMostEnemyBody = enemyBounds.center.x - enemyBounds.extents.x * enlargmentFactor;
        float rightMostEnemyBody = enemyBounds.center.x + enemyBounds.extents.x * enlargmentFactor;
        if (leftMostEnemyBody <= leftMostPlatformBound || rightMostEnemyBody >= rightMostPlatformBound)
        {
            movingDirection *= -1;
        }
        Vector2 newVelocity = new Vector2(moveSpeed * movingDirection, rigidbody2d.velocity.y);
        rigidbody2d.velocity = newVelocity;
    }

    public void Interact(Player player)
    {
        throw new System.NotImplementedException();
    }
}

using UnityEngine;

public class FallingPlatform : Platform
{
    [SerializeField] private float maxSecondsBeforeColorChange = 0.1F;
    [SerializeField] private float maxSecondsBeforeFall = 5.0F;

    private bool isFalling = false;
    private float timeSinceLastColorChange = 0.0F;

    private SpriteRenderer spriteRenderer;
    private Color initialColor;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
        player = Player.singleton;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFalling) return;

        timeSinceLastColorChange += Time.deltaTime;
        if (timeSinceLastColorChange >= maxSecondsBeforeColorChange)
        {
            timeSinceLastColorChange = 0.0F;
            spriteRenderer.color = spriteRenderer.color == initialColor ? Color.red : initialColor;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            isFalling = true;
            Destroy(this.gameObject, maxSecondsBeforeFall);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            isFalling = false;
        }
    }
}

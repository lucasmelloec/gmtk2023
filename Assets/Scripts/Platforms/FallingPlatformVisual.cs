using UnityEngine;

public class FallingPlatformVisual : MonoBehaviour
{
    [SerializeField] private FallingPlatform fallingPlatform;
    private float timeSinceLastColorChange = 0.0F;
    private float maxSecondsBeforeColorChange;
    private bool isFalling;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
        maxSecondsBeforeColorChange = fallingPlatform.maxSecondsBeforeColorChange;
    }

    private void OnEnable()
    {
        fallingPlatform.onIsFalling += FallingPlatform_OnIsFalling;
    }

    private void Update()
    {
        if (!isFalling) return;

        timeSinceLastColorChange += Time.deltaTime;
        if (timeSinceLastColorChange >= maxSecondsBeforeColorChange)
        {
            timeSinceLastColorChange = 0.0F;
            spriteRenderer.color = spriteRenderer.color == initialColor ? Color.red : initialColor;
        }
    }

    private void OnDisable()
    {
        fallingPlatform.onIsFalling -= FallingPlatform_OnIsFalling;
    }

    private void FallingPlatform_OnIsFalling()
    {
        isFalling = true;
    }
}

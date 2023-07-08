using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        player.onIsGlidingChanged += Player_OnIsGlidingChanged;
    }

    private void OnDisable()
    {
        player.onIsGlidingChanged -= Player_OnIsGlidingChanged;
    }

    private void Player_OnIsGlidingChanged(bool isGliding)
    {
        if (isGliding)
        {
            spriteRenderer.color = Color.blue;
        }
        else
        {
            spriteRenderer.color = initialColor;
        }
    }
}

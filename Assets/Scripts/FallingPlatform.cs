using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public GameObject player;
    public float maxSecondsBeforeColorChange = 0.5F;
    public float maxSecondsBeforeFall = 5.0F;

    private bool isFalling = false;
    private float timeSinceLastColorChange = 0.0F;

    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFalling) return;

        timeSinceLastColorChange += Time.deltaTime;
        if (timeSinceLastColorChange >= maxSecondsBeforeColorChange) {
            timeSinceLastColorChange = 0.0F;
            spriteRenderer.color = spriteRenderer.color == initialColor ? Color.red : initialColor;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player) {
            isFalling = true;
            Destroy(this.gameObject, maxSecondsBeforeFall);
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject == player) {
            isFalling = false;
        }
    }
}

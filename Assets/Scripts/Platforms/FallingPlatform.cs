using System;
using UnityEngine;

public class FallingPlatform : Platform
{
    public event Action onIsFalling;
    [field: SerializeField] public float maxSecondsBeforeColorChange { get; private set; } = 0.5F;
    [SerializeField] private float maxSecondsBeforeFall = 5.0F;

    private Player player;

    void Start()
    {
        player = Player.singleton;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            onIsFalling?.Invoke();
            Destroy(this.gameObject, maxSecondsBeforeFall);
        }
    }
}

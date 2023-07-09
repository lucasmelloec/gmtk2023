using Assets.Native;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private BoxCollider2D platformCollider;

    protected virtual void Awake()
    {
        platformCollider = GetComponent<BoxCollider2D>();
    }

    public static bool Intersects(Platform platform0, Transform platform1Center)
    {
        return Intersects(platform0.transform, platform1Center);
    }

    public static bool Intersects(Transform platform0Center, Transform platform1Center)
    {
        var bounds0 = new Bounds(platform0Center.position, new Vector3(Constants.PlatformNoOverlapWidth, Constants.PlatformNoOverlapHeight, 0));
        var bounds1 = new Bounds(platform1Center.position, new Vector3(Constants.PlatformNoOverlapWidth, Constants.PlatformNoOverlapHeight, 0));

        return bounds0.Intersects(bounds1);
    }

    public static bool Intersects(Vector3 platform0Center, Vector3 platform1Center)
    {
        var bounds0 = new Bounds(platform0Center, new Vector3(Constants.PlatformNoOverlapWidth, Constants.PlatformNoOverlapHeight, 0));
        var bounds1 = new Bounds(platform1Center, new Vector3(Constants.PlatformNoOverlapWidth, Constants.PlatformNoOverlapHeight, 0));

        return bounds0.Intersects(bounds1);
    }

    public Bounds bounds { get => platformCollider.bounds; }
}

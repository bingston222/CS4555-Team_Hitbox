using UnityEngine;

[CreateAssetMenu(menuName = "Glitch/Projectile Data", fileName = "ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Prefab & VFX")]
    [Tooltip("Projectile prefab that gets instantiated and launched.")]
    public GameObject prefab;

    [Tooltip("One-shot VFX that appears at fire point.")]
    public GameObject muzzleVfxPrefab;

    [Tooltip("Looping FX that follows projectile during flight.")]
    public GameObject flightVfxPrefab;

    [Tooltip("Impact VFX spawned when projectile hits something.")]
    public GameObject hitVfxPrefab;

    [Header("Audio")]
    [Tooltip("Sound played when projectile is fired.")]
    public AudioClip fireSfx;

    [Tooltip("Sound played when projectile hits.")]
    public AudioClip hitSfx;

    [Header("Stats")]
    public float speed = 16f;
    public float damage = 10f;
    public float lifetime = 6f;
    public bool useGravity = false;

    [Header("Collision Filtering")]
    [Tooltip("Layers this projectile is allowed to damage (Player, etc).")]
    public LayerMask hittableLayers;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // If user forgets to assign layers, default to Player layer
        if (hittableLayers == 0)
        {
            int mask = LayerMask.GetMask("Player");
            if (mask != 0)
                hittableLayers = mask;
        }
    }
#endif
}

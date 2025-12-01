using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Projectile Data", fileName = "ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Prefab & VFX")]
    [Tooltip("The projectile prefab that gets instantiated and launched.")]
    public GameObject prefab;

    [Tooltip("One-shot effect spawned at the shooter when fired.")]
    public GameObject muzzleVfxPrefab;

    public GameObject flightVfxPrefab;  // continuous FX that travels with projectile


    [Tooltip("Impact effect spawned where the projectile hits.")]
    public GameObject hitVfxPrefab;

    [Tooltip("Sound played at fire time.")]
    public AudioClip fireSfx;

    [Tooltip("Sound played on impact.")]
    public AudioClip hitSfx;

    [Header("Tuning")]
    public float speed = 16f;
    public float damage = 10f;
    public bool  useGravity = false;
    public float lifetime = 6f;

    [Header("Layers")]
    [Tooltip("What layers this projectile is allowed to damage (e.g., Player).")]
    public LayerMask hittableLayers;

#if UNITY_EDITOR
    // Convenience: if you forget to set it, default to Player layer (if it exists)
    private void OnValidate()
    {
        if (hittableLayers == 0)
        {
            int mask = LayerMask.GetMask("Player");
            if (mask != 0) hittableLayers = mask;
        }
    }
#endif
}

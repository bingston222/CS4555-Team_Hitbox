using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DataParticlesColorSwap : MonoBehaviour
{
    public Gradient corruptedGradient;
    public Gradient purifiedGradient;

    [SerializeField] private bool purified;
    private bool lastPurified;

    private ParticleSystem ps;
    private ParticleSystem.ColorOverLifetimeModule col;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        col = ps.colorOverLifetime;
        col.enabled = true;
        Apply();
        lastPurified = purified;
    }

    void Update()
    {
        // If you toggle the checkbox in Play Mode, apply immediately
        if (purified != lastPurified)
        {
            Apply();
            lastPurified = purified;
        }
    }

    // Lets you drive it from other scripts
    public void SetPurified(bool on)
    {
        purified = on;
        Apply();
        lastPurified = purified;
    }

    private void Apply()
    {
        var g = purified ? purifiedGradient : corruptedGradient;
        col.color = new ParticleSystem.MinMaxGradient(g);
    }

#if UNITY_EDITOR
    // Updates in Edit mode when you tweak fields
    void OnValidate()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        col = ps.colorOverLifetime;
        col.enabled = true;
        Apply();
        lastPurified = purified;
    }
#endif
}

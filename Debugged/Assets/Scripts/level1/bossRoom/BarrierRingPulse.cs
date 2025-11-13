using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BarrierRingPulse : MonoBehaviour
{
    [Header("Colors")]
    public Color corrupted = new Color(1f, 0.35f, 0f);  // orange/red
    public Color purified  = new Color(0f, 0.8f, 1f);   // cyan/blue

    [Header("State")]
    public bool isPurified;

    [Header("Pulse")]
    public float pulseSpeed = 2f;
    public float pulseMin = 0.6f;
    public float pulseMax = 1.2f;

    [Header("Intensity")]
    [Tooltip("Emission intensity multiplier (HDR).")]
    public float emissionIntensity = 1.0f;   // try 0.8–1.2
    [Tooltip("How much to tint the base color (0 = black).")]
    public float baseTintStrength = 0.0f;    // try 0–0.25

    private Renderer rend;
    private Material mat;

    private static readonly int Emiss   = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseCol = Shader.PropertyToID("_BaseColor"); // URP/Lit & URP/Unlit

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mat  = rend.material;                 // instance so we don't edit shared asset
        mat.EnableKeyword("_EMISSION");
        ApplyImmediate();
    }

    void Update()
    {
        float s = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        Color c = isPurified ? purified : corrupted;

        if (mat.HasProperty(BaseCol))
            mat.SetColor(BaseCol, c * baseTintStrength);   // keep low to avoid flooding

        mat.SetColor(Emiss, c * (emissionIntensity * s));  // glow
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!rend) rend = GetComponent<Renderer>();
        if (rend && mat == null) mat = rend.sharedMaterial; // preview in editor
        if (mat != null) { mat.EnableKeyword("_EMISSION"); ApplyImmediate(); }
    }
#endif

    public void SetPurified(bool on)
    {
        isPurified = on;
        ApplyImmediate();
    }

    private void ApplyImmediate()
    {
        if (!mat) return;
        Color c = isPurified ? purified : corrupted;
        if (mat.HasProperty(BaseCol)) mat.SetColor(BaseCol, c * baseTintStrength);
        mat.SetColor(Emiss, c * emissionIntensity);
    }
}

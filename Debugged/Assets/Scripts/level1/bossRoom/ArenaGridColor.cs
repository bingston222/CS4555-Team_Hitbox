// Add this to your ArenaStateManager (or drop as a small companion component)
using UnityEngine;
using System.Collections.Generic;

public class ArenaGridColor : MonoBehaviour
{
    [Tooltip("Renderers that use the grid material (TrainSta_GridGlow_MAT). " +
             "If the grid is a second material slot on the same mesh, set materialIndex=1.")]
    public List<Renderer> gridRenderers = new List<Renderer>();
    public int materialIndex = 0; // which material slot has the grid
    public Color corrupted = new Color(1f, 0.35f, 0f);
    public Color purified  = new Color(0f, 0.8f, 1f);
    [Range(0f, 5f)] public float speed = 1f;

    // we’ll try common color property names in order
    static readonly int _BaseColor = Shader.PropertyToID("_BaseColor");
    static readonly int _Color     = Shader.PropertyToID("_Color");
    static readonly int _TintColor = Shader.PropertyToID("_TintColor");
    MaterialPropertyBlock mpb;
    Color target, current;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        target = corrupted;
        current = corrupted;
        ApplyImmediate();
    }

    void Update()
    {
        current = Color.Lerp(current, target, Time.deltaTime * Mathf.Max(speed, 0.0001f));
        Apply(current);
    }

    public void SetPurified(bool on)
    {
        target = on ? purified : corrupted;
    }

    void ApplyImmediate()
    {
        current = target;
        Apply(current);
    }

    void Apply(Color c)
    {
        foreach (var r in gridRenderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(mpb, materialIndex);

            // pick the first property this material actually has
            if      (Has(r, _BaseColor)) mpb.SetColor(_BaseColor, c);
            else if (Has(r, _Color))     mpb.SetColor(_Color, c);
            else if (Has(r, _TintColor)) mpb.SetColor(_TintColor, c);

            r.SetPropertyBlock(mpb, materialIndex);
        }
    }

    bool Has(Renderer r, int propId)
    {
        var mats = r.sharedMaterials;
        if (materialIndex < 0 || materialIndex >= mats.Length || mats[materialIndex] == null) return false;
        return mats[materialIndex].HasProperty(propId);
    }
}

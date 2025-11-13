using UnityEngine;
using System.Collections.Generic;

public class ArenaStateManager : MonoBehaviour
{
    [Header("Renderers (not materials)")]
    public Renderer floorRenderer;        // assign ArenaFloor's MeshRenderer
    public List<Renderer> wallRenderers;  // assign WallNorth/East/South/... renderers

    [Header("Lights")]
    public Light[] arenaLights;

    [Header("Colors")]
    public Color corruptedColor = new Color(1f, 0.35f, 0f); // orange/red
    public Color purifiedColor  = new Color(0f, 0.8f, 1f);  // cyan/blue

    [Range(0f, 5f)] public float transitionSpeed = 1f;
    public bool startPurified = false;

    // shader property ids
    static readonly int Emiss   = Shader.PropertyToID("_EmissionColor");
    static readonly int BaseCol = Shader.PropertyToID("_BaseColor");

    // working state
    private bool isPurified;
    private MaterialPropertyBlock floorBlock;
    private MaterialPropertyBlock wallBlock;

    void Awake()
    {
        // Create reusable MPBs
        floorBlock = new MaterialPropertyBlock();
        wallBlock  = new MaterialPropertyBlock();

        // Force an initial state (red by default)
        isPurified = startPurified;
        ApplyImmediate();
    }

    void Update()
    {
        // Smoothly lerp to target
        Color target = isPurified ? purifiedColor : corruptedColor;
        float t = Time.deltaTime * Mathf.Max(transitionSpeed, 0.0001f);

        // Lights
        foreach (var l in arenaLights)
            if (l) l.color = Color.Lerp(l.color, target, t);

        // Floor
        if (floorRenderer)
        {
            floorRenderer.GetPropertyBlock(floorBlock);
            Color curE = floorBlock.GetVector(Emiss);
            Color curB = floorBlock.GetVector(BaseCol);
            floorBlock.SetColor(Emiss, Color.Lerp(curE, target * 1.0f, t));
            floorBlock.SetColor(BaseCol, Color.Lerp(curB, target * 0.10f, t)); // subtle tint
            floorRenderer.SetPropertyBlock(floorBlock);
        }

        // Walls
        foreach (var r in wallRenderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(wallBlock);
            Color curE = wallBlock.GetVector(Emiss);
            Color curB = wallBlock.GetVector(BaseCol);
            wallBlock.SetColor(Emiss, Color.Lerp(curE, target * 1.0f, t));
            wallBlock.SetColor(BaseCol, Color.Lerp(curB, target * 0.10f, t));
            r.SetPropertyBlock(wallBlock);
        }
    }

    public void SetPurified(bool purified)
    {
        isPurified = purified;
        ApplyImmediate(); // snap once so it feels responsive; Update will smooth after
    }

    private void ApplyImmediate()
    {
        Color c = isPurified ? purifiedColor : corruptedColor;

        // Lights
        foreach (var l in arenaLights)
            if (l) l.color = c;

        // Floor
        if (floorRenderer)
        {
            floorRenderer.GetPropertyBlock(floorBlock);
            floorBlock.SetColor(Emiss,   c * 1.0f);
            floorBlock.SetColor(BaseCol, c * 0.10f);
            floorRenderer.SetPropertyBlock(floorBlock);
        }

        // Walls
        foreach (var r in wallRenderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(wallBlock);
            wallBlock.SetColor(Emiss,   c * 1.0f);
            wallBlock.SetColor(BaseCol, c * 0.10f);
            r.SetPropertyBlock(wallBlock);
        }
    }

    // Handy editor button to auto-fill wall renderers from children named "Wall*"
    [ContextMenu("Auto-Fill Wall Renderers")]
    void AutoFillWalls()
    {
        wallRenderers = new List<Renderer>();
        var rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
            if (r.name.StartsWith("Wall"))
                wallRenderers.Add(r);
    }
}

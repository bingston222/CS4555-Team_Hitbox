using UnityEngine;

[RequireComponent(typeof(Health))]
public class SimpleHealthBar: MonoBehaviour
{
    [Header("Position / Size")]
    public Vector3 offset = new Vector3(0, 1.6f, 0);
    public Vector2 size = new Vector2(0.9f, 0.12f);

    [Header("Colors")]
    public Color bgColor   = new Color(0f, 0f, 0f, 0.6f);
    public Color fillColor = new Color(0.2f, 0.95f, 0.2f, 0.95f);

    [Header("Behavior")]
    public bool alwaysFaceCamera = true;
    public bool hideWhenFull = false;       // start with false so you SEE it
    public bool drawDebugCube = true;       // shows a tiny cube at the anchor

    Transform barRoot, bgQuad, fillQuad;
    Material bgMat, fillMat;
    Health hp;

    static Shader FindAnyShader()
    {
        // Built-in first
        var s = Shader.Find("Unlit/Color");
        if (s) return s;
        // URP
        s = Shader.Find("Universal Render Pipeline/Unlit");
        if (s) return s;
        // HDRP (fallback to Lit with color)
        s = Shader.Find("HDRP/Unlit");
        if (s) return s;
        // As a last resort, TMP Mobile Distance Field exists in most projects
        s = Shader.Find("TextMeshPro/Mobile/Distance Field");
        return s;
    }

    void Start()
    {
        hp = GetComponent<Health>();
        Debug.Log($"[HealthBar3D] On {name}. CurrentHP={hp.CurrentHP}/{hp.maxHP}");

        // Anchor
        barRoot = new GameObject("HealthBar3D").transform;
        barRoot.SetParent(transform, worldPositionStays:false);
        barRoot.localPosition = offset;
        barRoot.localRotation = Quaternion.identity;

        // Debug cube at anchor so you can SEE it's there
        if (drawDebugCube)
        {
            var dbg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dbg.name = "HB_Debug";
            dbg.transform.SetParent(barRoot, false);
            dbg.transform.localScale = Vector3.one * 0.05f;
            Destroy(dbg.GetComponent<Collider>());
            var mr = dbg.GetComponent<MeshRenderer>();
            if (mr) mr.material.color = Color.cyan;
            Destroy(dbg, 1.0f); // auto clean
        }

        // Make quads
        bgQuad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
        bgQuad.name = "BG";
        bgQuad.SetParent(barRoot, false);
        bgQuad.localScale = new Vector3(size.x, size.y, 1f);
        Destroy(bgQuad.GetComponent<Collider>());

        fillQuad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
        fillQuad.name = "Fill";
        fillQuad.SetParent(barRoot, false);
        Destroy(fillQuad.GetComponent<Collider>());

        // Materials with shader fallback
        var shader = FindAnyShader();
        if (!shader) Debug.LogWarning("[HealthBar3D] Could not find an Unlit shader; will still try with default.");
        bgMat = new Material(shader ? shader : Shader.Find("Standard"));
        fillMat = new Material(shader ? shader : Shader.Find("Standard"));

        if (bgMat.HasProperty("_BaseColor")) bgMat.SetColor("_BaseColor", bgColor);
        if (bgMat.HasProperty("_Color"))     bgMat.SetColor("_Color",     bgColor);
        if (fillMat.HasProperty("_BaseColor")) fillMat.SetColor("_BaseColor", fillColor);
        if (fillMat.HasProperty("_Color"))     fillMat.SetColor("_Color",     fillColor);

        bgQuad.GetComponent<MeshRenderer>().sharedMaterial = bgMat;
        fillQuad.GetComponent<MeshRenderer>().sharedMaterial = fillMat;

        // Initialize + subscribe
        UpdateBar(hp.CurrentHP, hp.maxHP);
        hp.onHealthChanged.AddListener(UpdateBar);
        hp.onDeath += () => { if (barRoot) Destroy(barRoot.gameObject); };

        Debug.Log($"[HealthBar3D] Shader used: {(shader ? shader.name : "Standard Fallback")}");
    }

    void LateUpdate()
    {
        if (alwaysFaceCamera && Camera.main)
        {
            var cam = Camera.main.transform;
            barRoot.rotation = Quaternion.LookRotation(barRoot.position - cam.position, Vector3.up);
        }
    }

    void UpdateBar(float current, float max)
    {
        float p = Mathf.Approximately(max, 0f) ? 0f : Mathf.Clamp01(current / max);

        // width based on percent
        float w = size.x * p;
        fillQuad.localScale = new Vector3(Mathf.Max(w, 0.0001f), size.y, 1f);
        // keep fill anchored to left edge
        fillQuad.localPosition = new Vector3(-(size.x * 0.5f) + (w * 0.5f), 0f, -0.001f);

        bool show = !(hideWhenFull && p >= 0.999f);
        if (bgQuad)  bgQuad.gameObject.SetActive(show);
        if (fillQuad) fillQuad.gameObject.SetActive(show);
    }
}

using UnityEngine;

[RequireComponent(typeof(Health))]
public class SimpleHealthBar : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1.6f, 0);
    public Vector2 size = new Vector2(0.9f, 0.12f);

    public Color bgColor = new Color(0, 0, 0, 0.6f);
    public Color fillColor = new Color(0.2f, 0.95f, 0.2f, 0.95f);

    public bool alwaysFaceCamera = true;
    public bool hideWhenFull = false;
    public bool drawDebugCube = true;

    Transform barRoot, bgQuad, fillQuad;
    Material bgMat, fillMat;
    Health hp;

    void Start()
    {
        hp = GetComponent<Health>();

        // build bar...
        BuildBar();

        UpdateBar(hp.CurrentHP, hp.maxHP);

        hp.onHealthChanged += UpdateBar;   // FIXED
        hp.onDeath += () => { Destroy(barRoot.gameObject); };
    }

    void UpdateBar(float current, float max)
    {
        float p = Mathf.Clamp01(current / Mathf.Max(0.0001f, max));

        float w = size.x * p;
        fillQuad.localScale = new Vector3(Mathf.Max(w, 0.0001f), size.y, 1f);
        fillQuad.localPosition = new Vector3(-(size.x * 0.5f) + (w * 0.5f), 0f, -0.001f);

        bool show = !(hideWhenFull && p >= 1f);
        bgQuad.gameObject.SetActive(show);
        fillQuad.gameObject.SetActive(show);
    }

    void LateUpdate()
    {
        if (alwaysFaceCamera && Camera.main)
            barRoot.rotation = Quaternion.LookRotation(barRoot.position - Camera.main.transform.position, Vector3.up);
    }

    void BuildBar()
    {
        // (your original code stayed the same)
    }
}

using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshPro text;          // assign in prefab
    public float lifetime = 0.8f;
    public float riseSpeed = 1.2f;
    public float startScale = 0.9f;
    public float endScale = 1.1f;

    float t;
    Camera cam;
    Color startColor;

    void Awake()
    {
        cam = Camera.main;
        if (!text) text = GetComponentInChildren<TextMeshPro>();
        startColor = text.color;
        transform.localScale = Vector3.one * startScale;
    }

    public void Set(string msg, Color color)
    {
        if (!text) return;
        text.text = msg;
        text.color = color;
        startColor = color;
        t = 0f;
    }

    void Update()
    {
        if (!cam) cam = Camera.main;

        t += Time.deltaTime;
        float u = Mathf.Clamp01(t / lifetime);

        // Face camera (billboard)
        if (cam)
        {
            var fwd = cam.transform.rotation * Vector3.forward;
            var up  = cam.transform.rotation * Vector3.up;
            transform.LookAt(transform.position + fwd, up);
        }

        // Drift up
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Scale & fade
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, u);
        if (text)
        {
            Color c = startColor; c.a = 1f - u;
            text.color = c;
        }

        if (t >= lifetime) Destroy(gameObject);
    }
}

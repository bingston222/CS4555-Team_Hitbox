using UnityEngine;

public class HoverBob : MonoBehaviour
{
    public float amplitude = 0.15f;
    public float frequency = 1.5f;
    public float spin = 45f; // deg/sec
    Vector3 startPos;

    void Start() => startPos = transform.localPosition;

    void Update() {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
        transform.Rotate(0f, spin * Time.deltaTime, 0f, Space.World);
    }
}

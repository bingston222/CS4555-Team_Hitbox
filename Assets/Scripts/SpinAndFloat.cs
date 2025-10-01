using UnityEngine;

public class SpinAndFloat : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // spin
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // float up/down
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}

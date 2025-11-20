using UnityEngine;

public class SweepingLight : MonoBehaviour
{
    [Header("Sweep")]
    [SerializeField] private float sweepAngle = 60f;    // total arc
    [SerializeField] private float sweepSpeed = 1.2f;   // cycles per second
    [SerializeField] private Vector3 sweepAxis = Vector3.up;

    [Header("Blink")]
    [SerializeField] private bool blink = true;
    [SerializeField] private float blinkHz = 2.0f;      // on/off per second
    [SerializeField] private float dutyCycle = 0.5f;    // 0..1 (fraction ON)

    [Header("Light & Visuals")]
    [SerializeField] private Light spot;                // assign your child Spot Light
    [SerializeField] private Color offColor = Color.black;
    [SerializeField] private Color onColor = Color.red;
    [SerializeField] private float onIntensity = 7f;
    [SerializeField] private float offIntensity = 0f;

    // Exposed for DetectionZone
    public bool IsOn { get; private set; } = true;

    private Quaternion baseRotation;

    void Reset()
    {
        spot = GetComponentInChildren<Light>();
    }

    void Start()
    {
        baseRotation = transform.localRotation;
        if (spot != null)
        {
            spot.type = LightType.Spot;
            spot.color = onColor;
            spot.intensity = onIntensity;
        }
    }

    void Update()
    {
        // Sweep
        float t = Mathf.PingPong(Time.time * sweepSpeed, 1f) * 2f - 1f; // -1..1
        float half = sweepAngle * 0.5f;
        Quaternion sweepRot = Quaternion.AngleAxis(t * half, transform.TransformDirection(sweepAxis));
        transform.localRotation = baseRotation * sweepRot;

        // Blink
        if (blink)
        {
            float phase = Mathf.Repeat(Time.time * blinkHz, 1f);
            IsOn = phase <= dutyCycle;
        }
        else IsOn = true;

        if (spot)
        {
            spot.enabled = IsOn;
            spot.color = IsOn ? onColor : offColor;
            spot.intensity = IsOn ? onIntensity : offIntensity;
        }
    }
}

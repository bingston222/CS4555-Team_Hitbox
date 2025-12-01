using UnityEngine;

public class SweepingLight : MonoBehaviour
{
    [Header("Sweep")]
    [SerializeField] private float sweepAngle = 60f;          // total arc (degrees)
    [SerializeField] private float sweepSpeed = 1.2f;         // cycles per second
    [SerializeField] private Vector3 sweepAxis = Vector3.up;  // LOCAL axis

    [Header("Blink")]
    [SerializeField] private bool blink = true;
    [SerializeField] private float blinkHz = 2.0f;
    [Range(0f, 1f)] [SerializeField] private float dutyCycle = 0.5f;

    [Header("Light & Visuals")]
    [SerializeField] private Light spot;                      // assign child Spot Light
    [SerializeField] private Color offColor = Color.black;
    [SerializeField] private Color onColor = Color.red;
    [SerializeField] private float onIntensity = 7f;
    [SerializeField] private float offIntensity = 0f;

    [Header("Caught Behavior")]
    [Tooltip("If true, this light shuts off when the player is caught (full alarm).")]
    [SerializeField] private bool disableOnCatch = true;

    public bool IsOn { get; private set; } = true;

    private Quaternion baseRotation;

    // late-subscribe support
    private bool _subscribed;

    void Reset()
    {
        if (!spot) spot = GetComponentInChildren<Light>(true);
    }

    void OnValidate()
    {
        dutyCycle = Mathf.Clamp01(dutyCycle);
        if (!spot) spot = GetComponentInChildren<Light>(true);
    }

    void OnEnable()
    {
        TrySubscribe();
        if (!_subscribed) StartCoroutine(WaitForAlarmThenSubscribe());
    }

    void OnDisable()
    {
        if (AlarmManager.Instance != null)
            AlarmManager.Instance.OnPlayerCaught -= HandlePlayerCaught;
        _subscribed = false;
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;
        if (AlarmManager.Instance == null) return;
        AlarmManager.Instance.OnPlayerCaught += HandlePlayerCaught;
        _subscribed = true;
    }

    System.Collections.IEnumerator WaitForAlarmThenSubscribe()
    {
        float end = Time.time + 3f;
        while (!_subscribed && Time.time < end)
        {
            TrySubscribe();
            yield return null;
        }
    }

    void Start()
    {
        baseRotation = transform.localRotation;
        if (spot)
        {
            spot.type = LightType.Spot;
            spot.color = onColor;
            spot.intensity = onIntensity;
        }
    }

    void Update()
    {
        // Sweep (LOCAL axis)
        float t = Mathf.PingPong(Time.time * sweepSpeed, 1f) * 2f - 1f; // -1..1
        float half = sweepAngle * 0.5f;
        Quaternion sweepRot = Quaternion.AngleAxis(t * half, sweepAxis.normalized);
        transform.localRotation = baseRotation * sweepRot;

        // Blink
        IsOn = blink ? (Mathf.Repeat(Time.time * blinkHz, 1f) <= dutyCycle) : true;

        if (spot)
        {
            spot.enabled = IsOn;
            spot.color = IsOn ? onColor : offColor;
            spot.intensity = IsOn ? onIntensity : offIntensity;
        }
    }

    // called by event and by AlarmManager.ForceAllLightsOff()
    private void HandlePlayerCaught()
    {
        if (!disableOnCatch) return;

        IsOn = false;
        if (spot)
        {
            spot.enabled = false;
            spot.color = offColor;
            spot.intensity = offIntensity;
        }
        enabled = false; // stop Update
        // Debug.Log("[SweepingLight] HandlePlayerCaught fired → shutting down", this);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 axis = transform.TransformDirection(sweepAxis.normalized);
        Vector3 forward = transform.forward;
        Quaternion left = Quaternion.AngleAxis(-sweepAngle * 0.5f, axis);
        Quaternion right = Quaternion.AngleAxis(+sweepAngle * 0.5f, axis);
        Gizmos.DrawLine(transform.position, transform.position + left * forward * 2f);
        Gizmos.DrawLine(transform.position, transform.position + right * forward * 2f);
    }
#endif
}

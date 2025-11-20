using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectionZone : MonoBehaviour
{
    [Tooltip("Reference to the SweepingLight on the camera root")]
    [SerializeField] private SweepingLight sweepingLight;

    [Header("Debounce")]
    [SerializeField] private float perPlayerCooldown = 0.75f;

    private Collider col;
    private readonly System.Collections.Generic.Dictionary<GameObject, float> lastPing =
        new System.Collections.Generic.Dictionary<GameObject, float>();

    void Reset()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        if (sweepingLight == null)
            sweepingLight = GetComponentInParent<SweepingLight>();
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        if (sweepingLight == null)
            sweepingLight = GetComponentInParent<SweepingLight>();
    }

    void OnTriggerEnter(Collider other)
    {
        TryTrip(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        // In case they spawn inside or linger; still respects cooldown
        TryTrip(other.gameObject);
    }

    private void TryTrip(GameObject obj)
    {
        if (obj.CompareTag("Player") == false) return;
        if (sweepingLight == null || sweepingLight.IsOn == false) return;

        float now = Time.time;
        if (lastPing.TryGetValue(obj, out float last) && (now - last) < perPlayerCooldown) return;

        lastPing[obj] = now;

        // Where the camera is raising the alarm (use the light position for spatialized SFX)
        Vector3 at = sweepingLight ? sweepingLight.transform.position : transform.position;
        AlarmManager.Instance?.TriggerAlarm(at);
    }
}

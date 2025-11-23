using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class DetectionZone : MonoBehaviour
{
    [Tooltip("Reference to the SweepingLight on the camera root")]
    [SerializeField] private SweepingLight sweepingLight;

    [Header("Debounce")]
    [SerializeField] private float perPlayerCooldown = 0.75f;

    [Header("Filtering")]
    [SerializeField] private LayerMask playerLayers = ~0; // optional: restrict detections

    [SerializeField] private bool debugLogs = true;

    private Collider col;
    private readonly Dictionary<GameObject, float> lastPing = new();

    void Reset()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        if (sweepingLight == null) sweepingLight = GetComponentInParent<SweepingLight>();
    }

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        if (sweepingLight == null) sweepingLight = GetComponentInParent<SweepingLight>();
    }

    void OnTriggerEnter(Collider other) => TryTrip(other);
    void OnTriggerStay(Collider other)  => TryTrip(other);

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            lastPing.Remove(other.gameObject); // cleanup
    }

    private void TryTrip(Collider other)
    {
        if (!other || !other.gameObject.activeInHierarchy) return;
        if (!other.CompareTag("Player")) return;

        // Optional layer filter
        if ((playerLayers.value & (1 << other.gameObject.layer)) == 0) return;

        if (sweepingLight == null || sweepingLight.IsOn == false)
        {
            if (debugLogs) Debug.Log("[DetectionZone] Ignored: light off or missing.");
            return;
        }

        float now = Time.time;
        GameObject obj = other.gameObject;

        if (lastPing.TryGetValue(obj, out float last) && (now - last) < perPlayerCooldown)
        {
            if (debugLogs) Debug.Log("[DetectionZone] Debounced (cooldown).");
            return;
        }

        lastPing[obj] = now;

        Vector3 at = sweepingLight ? sweepingLight.transform.position : transform.position;
        if (debugLogs) Debug.Log("[DetectionZone] TRIP! Sending TriggerAlarm.");
        AlarmManager.Instance?.TriggerAlarm(at);
    }
}

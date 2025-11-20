using UnityEngine;
using System.Collections.Generic;

public class SecurityCamera : MonoBehaviour
{
    [Header("Vision")]
    public float viewDistance = 12f;
    [Range(1f,179f)] public float fov = 55f;
    public LayerMask obstacleMask;
    public Transform head;

    [Header("Sweep")]
    public float sweepSpeed = 45f;
    public float leftLimit = -60f;
    public float rightLimit = 60f;

    [Header("Detection")]
    public float timeToCatch = 1.0f;
    public string playerTag = "Player";

    float baseYaw;
    float tDetectP1, tDetectP2;
    bool jammed;
    [Header("Cooldown")]
    public float alarmLockout = 1.0f;
    float _lockoutUntil;

    void Awake()
    {
        if (!head) head = transform;
        baseYaw = head.localEulerAngles.y;
    }

    void Update()
    {
        if (Time.time < _lockoutUntil) return;
        if (!jammed) Sweep();

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float dt = Time.deltaTime;

        tDetectP1 = UpdateDetect(players, 0, tDetectP1, dt);
        tDetectP2 = UpdateDetect(players, 1, tDetectP2, dt);

        if (tDetectP1 >= timeToCatch || tDetectP2 >= timeToCatch)
        {
            AlarmManager.Instance?.OnSpotted(this);
            _lockoutUntil = Time.time + alarmLockout;
            tDetectP1 = tDetectP2 = 0f;
        }
    }

    float UpdateDetect(GameObject[] players, int idx, float timer, float dt)
    {
        if (idx >= players.Length) return Mathf.Max(0f, timer - dt * 2f);
        var p = players[idx].transform.position + Vector3.up * 0.9f;
        var o = head.position;
        var dir = (p - o);
        float dist = dir.magnitude;
        if (dist > viewDistance) return Mathf.Max(0f, timer - dt * 2f);
        var fwd = head.forward;
        float ang = Vector3.Angle(fwd, dir);
        if (ang > fov) return Mathf.Max(0f, timer - dt * 2f);
        if (Physics.Raycast(o, dir.normalized, out var hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
            return Mathf.Max(0f, timer - dt * 2f);
        return Mathf.Min(timeToCatch, timer + dt);
    }

    void Sweep()
    {
        float span = rightLimit - leftLimit;
        float y = baseYaw + leftLimit + Mathf.PingPong(Time.time * sweepSpeed, span);
        head.localRotation = Quaternion.Euler(0f, y, 0f);
    }

    public void SetJammed(bool value) { jammed = value; }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!head) head = transform;
        Gizmos.color = new Color(1,1,0,0.25f);
        Vector3 a = Quaternion.Euler(0, -fov, 0) * head.forward;
        Vector3 b = Quaternion.Euler(0,  fov, 0) * head.forward;
        Gizmos.DrawRay(head.position, a * viewDistance);
        Gizmos.DrawRay(head.position, b * viewDistance);
        Gizmos.DrawWireSphere(head.position, 0.15f);
    }
#endif
}

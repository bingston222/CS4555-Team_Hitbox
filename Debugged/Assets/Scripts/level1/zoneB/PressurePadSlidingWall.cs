using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class PressurePadSlidingWall : MonoBehaviour
{
    [Header("Wall to Move")]
    [SerializeField] private Transform wall;            // the object that slides (defaults to this.transform if empty)
    [SerializeField] private Vector3 localOpenOffset = new Vector3(0f, 0f, -3f);
    [SerializeField] private float openTime = 1.2f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);

    [Header("Pad Settings")]
    [Tooltip("Player must be tagged 'Player' or assigned via customTag below.")]
    [SerializeField] private string customPlayerTag = "Player";
    [Tooltip("Open only once; if false, it will stay open anyway (no auto-close).")]
    [SerializeField] private bool openOnce = true;

    [Header("Optional: Also require the alarm")]
    [SerializeField] private bool requireAlarm = false;
    [Tooltip("If true: need full alarm (== MaxAlarms). If false: any alarm level > 0.")]
    [SerializeField] private bool requireFullAlarm = true;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Vector3 _closedLocalPos;
    private bool _opened;
    private Collider _col;

    void Reset()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
        if (!wall) wall = transform;  // default
    }

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
        if (!wall) wall = transform;
        _closedLocalPos = wall.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other || !other.gameObject.activeInHierarchy) return;
        if (!string.IsNullOrEmpty(customPlayerTag) && !other.CompareTag(customPlayerTag)) return;

        if (_opened && openOnce) return;

        if (requireAlarm)
        {
            var am = AlarmManager.Instance;
            if (am == null)
            {
                if (debugLogs) Debug.LogWarning("[PressurePadSlidingWall] Alarm required but AlarmManager not found.");
                return;
            }

            bool ok = requireFullAlarm ? (am.AlertLevel >= am.MaxAlarms) : (am.AlertLevel > 0);
            if (!ok)
            {
                if (debugLogs) Debug.Log("[PressurePadSlidingWall] Pad pressed but alarm condition not met.");
                return;
            }
        }

        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        _opened = true;
        if (debugLogs) Debug.Log($"[PressurePadSlidingWall] Opening wall '{wall.name}' from pad '{name}'.");

        Vector3 start = _closedLocalPos;
        Vector3 end   = _closedLocalPos + localOpenOffset;
        float   t     = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, openTime);
            float k = ease.Evaluate(Mathf.Clamp01(t));
            wall.localPosition = Vector3.LerpUnclamped(start, end, k);
            yield return null;
        }
        wall.localPosition = end;

        if (openOnce) _col.enabled = false; // don’t retrigger
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // draw pad bounds
        var c = GetComponent<Collider>() as BoxCollider;
        if (!c) return;
        Gizmos.color = new Color(0f, 1f, 0.6f, 0.25f);
        Matrix4x4 prev = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(c.center, c.size);
        Gizmos.matrix = prev;
    }
#endif

}
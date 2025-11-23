using UnityEngine;
using System.Collections;

public class SlidingWallOnAlarm : MonoBehaviour
{
    [Header("What to move")]
    [SerializeField] private Transform wall;          // the thing that slides; defaults to this.transform

    [Header("Motion")]
    [SerializeField] private Vector3 localOpenOffset = new Vector3(0, 0, -3f); // how far to move (local space)
    [SerializeField] private float openTime = 1.2f;   // seconds
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0,0,1,1);

    [Header("When to open")]
    [Tooltip("Open when we hit AlarmManager.MaxAlarms (full alarm). If false, opens on any alarm > 0.")]
    [SerializeField] private bool requireFullAlarm = true;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Vector3 _closedLocalPos;
    private bool _opened;

    void Reset()
    {
        wall = transform;
    }

    void Awake()
    {
        if (!wall) wall = transform;
        _closedLocalPos = wall.localPosition;
    }

    void OnEnable()
    {
        TrySubscribe();
        if (AlarmManager.Instance == null)
            StartCoroutine(WaitAndSubscribe());
    }

    void OnDisable()
    {
        if (AlarmManager.Instance != null)
            AlarmManager.Instance.OnAlertLevelChanged -= HandleAlertLevel;
    }

    void TrySubscribe()
    {
        if (AlarmManager.Instance != null)
            AlarmManager.Instance.OnAlertLevelChanged += HandleAlertLevel;
    }

    IEnumerator WaitAndSubscribe()
    {
        // In case AlarmManager spawns a little later
        float end = Time.time + 3f;
        while (AlarmManager.Instance == null && Time.time < end)
            yield return null;

        TrySubscribe();
    }

    void HandleAlertLevel(int level)
    {
        if (_opened) return;

        bool shouldOpen = requireFullAlarm
            ? (AlarmManager.Instance != null && level >= AlarmManager.Instance.MaxAlarms)
            : level > 0;

        if (shouldOpen) StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        _opened = true;
        if (debugLogs) Debug.Log($"[SlidingWallOnAlarm] Opening {name}.");

        Vector3 start = _closedLocalPos;
        Vector3 end = _closedLocalPos + localOpenOffset;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, openTime);
            float k = ease.Evaluate(Mathf.Clamp01(t));
            wall.localPosition = Vector3.LerpUnclamped(start, end, k);
            yield return null;
        }
        wall.localPosition = end;
    }

    // Optional helper if you ever want to reset it via code
    public void ResetClosedPosition()
    {
        _opened = false;
        if (wall) wall.localPosition = _closedLocalPos;
    }
}
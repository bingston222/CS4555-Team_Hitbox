using UnityEngine;

public class RedAlertLights : MonoBehaviour
{
    [SerializeField] private Light[] lights;
    [SerializeField] private float flashHz = 4f;

    [Header("Caught Behavior")]
    [Tooltip("If true, all the listed lights go dark when the player is caught.")]
    [SerializeField] private bool goDarkOnCatch = true;

    private bool flashing;
    private bool _subscribed;

    void OnEnable()
    {
        TrySubscribe();
        if (!_subscribed) StartCoroutine(WaitForAlarmThenSubscribe());
    }

    void OnDisable()
    {
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlertLevelChanged -= HandleAlertChanged;
            AlarmManager.Instance.OnPlayerCaught -= HandlePlayerCaught;
        }
        _subscribed = false;
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;
        if (AlarmManager.Instance == null) return;

        AlarmManager.Instance.OnAlertLevelChanged += HandleAlertChanged;
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

    void HandleAlertChanged(int level)
    {
        flashing = (AlarmManager.Instance != null) && (level >= AlarmManager.Instance.MaxAlarms);
    }

    void Update()
    {
        if (lights == null || lights.Length == 0) return;

        if (flashing)
        {
            bool on = Mathf.Repeat(Time.time * flashHz, 1f) < 0.5f;
            foreach (var l in lights) if (l) l.enabled = on;
        }
        else
        {
            foreach (var l in lights) if (l) l.enabled = true;
        }
    }

    private void HandlePlayerCaught()
    {
        if (!goDarkOnCatch) return;

        flashing = false;
        if (lights != null)
            foreach (var l in lights) if (l) l.enabled = false;

        enabled = false;
        // Debug.Log("[RedAlertLights] HandlePlayerCaught fired → going dark", this);
    }
}

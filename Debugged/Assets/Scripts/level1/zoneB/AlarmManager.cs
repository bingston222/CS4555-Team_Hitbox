// AlarmManager.cs (spawn-free)
using System;
using UnityEngine;
using System.Collections.Generic;

public class AlarmManager : MonoBehaviour
{
    public static AlarmManager Instance;

    [Header("Alarm Rules")]
    [Tooltip("Number of detections before full alarm.")]
    [SerializeField] private int maxAlarms = 2;
    public int MaxAlarms => maxAlarms;

    [SerializeField] private float detectionDecay = 12f;

    [Header("Audio")]
    [SerializeField] private AudioClip warningClip;   // for levels < max
    [SerializeField] private AudioClip fullAlarmClip; // for level == max
    [SerializeField] private float alarmVolume = 0.9f;

    [Header("Mute / Debug")]
    [SerializeField] private bool _suppressAlarms = false;
    [SerializeField] private bool debugLogs = true;

    public int AlertLevel { get; private set; } = 0; // 0..maxAlarms

    // Events
    public event Action<int> OnAlertLevelChanged;
    public event Action OnPlayerCaught;               // fires once when first reaching full alarm
    public event Action OnFullAlarmActivated;         // fires once when we FIRST hit full alarm

    private float lastDetectionTime = -Mathf.Infinity;
    private bool hasCaught = false;

    // Always-audible source
    private AudioSource alarmSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        alarmSource = gameObject.AddComponent<AudioSource>();
        alarmSource.playOnAwake = false;
        alarmSource.spatialBlend = 0f;  // 2D
        alarmSource.volume = alarmVolume;
    }

    void Update()
    {
        // decay one step at a time if quiet
        if (AlertLevel > 0 && Time.time - lastDetectionTime > detectionDecay)
        {
            SetAlertLevel(AlertLevel - 1);
            lastDetectionTime = Time.time;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (debugLogs) Debug.Log("[AlarmManager] F9 pressed → forcing FULL ALARM.");
            int prev = AlertLevel;
            SetAlertLevel(MaxAlarms);
            OnAlertLevelChanged?.Invoke(AlertLevel);

            // One-shot broadcast the first time we hit full alarm
            if (prev < MaxAlarms)
            {
                if (debugLogs) Debug.Log("[AlarmManager] Broadcasting OnFullAlarmActivated (F9).");
                OnFullAlarmActivated?.Invoke();
            }

            if (!hasCaught && prev < MaxAlarms)
            {
                hasCaught = true;
                if (debugLogs) Debug.Log("[AlarmManager] Player caught (F9) → broadcasting.");
                OnPlayerCaught?.Invoke();
                ForceAllLightsOff();
            }
        }
#endif
    }

    public void TriggerAlarm(Vector3 atPosition)
    {
        if (_suppressAlarms) { if (debugLogs) Debug.Log("[AlarmManager] Suppressed."); return; }

        lastDetectionTime = Time.time;
        int prevLevel = AlertLevel;

        if (AlertLevel < MaxAlarms) SetAlertLevel(AlertLevel + 1);
        if (debugLogs) Debug.Log($"[AlarmManager] Alarm raised. Level = {AlertLevel}/{MaxAlarms}");

        // Always audible alarm
        var clip = (AlertLevel < MaxAlarms) ? warningClip : fullAlarmClip;
        if (clip)
        {
            if (debugLogs) Debug.Log("[AlarmManager] Playing alarm sound (2D OneShot).");
            alarmSource.volume = alarmVolume;
            alarmSource.PlayOneShot(clip);
        }

        if (AlertLevel >= MaxAlarms)
        {
            if (debugLogs) Debug.Log("[AlarmManager] FULL ALARM reached.");
            OnAlertLevelChanged?.Invoke(AlertLevel);

            // Fire the activation event the *first* time we cross to full
            if (prevLevel < MaxAlarms)
            {
                if (debugLogs) Debug.Log("[AlarmManager] Broadcasting OnFullAlarmActivated.");
                OnFullAlarmActivated?.Invoke();
            }

            if (!hasCaught && prevLevel < MaxAlarms)
            {
                hasCaught = true;
                if (debugLogs) Debug.Log("[AlarmManager] Player caught → broadcasting.");
                OnPlayerCaught?.Invoke();
                ForceAllLightsOff(); // hard kill in case any missed the event
            }
        }
        else
        {
            OnAlertLevelChanged?.Invoke(AlertLevel);
        }
    }

    public void ResetAlarms(bool broadcastReset = true)
    {
        SetAlertLevel(0);
        lastDetectionTime = -Mathf.Infinity;
        hasCaught = false;
        if (broadcastReset && debugLogs) Debug.Log("[AlarmManager] Alarm reset.");
        OnAlertLevelChanged?.Invoke(AlertLevel);
    }

    private void SetAlertLevel(int level)
    {
        level = Mathf.Clamp(level, 0, maxAlarms);
        if (level == AlertLevel) return;
        AlertLevel = level;
        OnAlertLevelChanged?.Invoke(AlertLevel);
    }

    /// Hard shut off all registered light rigs (backup in case of missed events)
    private void ForceAllLightsOff()
    {
        foreach (var s in FindObjectsOfType<SweepingLight>(true))
            s.SendMessage("HandlePlayerCaught", SendMessageOptions.DontRequireReceiver);

        foreach (var r in FindObjectsOfType<RedAlertLights>(true))
            r.SendMessage("HandlePlayerCaught", SendMessageOptions.DontRequireReceiver);
    }

    // === Enemy tracking for camera/alert === (unchanged, optional)
    private readonly HashSet<EnemyRegister> _aliveEnemies = new();
    public int AliveEnemyCount => _aliveEnemies.Count;
    public event Action<int> OnAliveEnemyCountChanged;

    public void RegisterEnemy(EnemyRegister e)
    {
        if (e == null) return;
        if (_aliveEnemies.Add(e))
            OnAliveEnemyCountChanged?.Invoke(_aliveEnemies.Count);
    }

    public void DeregisterEnemy(EnemyRegister e)
    {
        if (e == null) return;
        if (_aliveEnemies.Remove(e))
            OnAliveEnemyCountChanged?.Invoke(_aliveEnemies.Count);
    }
}

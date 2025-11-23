using UnityEngine;
using Unity.Cinemachine;

public class CameraOnCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera mainCam;   // assign VCam_Main
    [SerializeField] private CinemachineCamera wideCam;   // assign VCam_Wide

    [Header("Priorities")]
    [SerializeField] private int mainPriority = 10;
    [SerializeField] private int widePriority = 20;

    [Header("Behavior")]
    [Tooltip("Minimum seconds to stay in wide after combat starts.")]
    [SerializeField] private float minWideSeconds = 3f;
    [Tooltip("Stay wide while any enemies registered are alive.")]
    [SerializeField] private bool stickWhileEnemiesAlive = true;

    [Header("Debug")]
    [SerializeField] private bool logs = true;
    [SerializeField] private bool showHUD = true;

    float wideLatchUntil = -999f;
    bool subscribed;

    void OnEnable()
    {
        TrySubscribe();
        if (!subscribed) StartCoroutine(WaitThenSubscribe());
        // Start in MAIN
        ForceMain("OnEnable");
    }

    void OnDisable()
    {
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnPlayerCaught -= HandleCombatStart;
            AlarmManager.Instance.OnAliveEnemyCountChanged -= HandleEnemyCountChanged;
            AlarmManager.Instance.OnAlertLevelChanged -= HandleAlertChanged;
        }
        subscribed = false;
    }

    void TrySubscribe()
    {
        if (subscribed || AlarmManager.Instance == null) return;
        AlarmManager.Instance.OnPlayerCaught += HandleCombatStart;           // go Wide
        AlarmManager.Instance.OnAliveEnemyCountChanged += HandleEnemyCountChanged; // maybe return
        AlarmManager.Instance.OnAlertLevelChanged += HandleAlertChanged;     // fallback for reset
        subscribed = true;
        if (logs) Debug.Log("[CameraOnCombat] Subscribed to AlarmManager");
    }

    System.Collections.IEnumerator WaitThenSubscribe()
    {
        float end = Time.time + 3f;
        while (!subscribed && Time.time < end) { TrySubscribe(); yield return null; }
    }

    // ----- Events -----
    void HandleCombatStart()
    {
        wideLatchUntil = Time.time + minWideSeconds;
        ForceWide("OnPlayerCaught");
    }

    void HandleEnemyCountChanged(int alive)
    {
        if (logs) Debug.Log($"[CameraOnCombat] Alive enemies = {alive}");
        // Decision is made in LateUpdate continuously
    }

    void HandleAlertChanged(int level)
    {
        // Ignore decay — LateUpdate will decide based on enemies + latch.
        if (logs) Debug.Log($"[CameraOnCombat] Alert level = {level} (ignored for camera)");
    }

    // ----- Core enforcement every frame -----
    void LateUpdate()
    {
        if (!mainCam || !wideCam) return;

        bool inLatch = Time.time < wideLatchUntil;
        bool enemiesAlive = stickWhileEnemiesAlive && AlarmManager.Instance && AlarmManager.Instance.AliveEnemyCount > 0;

        bool shouldBeWide = inLatch || enemiesAlive;

        // Re-apply priorities if they drift
        if (shouldBeWide)
            ForceWide("LateUpdate");
        else
            ForceMain("LateUpdate");
    }

    void ForceWide(string reason)
    {
        if (!mainCam || !wideCam) return;
        if (wideCam.Priority != widePriority || mainCam.Priority != (mainPriority - 1))
        {
            wideCam.Priority = widePriority;
            mainCam.Priority = mainPriority - 1;
            if (logs) LogLive($"WIDE ({reason})");
        }
    }

    void ForceMain(string reason)
    {
        if (!mainCam || !wideCam) return;
        if (mainCam.Priority != mainPriority || wideCam.Priority != (mainPriority - 1))
        {
            mainCam.Priority = mainPriority;
            wideCam.Priority = mainPriority - 1;
            if (logs) LogLive($"MAIN ({reason})");
        }
    }

    void LogLive(string msg)
    {
        var brain = Camera.main ? Camera.main.GetComponent<CinemachineBrain>() : null;
        var live = brain ? brain.ActiveVirtualCamera : null;
        Debug.Log($"[CameraOnCombat] -> {msg}. Live={live?.Name ?? "null"}, main={mainCam.Priority}, wide={wideCam.Priority}");
    }

    // Small on-screen HUD so you can see state while playing
    void OnGUI()
    {
        if (!showHUD) return;
        var brain = Camera.main ? Camera.main.GetComponent<CinemachineBrain>() : null;
        var live = brain ? brain.ActiveVirtualCamera : null;
        int alive = AlarmManager.Instance ? AlarmManager.Instance.AliveEnemyCount : -1;
        var rect = new Rect(8, 8, 420, 60);
        GUI.Box(rect, "");
        GUI.Label(new Rect(16, 12, 400, 20), $"Live: {live?.Name ?? "null"}");
        GUI.Label(new Rect(16, 30, 400, 20), $"Enemies Alive: {alive}  |  Latch for {(Mathf.Max(0f, wideLatchUntil - Time.time)):0.0}s");
    }
}

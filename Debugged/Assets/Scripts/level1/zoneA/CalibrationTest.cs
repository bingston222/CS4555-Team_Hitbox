using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple keys-based calibration: both players must
/// 1) Walk   (P1: WASD, P2: Arrows)
/// 2) Run    (P1: Left Shift, P2: Right Shift)
/// 3) Jump   (P1: F, P2: /)
///
/// Exposes:
/// - StartCalibration()  -> begins the flow
/// - OnCalibrationStarted
/// - OnCalibrationCompleted
///
/// Hook this up to your IntroController and/or CalibrationDriver.
/// </summary>
public class CalibrationTest : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnCalibrationStarted;
    public UnityEvent OnCalibrationCompleted;

    [Header("Step Prompts (optional)")]
    public UnityEvent OnStepWalk;   // fires when Walk step begins
    public UnityEvent OnStepRun;    // fires when Run step begins
    public UnityEvent OnStepJump;   // fires when Jump step begins

    [Header("Settings")]
    [Tooltip("If true, StartCalibration() is called on Start() for quick testing.")]
    public bool autoStartInPlaymode = false;

    [Tooltip("Maximum time to wait on a step before timing out (0 = no timeout).")]
    public float stepTimeout = 0f;

    // Runtime flags
    bool isRunning;

    // Per-step completion flags
    bool p1Walked, p2Walked;
    bool p1Ran,    p2Ran;
    bool p1Jumped, p2Jumped;

    void Start()
    {
        if (autoStartInPlaymode)
            StartCalibration();
    }

    public void StartCalibration()
    {
        if (isRunning) return;
        ResetFlags();

        OnCalibrationStarted?.Invoke();
        StartCoroutine(RunCalibration());
    }

    void ResetFlags()
    {
        isRunning = false;

        p1Walked = p2Walked = false;
        p1Ran    = p2Ran    = false;
        p1Jumped = p2Jumped = false;
    }

    IEnumerator RunCalibration()
    {
        isRunning = true;

        // --- STEP 1: WALK ---
        OnStepWalk?.Invoke();
        yield return WaitForBoth(
            // P1 walk: any WASD
            () => p1Walked = p1Walked || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                                            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D),
            // P2 walk: any Arrows
            () => p2Walked = p2Walked || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
                                            Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow),
            () => p1Walked, () => p2Walked
        );

        // --- STEP 2: RUN ---
        OnStepRun?.Invoke();
        yield return WaitForBoth(
            // P1 run: Left Shift
            () => p1Ran = p1Ran || Input.GetKeyDown(KeyCode.LeftShift),
            // P2 run: Right Shift
            () => p2Ran = p2Ran || Input.GetKeyDown(KeyCode.RightShift),
            () => p1Ran, () => p2Ran
        );

        // --- STEP 3: JUMP ---
        OnStepJump?.Invoke();
        yield return WaitForBoth(
            // P1 jump: F
            () => p1Jumped = p1Jumped || Input.GetKeyDown(KeyCode.F),
            // P2 jump: Slash (/)
            () => p2Jumped = p2Jumped || Input.GetKeyDown(KeyCode.Slash),
            () => p1Jumped, () => p2Jumped
        );

        isRunning = false;
        OnCalibrationCompleted?.Invoke();
    }

    IEnumerator WaitForBoth(System.Action pollP1, System.Action pollP2, System.Func<bool> doneP1, System.Func<bool> doneP2)
    {
        float t = 0f;
        while (!(doneP1() && doneP2()))
        {
            pollP1?.Invoke();
            pollP2?.Invoke();

            if (stepTimeout > 0f)
            {
                t += Time.deltaTime;
                if (t >= stepTimeout)
                {
                    // If you want to fail out, you could break or log a warning here.
                    // For now, we allow “timeout equals success” so players aren’t soft-locked:
                    break;
                }
            }
            yield return null;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Start Calibration Now")]
    void __StartNow() => StartCalibration();
#endif
}

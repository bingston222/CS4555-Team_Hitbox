using UnityEngine;
using UnityEngine.Events;

public class CalibrationTest : MonoBehaviour
{
    public UnityEvent OnCalibrationStarted;
    public UnityEvent OnCalibrationCompleted;

    // Step start prompts (fire once)
    public UnityEvent OnStepWalk;
    public UnityEvent OnStepRun;
    public UnityEvent OnStepJump;

    // Step done (HUD checkmarks)
    public UnityEvent OnStepWalkDone;
    public UnityEvent OnStepRunDone;
    public UnityEvent OnStepJumpDone;

    public bool autoStartInPlaymode = false;

    // Per-player completion flags (reset each run)
    bool p1Walked, p2Walked;
    bool p1Ran,    p2Ran;
    bool p1Jumped, p2Jumped;

    enum Step { Idle, Walk, Run, Jump, Done }
    Step step = Step.Idle;

    void Start()
    {
        if (autoStartInPlaymode) StartCalibration();
    }

    public void StartCalibration()
    {
        // reset flags every time we start
        p1Walked = p2Walked = p1Ran = p2Ran = p1Jumped = p2Jumped = false;

        step = Step.Walk;
        OnCalibrationStarted?.Invoke(); // your single VO prompt
        OnStepWalk?.Invoke();           // (optional) a short “Walk now” ping
    }

    void Update()
    {
        switch (step)
        {
            case Step.Walk:
                if (WalkPressedP1_ThisFrame()) p1Walked = true;
                if (WalkPressedP2_ThisFrame()) p2Walked = true;

                if (p1Walked && p2Walked)
                {
                    OnStepWalkDone?.Invoke();
                    step = Step.Run;
                    OnStepRun?.Invoke();
                }
                break;

            case Step.Run:
                if (RunPressedP1_ThisFrame()) p1Ran = true;
                if (RunPressedP2_ThisFrame()) p2Ran = true;

                if (p1Ran && p2Ran)
                {
                    OnStepRunDone?.Invoke();
                    step = Step.Jump;
                    OnStepJump?.Invoke();
                }
                break;

            case Step.Jump:
                if (JumpPressedP1_ThisFrame()) p1Jumped = true;
                if (JumpPressedP2_ThisFrame()) p2Jumped = true;

                if (p1Jumped && p2Jumped)
                {
                    OnStepJumpDone?.Invoke();
                    step = Step.Done;
                    OnCalibrationCompleted?.Invoke();
                }
                break;
        }
    }

    // ---------- INPUT HELPERS (edge-press this frame) ----------
    // Walk = any movement key for that player this frame
    bool WalkPressedP1_ThisFrame() =>
    Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);

bool WalkPressedP2_ThisFrame() =>
    Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.J) ||
    Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L);

bool RunPressedP1_ThisFrame() => Input.GetKeyDown(KeyCode.C);
bool RunPressedP2_ThisFrame() => Input.GetKeyDown(KeyCode.N);

bool JumpPressedP1_ThisFrame() => Input.GetKeyDown(KeyCode.F);
bool JumpPressedP2_ThisFrame() => Input.GetKeyDown(KeyCode.H);

}

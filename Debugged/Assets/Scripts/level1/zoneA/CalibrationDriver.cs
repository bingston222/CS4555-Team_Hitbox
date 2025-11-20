using UnityEngine;

public class CalibrationDriver : MonoBehaviour
{
    [SerializeField] SceneBeats beats;
    [SerializeField] CalibrationTest calibration;
    public bool lockControlsDuringGuide = true;

    void OnEnable()
    {
        calibration.OnCalibrationStarted.AddListener(OnCalibStart);
        calibration.OnCalibrationCompleted.AddListener(OnCalibComplete);
    }
    void OnDisable()
    {
        calibration.OnCalibrationStarted.RemoveListener(OnCalibStart);
        calibration.OnCalibrationCompleted.RemoveListener(OnCalibComplete);
    }

    void OnCalibStart()
    {
        if (lockControlsDuringGuide) TogglePlayers(false);
        StartCoroutine(RunIntroAndGuide());
    }

    System.Collections.IEnumerator RunIntroAndGuide()
{
    // One-time radio chatter (won’t replay)
    yield return StartCoroutine(beats.PlayOnce("calibrationIntro", beats.calibrationIntro));

    // Controls guide (also one-shot)
    var guided = BuildControlGuide(beats.calibrationGuide);
    yield return StartCoroutine(beats.PlayOnce("calibrationGuide", guided));
}

System.Collections.IEnumerator CalibCompleteSequence()
{
    // Say "Calibration complete" (one-shot)
    yield return StartCoroutine(beats.PlayOnce("calibrationDone", beats.calibrationDone));

    // Small pause
    yield return new WaitForSeconds(0.4f);

    // Door instructions (one-shot)
    yield return StartCoroutine(beats.PlayOnce("doorIntro", beats.doorIntro));

    // Re-enable controls if locked
    if (lockControlsDuringGuide) TogglePlayers(true);
}


    void OnCalibComplete()
{
    StartCoroutine(CalibCompleteSequence());
}



    void TogglePlayers(bool enabled)
    {
        foreach (var pi in FindObjectsOfType<UnityEngine.InputSystem.PlayerInput>())
            pi.enabled = enabled;
    }

    UnifiedDialogueController.DialogueLine[] BuildControlGuide(
        UnifiedDialogueController.DialogueLine[] template)
    {
        string p1Walk="WASD", p1Run="Left Shift", p1Jump="F";
        string p2Walk="Arrow Keys", p2Run="Right Shift", p2Jump="/";

        var arr = new UnifiedDialogueController.DialogueLine[template.Length];
        for (int i = 0; i < template.Length; i++)
        {
            var src = template[i];
            arr[i] = new UnifiedDialogueController.DialogueLine{
                characterIcon   = src.characterIcon,
                text            = (src.text ?? "")
                    .Replace("{P1_WALK}", p1Walk).Replace("{P1_RUN}", p1Run).Replace("{P1_JUMP}", p1Jump)
                    .Replace("{P2_WALK}", p2Walk).Replace("{P2_RUN}", p2Run).Replace("{P2_JUMP}", p2Jump),
                voiceClip       = src.voiceClip,
                waitForInput    = src.waitForInput,
                autoHold        = src.autoHold,
                typewriterSpeed = src.typewriterSpeed,
                panelFade       = src.panelFade,
                textFade        = src.textFade
            };
        }
        return arr;
    }
}

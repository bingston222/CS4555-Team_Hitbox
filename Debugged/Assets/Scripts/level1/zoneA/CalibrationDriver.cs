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
        // 1) Radio chatter / context
        yield return StartCoroutine(beats.Play(beats.calibrationIntro));

        // 2) Controls (placeholders replaced)
        var guided = BuildControlGuide(beats.calibrationGuide);
        yield return StartCoroutine(beats.Play(guided));
    }

    void OnCalibComplete()
{
    StartCoroutine(CalibCompleteSequence());
}

System.Collections.IEnumerator CalibCompleteSequence()
{
    // 1) say "Calibration complete"
    yield return StartCoroutine(beats.Play(beats.calibrationDone));

    // 2) tiny pause (optional)
    yield return new WaitForSeconds(0.4f);

    // 3) say door instructions
    yield return StartCoroutine(beats.Play(beats.doorIntro));

    // 4) re-enable controls if you locked them
    if (lockControlsDuringGuide) TogglePlayers(true);
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

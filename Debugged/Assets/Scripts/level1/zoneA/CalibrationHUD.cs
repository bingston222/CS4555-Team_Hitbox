using UnityEngine;
using TMPro;

public class CalibrationHUD : MonoBehaviour
{
    public CalibrationTest calibration;

    [Header("UI")]
    public GameObject root;           // <-- drag your Panel here
    public TextMeshProUGUI title;
    public TextMeshProUGUI walkLine;
    public TextMeshProUGUI runLine;
    public TextMeshProUGUI jumpLine;

    Color dim, done;

    void Awake()
    {
        dim  = new Color(1,1,1,0.55f);
        done = new Color(0.6f,1f,0.6f,1f);
        if (title) title.text = "Calibration";
        ResetLines();

        // IMPORTANT: keep this component enabled so it can subscribe.
        // Only hide the visual root (the Panel).
        if (root) root.SetActive(false);
    }

    void OnEnable()
    {
        if (!calibration) return;
        calibration.OnCalibrationStarted.AddListener(Show);

        calibration.OnStepWalk.AddListener(()=>Highlight(walkLine));
        calibration.OnStepRun.AddListener(()=>Highlight(runLine));
        calibration.OnStepJump.AddListener(()=>Highlight(jumpLine));

        calibration.OnStepWalkDone.AddListener(()=>SetDone(walkLine));
        calibration.OnStepRunDone.AddListener(()=>SetDone(runLine));
        calibration.OnStepJumpDone.AddListener(()=>SetDone(jumpLine));

        calibration.OnCalibrationCompleted.AddListener(Hide);
    }
    void OnDisable()
    {
        if (!calibration) return;
        calibration.OnCalibrationStarted.RemoveListener(Show);
        calibration.OnCalibrationCompleted.RemoveListener(Hide);
        calibration.OnStepWalk.RemoveAllListeners();
        calibration.OnStepRun.RemoveAllListeners();
        calibration.OnStepJump.RemoveAllListeners();
        calibration.OnStepWalkDone.RemoveAllListeners();
        calibration.OnStepRunDone.RemoveAllListeners();
        calibration.OnStepJumpDone.RemoveAllListeners();
    }

    void Show() { if (root) root.SetActive(true); }
    void Hide() { if (root) root.SetActive(false); }

    void ResetLines()
    {
        if (walkLine) walkLine.text = "Walk — WASD / IJKL";
        if (runLine)  runLine.text  = "Run — C / N";
        if (jumpLine) jumpLine.text = "Jump — F / H";
        if (walkLine) walkLine.color = dim;
        if (runLine)  runLine.color  = dim;
        if (jumpLine) jumpLine.color = dim;
    }
    void Highlight(TextMeshProUGUI line)
    {
        if (!line) return;
        line.color = Color.white;
        if (!line.text.StartsWith("▶ ")) line.text = "▶ " + line.text;
    }
    void SetDone(TextMeshProUGUI line)
    {
        if (!line) return;
        line.color = done;
        line.text = "✓ " + line.text.Replace("▶ ","");
    }
}

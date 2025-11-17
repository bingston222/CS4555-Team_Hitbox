using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MovementTutorial : MonoBehaviour
{
    public static MovementTutorial Instance;

    [Header("Players")]
    public PlayerInput player1;
    public PlayerInput player2;

    [Header("UI")]
    public CanvasGroup promptGroup;    // panel with CanvasGroup
    public TextMeshProUGUI promptText; // TMP text inside it

    [Header("Timings")]
    public float fade = 0.2f;
    public float minMoveTime = 0.15f;  // require a tiny bit of time moving

    [Header("Dialogue")]
    [TextArea] public string[] banterAfterMove = {
        "This isn’t so bad.",
        "Okay, I’m getting the hang of this.",
        "Tracks feel grippy… in a digital way.",
    };
    [TextArea] public string[] banterAfterJump = {
        "Nice airtime!",
        "Jump feels clean.",
        "Who knew the Boot Sector had hops?"
    };

    // Optional name tags/colors (TextMeshPro supports rich text)
    public string systemPrefix = "<color=#7FBFFF>[SYSTEM]</color> ";
    public string p1Prefix     = "<color=#FFD166>[You]</color> ";
    public string p2Prefix     = "<color=#A0E7E5>[Partner]</color> ";

    void Awake()
    {
        Instance = this;
        if (promptGroup) promptGroup.alpha = 0f; // start hidden
    }

    public void StartTutorial() => StartCoroutine(RunTutorial());

    // -------- main flow --------
    IEnumerator RunTutorial()
    {
        // SYSTEM: intro
        SaySystem("System calibration engaged… follow the prompts.", 1.2f);

        // STEP 1: Move
        yield return ShowPrompt("Try moving left and right…");
        yield return WaitForBothMovedHorizontal();
        SayPlayerRandom(banterAfterMove, 1.1f);

        // STEP 2: Jump
        yield return ShowPrompt("Now jump!");
        yield return WaitForBothJump();
        SayPlayerRandom(banterAfterJump, 1.0f);

        // STEP 3: Confirm
        yield return ShowPrompt("Great! Calibration looks good.", 0.9f);
        yield return HidePrompt();

        // Hand off to basic combat
        SaySystem("Loading combat check…", 0.9f);

        var intro = FindObjectOfType<IntroController>();
        if (intro)
        {
            intro.SpawnCombatOrbs();
            SaySystem("Combat check: give those test orbs a tap.", 1.4f);
        }
    }

    // -------- UI helpers --------
    IEnumerator ShowPrompt(string text, float hold = 0f)
    {
        if (!promptGroup || !promptText) yield break;
        promptText.text = text;
        yield return FadeCanvas(promptGroup, 0f, 1f, fade);
        if (hold > 0f) yield return new WaitForSeconds(hold);
    }

    IEnumerator HidePrompt()
    {
        if (!promptGroup) yield break;
        yield return FadeCanvas(promptGroup, 1f, 0f, fade);
    }

    IEnumerator FadeCanvas(CanvasGroup g, float a, float b, float t)
    {
        float e = 0f; g.alpha = a; g.interactable = g.blocksRaycasts = (b > a);
        while (e < t) { e += Time.deltaTime; g.alpha = Mathf.Lerp(a, b, e/t); yield return null; }
        g.alpha = b; g.interactable = g.blocksRaycasts = (b > 0.001f);
    }

    // -------- input waits --------
    IEnumerator WaitForBothMovedHorizontal()
    {
        bool p1done = false, p2done = false;
        float p1time = 0f, p2time = 0f;

        while (!p1done || !p2done)
        {
            Vector2 m1 = ReadVector(player1, "Move");
            Vector2 m2 = ReadVector(player2, "Move");

            if (Mathf.Abs(m1.x) > 0.2f) p1time += Time.deltaTime; else p1time = 0f;
            if (Mathf.Abs(m2.x) > 0.2f) p2time += Time.deltaTime; else p2time = 0f;

            p1done = p1time >= minMoveTime;
            p2done = p2time >= minMoveTime;
            yield return null;
        }
    }

    IEnumerator WaitForBothJump()
    {
        bool p1done = false, p2done = false;
        while (!p1done || !p2done)
        {
            p1done = p1done || WasPressedThisFrame(player1, "Jump");
            p2done = p2done || WasPressedThisFrame(player2, "Jump");
            yield return null;
        }
    }

    // -------- input helpers --------
    Vector2 ReadVector(PlayerInput pi, string actionName)
    {
        if (!pi) return Vector2.zero;
        var a = pi.actions[actionName];
        return a != null ? a.ReadValue<Vector2>() : Vector2.zero;
    }

    bool WasPressedThisFrame(PlayerInput pi, string actionName)
    {
        if (!pi) return false;
        var a = pi.actions[actionName];
        return a != null && a.WasPressedThisFrame();
    }

    // -------- dialogue helpers --------
    void SaySystem(string line, float hold = 1.6f, float fade = 0.18f)
    {
        if (Adialogue.Instance)
            Adialogue.Instance.StartCoroutine(
                Adialogue.Instance.ShowSubtitle(systemPrefix + line, hold, fade)
            );
    }

    void SayPlayerRandom(string[] pool, float hold = 1.3f, float fade = 0.18f)
    {
        if (pool == null || pool.Length == 0 || Adialogue.Instance == null) return;
        string line = pool[Random.Range(0, pool.Length)];
        string prefix = (Random.value < 0.5f) ? p1Prefix : p2Prefix;
        Adialogue.Instance.StartCoroutine(
            Adialogue.Instance.ShowSubtitle(prefix + line, hold, fade)
        );
    }
}

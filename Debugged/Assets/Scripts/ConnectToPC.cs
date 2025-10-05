using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ConnectToPC : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode p1Key = KeyCode.E;             // WASD player
    public KeyCode p2Key = KeyCode.RightShift;    // Arrow-keys player

    [Header("Detection (no triggers needed)")]
    public float radius = 1.8f;                   // horizontal distance from PC
    public float maxVertical = 1.2f;              // max Y difference allowed
    public LayerMask playerMask = ~0;             // players’ layer (Everything is fine)

    [Header("UI")]
    public TextMeshProUGUI topBanner;             // big “Goal” text (optional)
    public TextMeshProUGUI bottomPrompt;          // bottom prompt label (PromptText)

    [Header("FX (optional)")]
    public AudioSource connectSfx;
    public Animator pcAnimator;
    public string animatorTrigger = "Crash";

    [Header("Events")]
    public UnityEvent onP1Connected;
    public UnityEvent onP2Connected;
    public UnityEvent onBothConnected;

    [Header("Debug")]
    public bool startListeningOnPlay = false;     // tick to test without wiring GameState

    bool listening;
    readonly bool[] connected = new bool[2];

    // Disable these scene-wide during the PC step so nothing clears the bottom prompt
    readonly string[] toDisableByName = {
        "Interactor", "SpotInteractable", "InteractingPromptUI",
        "ControllerPickup", "DialogueManager"
    };
    readonly List<Behaviour> worldDisabled = new();

    void Start()
    {
        if (startListeningOnPlay)
        {
            listening = true;
            BeginPCStepTakeover();
            if (topBanner) topBanner.text = "Goal: Connect both controllers";
            UpdateUI(false, false); // show “Stand at PC” immediately
        }

        // Your project: public event System.Action<int> OnBothFound;
        if (GameState.I != null)
            GameState.I.OnBothFound += BeginListeningParam;
    }

    void OnDestroy()
    {
        if (GameState.I != null)
            GameState.I.OnBothFound -= BeginListeningParam;

        RestoreWorldInteractions();
    }

    // GameState -> when both controllers are found
    public void BeginListeningParam(int _) => BeginListening();

    public void BeginListening()
    {
        listening = true;
        connected[0] = connected[1] = false;

        // Hide any dialogue bubble that might be up
        if (DialogueManager.I && DialogueManager.I.bubble)
            DialogueManager.I.bubble.gameObject.SetActive(false);

        BeginPCStepTakeover();

        if (topBanner) topBanner.text = "Goal: Connect both controllers";
        UpdateUI(false, false); // keep “Stand at PC” visible from the start
    }

    void Update()
    {
        if (!listening) return;

        // Who is near the PC? (no triggers required)
        var hits = Physics.OverlapSphere(transform.position, radius, playerMask, QueryTriggerInteraction.Ignore);
        bool p1Near = false, p2Near = false;

        foreach (var h in hits)
        {
            if (!TryGetPlayerIndex(h, out int idx)) continue;
            if (Mathf.Abs(h.transform.position.y - transform.position.y) > maxVertical) continue;
            if (idx == 0) p1Near = true; else if (idx == 1) p2Near = true;
        }

        // Inputs per player
        if (p1Near && !connected[0] && Input.GetKeyDown(p1Key))
        {
            connected[0] = true;
            onP1Connected?.Invoke();
            if (connectSfx) connectSfx.Play();
        }
        if (p2Near && !connected[1] && Input.GetKeyDown(p2Key))
        {
            connected[1] = true;
            onP2Connected?.Invoke();
            if (connectSfx) connectSfx.Play();
        }

        // Update the bottom text EVERY frame so nothing else can “win”
        UpdateUI(p1Near, p2Near);

        // Finished?
        if (connected[0] && connected[1])
        {
            listening = false;

            if (pcAnimator && !string.IsNullOrEmpty(animatorTrigger))
                pcAnimator.SetTrigger(animatorTrigger);

            onBothConnected?.Invoke();

            if (bottomPrompt) bottomPrompt.text = "Both connected! ✅";

            RestoreWorldInteractions();
        }
    }

    // ---------- take over / restore ----------
    void BeginPCStepTakeover()
    {
        worldDisabled.Clear();
        var all = GameObject.FindObjectsOfType<Behaviour>(true);
        foreach (var b in all)
        {
            if (b == null || !b.enabled) continue;

            // don’t disable self/children
            if (b.transform.IsChildOf(transform)) continue;

            // keep the label’s own components alive
            if (bottomPrompt && (b.transform == bottomPrompt.transform || b.transform.IsChildOf(bottomPrompt.transform)))
                continue;

            string n = b.GetType().Name;
            for (int i = 0; i < toDisableByName.Length; i++)
            {
                if (n == toDisableByName[i])
                {
                    b.enabled = false;
                    worldDisabled.Add(b);
                    break;
                }
            }
        }
    }

    void RestoreWorldInteractions()
    {
        foreach (var b in worldDisabled)
            if (b) b.enabled = true;
        worldDisabled.Clear();
    }

    // ---------- UI (persists until done) ----------
    void UpdateUI(bool p1Near, bool p2Near)
    {
        if (!bottomPrompt) return;

        // Stand → Press → Connected per player
        string p1 = connected[0] ? "P1: Connected ✓" :
                    (p1Near ? $"P1: Press {p1Key}" : "P1: Stand at PC");

        string p2 = connected[1] ? "P2: Connected ✓" :
                    (p2Near ? $"P2: Press {p2Key}" : "P2: Stand at PC");

        bottomPrompt.text = $"{p1}\n{p2}";
    }

    // ---------- PlayerId lookup (works even if the field/property name differs) ----------
    bool TryGetPlayerIndex(Component c, out int index)
    {
        index = 0;
        if (!c) return false;

        var id = c.GetComponentInParent<PlayerId>();
        if (id == null) return false;

        object boxed = id;
        var t = boxed.GetType();
        string[] names = { "PlayerIndex","playerIndex","PlayerId","playerId","Index","index","Id","id","playerNumber","PlayerNumber" };

        foreach (var n in names)
        {
            var f = t.GetField(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null && f.FieldType == typeof(int)) { index = Normalize((int)f.GetValue(boxed)); return true; }
            var p = t.GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.PropertyType == typeof(int)) { index = Normalize((int)p.GetValue(boxed, null)); return true; }
        }
        return false;
    }

    int Normalize(int idx) { if (idx >= 1 && idx <= 2) idx -= 1; return Mathf.Clamp(idx, 0, 1); }

    // Visualize radius in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

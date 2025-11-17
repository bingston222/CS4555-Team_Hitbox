using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class ConnectToPC : MonoBehaviour
{
    [Header("Players (drag their Transforms)")]
    public Transform p1;
    public Transform p2;

    [Header("Keys")]
    public KeyCode p1Key = KeyCode.E;
    public KeyCode p2Key = KeyCode.Slash;

    [Header("Detection")]
    public float radius = 7.5f;
    public float maxVertical = 7f;

    [Header("Bottom Bar UI (grey panel)")]
    public CanvasGroup bottomGroup;     // CanvasGroup on PCBottomPanel
    public TMP_Text bottomPrompt;       // PCBottomText

    [Header("Goal UI")]
    [SerializeField] GameObject goalBar;   // Drag the GoalBar or GoalText GameObject here


    [Header("Behavior")]
    public bool startListeningOnPlay = false;
    [SerializeField] float hideDelayAfterBoth = 0.35f;

    [Header("Search cleanup (optional)")]
    [Tooltip("If true, disables all SpotInteractable components after both players connect so Search prompts never appear again.")]
    public bool disableAllSearchAfterPC = true;
    [Tooltip("Optional root transform to limit the search disabling (leave empty to scan whole scene).")]
    public Transform searchRoot;

    [Header("Events (optional)")]
    public UnityEvent onP1Connected;
    public UnityEvent onP2Connected;
    public UnityEvent onBothConnected;

    // runtime state
    public bool IsListening { get; private set; }
    public bool P1Connected { get; private set; }
    public bool P2Connected { get; private set; }
    public bool P1Near { get; private set; }
    public bool P2Near { get; private set; }
    private bool bothAnnounced = false;   // guard so we don't fire twice


    bool bothPhaseCompleted;    // becomes true once both are connected (used in StopListening)

    void Start()
    {
        SetBottomAlpha(0f);
        bottomPrompt?.SetText(string.Empty);
        if (startListeningOnPlay) BeginListening();
    }

    // Hook into your GameState “both found” so this step begins right after searching is done
    void OnEnable()
    {
        if (GameState.I != null) GameState.I.OnBothFound += HandleBothFound;
        else StartCoroutine(WaitAndHook());
    }
    void OnDisable()
    {
        if (GameState.I != null) GameState.I.OnBothFound -= HandleBothFound;
    }
    IEnumerator WaitAndHook()
    {
        while (GameState.I == null) yield return null;
        GameState.I.OnBothFound += HandleBothFound;
    }
    void HandleBothFound(int _lastFinder)
    {
        BeginListening();
    }

    // ── Public control ─────────────────────────────────────────────
    public void BeginListening()
    {
        bothPhaseCompleted = false;

        // Hide any lingering Search prompt immediately
        InteractionPromptUI.ClearNow();

        IsListening = true;
        P1Connected = false;
        P2Connected = false;

        SetBottomAlpha(1f);
        UpdatePrompt();
    }

    void Update()
{
    if (!IsListening) return;

    // 1) Proximity checks
    P1Near = IsNear(p1);
    P2Near = IsNear(p2);

    // 2) Key to connect (only if near and not already connected)
    if (P1Near && !P1Connected && Input.GetKeyDown(p1Key))
    {
        P1Connected = true;
        onP1Connected?.Invoke();
    }

    if (P2Near && !P2Connected && Input.GetKeyDown(p2Key))
    {
        P2Connected = true;
        onP2Connected?.Invoke();
    }

    // 3) Refresh the bottom prompt every frame
    UpdatePrompt();

    // 4) When BOTH are connected, show a short "Both connected ✓",
    //    hide the goal bar, then fade the bottom bar after a delay.
    if (P1Connected && P2Connected && !bothAnnounced)
    {
        bothAnnounced = true;

        onBothConnected?.Invoke();

        // Optional: briefly show a combined success line
        if (bottomPrompt) bottomPrompt.text = "Both connected ✓";

        // Hide the goal banner now
        if (goalBar) goalBar.SetActive(false);

        // Then fade out the bottom bar after a short delay
        CancelInvoke(nameof(StopListening));
        Invoke(nameof(StopListening), hideDelayAfterBoth);
    }
}


    // ── UI text ───────────────────────────────────────────────────
    void UpdatePrompt()
    {
        if (!bottomPrompt) return;

        string l1 = !P1Connected
            ? (!P1Near ? "P1: Stand at PC" : $"P1: Press {KeyLabel(p1Key)}")
            : "P1: Connected ✓";

        string l2 = !P2Connected
            ? (!P2Near ? "P2: Stand at PC" : $"P2: Press {KeyLabel(p2Key)}")
            : "P2: Connected ✓";

        bottomPrompt.text = $"{l1}\n{l2}";
    }

    // ── End of step / cleanup ─────────────────────────────────────
    void StopListening()
    {
        IsListening = false;

        // 1) Clear any active interaction prompt (safety)
        InteractionPromptUI.ClearNow();

        // 2) Optionally disable all search spots so Search prompts never reappear
        if (disableAllSearchAfterPC)
            DisableAllSearchSpots();

        // 3) Hide the bottom bar and clear text
        SetBottomAlpha(0f);
        bottomPrompt?.SetText(string.Empty);
    }

    void DisableAllSearchSpots()
    {
        // If you used HideAllGlowsOnBothFound earlier, this reproduces the effect here.
        // It disables SpotInteractable and their collider, and hides an optional glowRoot.

        // choose scope
        if (searchRoot)
        {
            var spots = searchRoot.GetComponentsInChildren<SpotInteractable>(true);
            foreach (var s in spots)
            {
                // hide glow if present
                if (s && s.glowRoot) s.glowRoot.SetActive(false);

                // disable collider so it can’t be detected
                var col = s ? s.GetComponent<Collider>() : null;
                if (col) col.enabled = false;

                // disable the script (extra safety so it won’t call the UI)
                s.enabled = false;
            }
        }
        else
        {
            var spots = FindObjectsOfType<SpotInteractable>(true);
            foreach (var s in spots)
            {
                if (s && s.glowRoot) s.glowRoot.SetActive(false);
                var col = s ? s.GetComponent<Collider>() : null;
                if (col) col.enabled = false;
                s.enabled = false;
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────
    bool IsNear(Transform t)
    {
        if (!t) return false;
        Vector3 a = t.position, b = transform.position;
        float horiz = Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
        float vert = Mathf.Abs(a.y - b.y);
        return horiz <= radius && vert <= maxVertical;
    }

    void SetBottomAlpha(float a)
    {
        if (!bottomGroup) return;
        bottomGroup.alpha = a;
        // keep raycasts off so this panel never blocks world clicks
        bottomGroup.blocksRaycasts = a > 0.01f;
        bottomGroup.interactable   = false;
    }

    string KeyLabel(KeyCode kc)
    {
        return kc == KeyCode.Slash ? "/" :
               kc == KeyCode.Return ? "Enter" :
               kc.ToString();
    }
}

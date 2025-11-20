using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class UnifiedDialogueController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        [Header("Who speaks / how it looks")]
        public Sprite characterIcon;
        [TextArea(2, 4)] public string text;

        [Header("Audio (optional)")]
        public AudioClip voiceClip;      // if null, falls back to defaultBlip

        [Header("Flow")]
        public bool waitForInput = true; // if true: waits for player (Space/Next) after typing
        public float autoHold = 1.6f;    // if waitForInput == false, hold this long then advance

        [Header("Per-line overrides (optional)")]
        public float typewriterSpeed = -1f; // chars/sec; <=0 uses global
        public float panelFade = -1f;       // seconds; <=0 uses global
        public float textFade = -1f;        // seconds; <=0 uses global
    }

    //public static UnifiedDialogueController Instance;

    [Header("Dialogue Data")]
    [SerializeField] private DialogueLine[] lines;

    [Header("UI References")]
    [SerializeField] private CanvasGroup subtitleGroup;     // your panel group
    [SerializeField] private Image iconUI;                  // speaker head
    [SerializeField] private TextMeshProUGUI subtitleText;  // line text

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;       // shared source
    [SerializeField] private AudioClip crackleClip;         // optional intro SFX
    [SerializeField] private AudioClip defaultBlip;         // fallback clip
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Behaviour")]
    [SerializeField] private bool autoStart = false;
    [Tooltip("Characters per second when typing")]
    [SerializeField] private float typewriterCps = 50f; // 50 cps ~ 0.02s per char
    [SerializeField] private float panelFadeSeconds = 0.25f;
    [SerializeField] private float textFadeSeconds = 0.20f;

    [Header("Events")]
    public UnityEvent OnConversationFinished;

    // runtime
    private int index = 0;
    private bool isTyping = false;
    private Coroutine running;
    private string fullCurrentText = "";


    private void Awake()
    {
        //Instance = this;
        if (subtitleGroup) subtitleGroup.alpha = 0f;
        if (subtitleText) subtitleText.text = "";
        if (iconUI) iconUI.enabled = false;
    }

    private void OnEnable()
    {
        if (autoStart) StartConversation();
    }

    private void OnDisable()
    {
        if (running != null) { StopCoroutine(running); running = null; }
        if (subtitleText) subtitleText.text = "";
        if (subtitleGroup) subtitleGroup.alpha = 0f;
        if (iconUI) iconUI.enabled = false;
        isTyping = false;
        index = 0;
    }

    // ===================== Public API =====================
    public void StartConversation()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(RunConversation());
    }

    public void Next() // optional UI Button hook
    {
        HandleAdvanceInput();
    }
    // ======================================================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleAdvanceInput();
        }
    }

    private void HandleAdvanceInput()
    {
        if (isTyping)
        {
            // Finish the line instantly
            isTyping = false;
        }
        else
        {
            // Go to next line only if we are on a waitForInput line (or idle)
            if (running == null) return;
            // Allow the coroutine to continue
            _advanceSignal = true;
        }
    }

    private IEnumerator RunConversation()
    {
        // Safety
        if (subtitleGroup == null || subtitleText == null)
            yield break;

        // Optional crackle lead-in
        if (audioSource && crackleClip)
        {
            audioSource.PlayOneShot(crackleClip, sfxVolume);
            yield return new WaitForSeconds(0.1f);
        }

        // Ensure panel visible
        yield return FadeGroup(subtitleGroup, 0f, 1f, panelFadeSeconds);

        index = 0;
        while (index < (lines?.Length ?? 0))
        {
            var line = lines[index];

            // Set icon
            if (iconUI)
            {
                iconUI.sprite = line.characterIcon;
                iconUI.enabled = (iconUI.sprite != null);
            }

            // Play line sound
            var clip = line.voiceClip ? line.voiceClip : defaultBlip;
            if (audioSource && clip) audioSource.PlayOneShot(clip, sfxVolume);

            // Show text with fades + typewriter
            float pf = (line.panelFade > 0f) ? line.panelFade : panelFadeSeconds;
            float tf = (line.textFade > 0f) ? line.textFade : textFadeSeconds;
            float cps = (line.typewriterSpeed > 0f) ? line.typewriterSpeed : typewriterCps;

            yield return FadeTMP(subtitleText, 0f, 1f, tf);

            // Typewriter
            yield return TypeText(subtitleText, line.text, cps);

            // Wait either for input or hold
            if (line.waitForInput)
            {
                yield return WaitForAdvanceSignal();
            }
            else
            {
                yield return new WaitForSeconds(Mathf.Max(0f, line.autoHold));
            }

            // Fade text out between lines
            yield return FadeTMP(subtitleText, 1f, 0f, tf);
            subtitleText.text = "";
            index++;
        }

        // Clean up UI
        if (subtitleText) subtitleText.text = "";
        if (iconUI) iconUI.enabled = false;

        // Panel fade out
        yield return FadeGroup(subtitleGroup, 1f, 0f, panelFadeSeconds);

        running = null;
        OnConversationFinished?.Invoke();
    }

    // --- helpers ---
    private IEnumerator TypeText(TextMeshProUGUI tmp, string text, float cps)
    {
        if (!tmp) yield break;

        isTyping = true;
        fullCurrentText = text ?? "";
        tmp.text = "";

        if (cps <= 0f)
        {
            // instant
            tmp.text = fullCurrentText;
            isTyping = false;
            yield break;
        }

        float delay = 1f / cps; // seconds per char
        for (int i = 0; i < fullCurrentText.Length; i++)
        {
            if (!isTyping) // user skipped
            {
                tmp.text = fullCurrentText;
                break;
            }

            tmp.text += fullCurrentText[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    private bool _advanceSignal = false;
    private IEnumerator WaitForAdvanceSignal()
    {
        _advanceSignal = false;
        while (!_advanceSignal)
            yield return null;
        _advanceSignal = false;
    }

    private IEnumerator FadeGroup(CanvasGroup g, float a, float b, float t)
    {
        if (!g) yield break;
        float e = 0f;
        g.alpha = a;
        g.interactable = g.blocksRaycasts = (b > a);

        while (e < t)
        {
            e += Time.deltaTime;
            g.alpha = Mathf.Lerp(a, b, e / Mathf.Max(0.0001f, t));
            yield return null;
        }

        g.alpha = b;
        g.interactable = g.blocksRaycasts = (b > 0.001f);
    }

    private IEnumerator FadeTMP(TextMeshProUGUI tmp, float a, float b, float t)
    {
        if (!tmp) yield break;
        float e = 0f;
        var c = tmp.color;
        c.a = a; tmp.color = c;

        while (e < t)
        {
            e += Time.deltaTime;
            c.a = Mathf.Lerp(a, b, e / Mathf.Max(0.0001f, t));
            tmp.color = c;
            yield return null;
        }

        c.a = b; tmp.color = c;
    }

    // Add near your other public API
    public void PlayLines(DialogueLine[] set)
    {
        if (running != null) { StopCoroutine(running); running = null; }
        lines = set ?? System.Array.Empty<DialogueLine>();
        gameObject.SetActive(true);
        running = StartCoroutine(RunConversation());
    }

    // Convenience: single auto-advancing line (good for VO/system)
    public void Say(string text, Sprite icon = null, AudioClip clip = null, float hold = 1.5f)
    {
        PlayLines(new DialogueLine[] {
            new DialogueLine{
                characterIcon = icon,
                text = text,
                voiceClip = clip,
                waitForInput = false,
                autoHold = hold
            }
        });
    }

//npcs

public void SetLinesAndStart(DialogueLine[] newLines)
{
    lines = newLines;
    StartConversation();
}



    // ============ Editor QoL: quick sample starter ============
#if UNITY_EDITOR
    [ContextMenu("Sample: Start Now")]
    private void __StartNow() => StartConversation();
#endif
}

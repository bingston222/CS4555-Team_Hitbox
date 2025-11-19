using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[DisallowMultipleComponent]
public class Adialogue : MonoBehaviour
{
    public static Adialogue Instance;

    [Header("UI References")]
    [SerializeField] private CanvasGroup subtitleGroup;   // drag SubtitlePanel here
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Audio")]
    [SerializeField] private AudioSource radioSource;
    [SerializeField] private AudioClip crackleClip;
    [SerializeField] private AudioClip[] voiceLines;  // assign HQ voice clips here

    [Header("Behaviour")]
    [SerializeField] private bool autoStart = false;

    [Header("Events")]
    public UnityEvent OnIntroFinished; // Hook your "unlock input" or "start tutorial" here

    Coroutine running;

    private void Awake()
    {
        Instance = this;
        if (subtitleGroup) subtitleGroup.alpha = 0f;
        if (subtitleText) subtitleText.text = "";
    }

    private void OnEnable()
    {
        if (autoStart)
        {
            StartBootSectorIntro();
        }
    }

    private void OnDisable()
    {
        if (running != null) { StopCoroutine(running); running = null; }
        if (subtitleText) subtitleText.text = "";
        if (subtitleGroup) subtitleGroup.alpha = 0f;
    }

    /// <summary>Public entry to start the intro sequence.</summary>
    public void StartBootSectorIntro()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(RunBootSectorIntro());
    }

    private IEnumerator RunBootSectorIntro()
    {
        // Guard UI
        if (!subtitleGroup || !subtitleText)
            yield break;

        // Optional crackle
        if (radioSource && crackleClip)
        {
            radioSource.PlayOneShot(crackleClip);
            yield return new WaitForSeconds(0.1f); // quick lead-in
        }

        // Line 1
        if (radioSource && voiceLines != null && voiceLines.Length > 0 && voiceLines[0])
            radioSource.PlayOneShot(voiceLines[0]);

        yield return ShowSubtitle(
            "Receiving you loud and clear! Looks like you've landed inside the Hard Drive.",
            hold: 1.7f,
            fade: 0.18f
        );

        yield return new WaitForSeconds(0.05f);

        // Line 2
        if (radioSource && voiceLines != null && voiceLines.Length > 1 && voiceLines[1])
            radioSource.PlayOneShot(voiceLines[1]);

        yield return ShowSubtitle(
            "Before you go any deeper, let's run a quick system calibration.",
            hold: 1.6f,
            fade: 0.18f
        );

        // Clean up UI
        if (subtitleText) subtitleText.text = "";

        // Fire event instead of calling a component method
        OnIntroFinished?.Invoke();

        running = null;
    }

    /// <summary>Shows a subtitle with panel/text fade in/out.</summary>
    private IEnumerator ShowSubtitle(string text, float hold = 2.5f, float fade = 0.25f)
    {
        if (!subtitleText || !subtitleGroup) yield break;

        // Ensure panel visible
        yield return FadeGroup(subtitleGroup, 0f, 1f, fade);

        // Text fade in
        subtitleText.alpha = 0f;
        subtitleText.text = text;
        yield return FadeTMP(subtitleText, 0f, 1f, fade * 0.8f);

        // Hold
        yield return new WaitForSeconds(hold);

        // Text fade out
        yield return FadeTMP(subtitleText, 1f, 0f, fade * 0.8f);
        subtitleText.text = "";

        // Panel fade out
        yield return FadeGroup(subtitleGroup, 1f, 0f, fade);
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
            g.alpha = Mathf.Lerp(a, b, e / t);
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
        c.a = a;
        tmp.color = c;

        while (e < t)
        {
            e += Time.deltaTime;
            c.a = Mathf.Lerp(a, b, e / t);
            tmp.color = c;
            yield return null;
        }

        c.a = b;
        tmp.color = c;
    }
}

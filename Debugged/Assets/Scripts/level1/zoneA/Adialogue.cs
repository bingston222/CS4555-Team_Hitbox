using System.Collections;
using UnityEngine;
using TMPro;

public class Adialogue : MonoBehaviour
{
    public CanvasGroup subtitleGroup;   // drag SubtitlePanel here in Inspector

    public static Adialogue Instance;

    [Header("UI References")]
    public TextMeshProUGUI subtitleText;

    [Header("Audio")]
    public AudioSource radioSource;
    public AudioClip crackleClip;
    public AudioClip[] voiceLines;  // assign HQ voice clips here

    private void Awake()
    {
        Instance = this;
    }

    public void StartBootSectorIntro()
    {
        StartCoroutine(RunBootSectorIntro());
    }

    private IEnumerator RunBootSectorIntro()
{
    // Faster crackle: tiny delay before first line
    if (crackleClip)
    {
        radioSource.PlayOneShot(crackleClip);
        yield return new WaitForSeconds(0.1f);   // was crackleClip.length + 0.2f
    }

    // Line 1 – shorter fade and hold
    if (voiceLines.Length > 0) radioSource.PlayOneShot(voiceLines[0]);
    yield return StartCoroutine(ShowSubtitle(
        "Receiving you loud and clear! Looks like you've landed inside the Hard Drive.",
        hold: 1.7f,   // was ~2.8–3.0
        fade: 0.18f   // was 0.25
    ));

    // Micro gap between lines (or 0 if you want it immediate)
    yield return new WaitForSeconds(0.05f);

    // Line 2 – quick as well
    if (voiceLines.Length > 1) radioSource.PlayOneShot(voiceLines[1]);
    yield return StartCoroutine(ShowSubtitle(
        "Before you go any deeper, let's run a quick system calibration.",
        hold: 1.6f,
        fade: 0.18f
    ));

    // Clean up and hand off to tutorial (this unlocks input)
    subtitleText.text = "";
    IntroController intro = FindObjectOfType<IntroController>();
    if (intro != null) intro.EnableTutorialPhase();
}


    void Start()
    {
        if (subtitleGroup) subtitleGroup.alpha = 0f;
    }

    public IEnumerator ShowSubtitle(string text, float hold = 2.5f, float fade = 0.25f)
    {
        if (!subtitleText || !subtitleGroup) yield break;

        // fade panel in
        yield return FadeGroup(subtitleGroup, 0f, 1f, fade);

        // show text (quick fade optional)
        subtitleText.alpha = 0f;
        subtitleText.text = text;
        yield return FadeTMP(subtitleText, 0f, 1f, fade * 0.8f);

        // hold
        yield return new WaitForSeconds(hold);

        // fade text out, then panel out
        yield return FadeTMP(subtitleText, 1f, 0f, fade * 0.8f);
        subtitleText.text = "";
        yield return FadeGroup(subtitleGroup, 1f, 0f, fade);
    }

    IEnumerator FadeGroup(CanvasGroup g, float a, float b, float t)
    {
        float e = 0f; g.alpha = a; g.interactable = g.blocksRaycasts = (b > a);
        while (e < t) { e += Time.deltaTime; g.alpha = Mathf.Lerp(a, b, e/t); yield return null; }
        g.alpha = b; g.interactable = g.blocksRaycasts = (b > 0.001f);
    }

    IEnumerator FadeTMP(TMPro.TextMeshProUGUI tmp, float a, float b, float t)
    {
        float e = 0f; var c = tmp.color; c.a = a; tmp.color = c;
        while (e < t) { e += Time.deltaTime; c.a = Mathf.Lerp(a, b, e/t); tmp.color = c; yield return null; }
        c.a = b; tmp.color = c;
    }


}

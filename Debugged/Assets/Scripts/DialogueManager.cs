using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    // Singleton so other scripts can check visibility
    public static DialogueManager I { get; private set; }

    [Header("UI")]
    public TMP_Text bubble;          // assign your DialogueText TMP in the scene

    [Header("Durations")]
    public float seconds = 2.5f;     // normal lines
    public float shortSeconds = 1f;  // quick hints

    public bool IsVisible { get; private set; }

    Coroutine current;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        if (bubble) bubble.gameObject.SetActive(false);

        // (Optional) sample hooks like you had before
        if (GameState.I != null)
        {
            GameState.I.OnFound += who =>
            {
                if (!(GameState.I.P1HasController && GameState.I.P2HasController))
                    Show($"Player {who}: I found my controller!");
            };
            GameState.I.OnBothFound += last =>
            {
                Show($"Player {last}: I found mine too—let’s go connect it to the PC.");
            };
        }
    }

    // ---------- Public API ----------
    public void Show(string s)      => ShowFor(s, seconds);
    public void ShowShort(string s) => ShowFor(s, shortSeconds);

    public void ShowFor(string s, float dur)
    {
        // Kill any lingering interaction prompt so it never overlaps the dialogue
        InteractionPromptUI.ClearNow();

        if (!bubble) return;

        if (current != null) StopCoroutine(current);
        current = StartCoroutine(ShowCo(s, Mathf.Max(0.01f, dur)));
    }

    public void HideNow()
    {
        if (current != null) StopCoroutine(current);
        current = null;

        if (bubble) bubble.gameObject.SetActive(false);
        IsVisible = false;
    }

    // ---------- Impl ----------
    IEnumerator ShowCo(string s, float dur)
    {
        IsVisible = true;
        bubble.text = s;
        bubble.gameObject.SetActive(true);

        yield return new WaitForSeconds(dur);

        bubble.gameObject.SetActive(false);
        IsVisible = false;
        current = null;
    }
}

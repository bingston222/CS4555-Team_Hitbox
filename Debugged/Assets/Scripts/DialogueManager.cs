using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    // Singleton (so other scripts can check visibility)
    public static DialogueManager I { get; private set; }

    [Header("UI")]
    public TMP_Text bubble;              // assign your DialogueText TMP

    [Header("Durations")]
    public float seconds = 2.5f;         // normal lines
    public float shortSeconds = 1.0f;    // quick hints like "Oops, not here!"

    public bool IsVisible { get; private set; }  // <— other scripts read this

    Coroutine current;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        if (bubble) bubble.gameObject.SetActive(false);

        // Optional: sample hooks
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
        if (!bubble) return;
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(ShowCo(s, dur));
    }

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

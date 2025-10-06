using UnityEngine;
using TMPro;

/// Classic “Search (E)” prompt controller (singleton).
public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI I;

    [Header("UI")]
    public CanvasGroup group;          // assign your gray bar CanvasGroup
    public TMP_Text text;              // the TMP text that shows “Search (E)”
    [Header("Behavior")]
    public float holdAfterLost = 0.25f;

    float bestScoreThisFrame = float.PositiveInfinity;
    string bestTextThisFrame = null;
    float hideAt = -1f;

    void Awake()
    {
        I = this;
        if (group) group.alpha = 0f;
        if (text) text.text = string.Empty;
    }

    public static void Request(string t, float score)
    {
        if (!I) return;
        if (score < I.bestScoreThisFrame)
        {
            I.bestScoreThisFrame = score;
            I.bestTextThisFrame  = t;
        }
    }

    void LateUpdate()
    {
        // no requests this frame? start fade timer
        if (bestTextThisFrame == null)
        {
            if (hideAt < 0f) hideAt = Time.time + holdAfterLost;
        }
        else
        {
            if (text) text.text = bestTextThisFrame;
            if (group) group.alpha = 1f;
            hideAt = -1f;
        }

        if (hideAt > 0f && Time.time >= hideAt)
        {
            if (group) group.alpha = 0f;
            if (text) text.text = string.Empty;
            hideAt = -1f;
        }

        // reset arbitration
        bestScoreThisFrame = float.PositiveInfinity;
        bestTextThisFrame  = null;
    }

    public static void ClearNow()
    {
        if (!I) return;
        I.bestScoreThisFrame = float.PositiveInfinity;
        I.bestTextThisFrame  = null;
        if (I.text)  I.text.text = string.Empty;
        if (I.group) I.group.alpha = 0f;
        I.hideAt = -1f;
    }
}

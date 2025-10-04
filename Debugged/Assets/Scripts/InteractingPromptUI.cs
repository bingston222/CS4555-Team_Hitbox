using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI I;

    [Header("UI")]
    public CanvasGroup group;    // drag your InteractPrompt panel's CanvasGroup
    public TMP_Text text;        // drag the TMP text inside the prompt

    [Header("Behavior")]
    [SerializeField] float holdAfterLost = 0.25f;

    // arbitration fields (closest request wins each frame)
    static float bestScoreThisFrame = float.PositiveInfinity;
    static string bestTextThisFrame = null;

    float holdTimer = 0f;

    void Awake()
    {
        I = this;
        HideImmediate();
    }

    /// <summary>Call from Interactor each frame when it has a target.</summary>
    public static void Request(string prompt, float distanceScore)
    {
        if (string.IsNullOrEmpty(prompt)) return;
        if (distanceScore < bestScoreThisFrame)
        {
            bestScoreThisFrame = distanceScore;
            bestTextThisFrame = prompt;
        }
    }

    void LateUpdate()
    {
        bool dialogueUp = DialogueManager.I != null && DialogueManager.I.IsVisible;

        if (!dialogueUp && !string.IsNullOrEmpty(bestTextThisFrame))
        {
            text.text = bestTextThisFrame;
            group.alpha = 1f;
            holdTimer = holdAfterLost; // refresh hold
        }
        else
        {
            if (!dialogueUp && holdTimer > 0f)
            {
                holdTimer -= Time.deltaTime;
                group.alpha = 1f;
            }
            else
            {
                group.alpha = 0f; // hide if dialogue is up OR no target
            }
        }

        // reset arbitration for next frame
        bestScoreThisFrame = float.PositiveInfinity;
        bestTextThisFrame = null;
    }

    public void HideImmediate()
    {
        if (group) group.alpha = 0f;
    }
}

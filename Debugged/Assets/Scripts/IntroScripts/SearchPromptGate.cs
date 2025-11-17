using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(Collider))]
public class SearchPromptGate : MonoBehaviour
{
    [Header("Prompt UI")]
    [SerializeField] private GameObject promptRoot;        // whole "Search (E)" object
    [SerializeField] private TMP_Text promptLabel;         // optional label inside
    [SerializeField] private string activeText = "Search (E)";

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";

    private bool playerInside;

    // NOTE: must match GameState event signatures -> Action<int>
    private Action<int> onFoundHandler;
    private Action<int> onBothFoundHandler;

    void Awake()
    {
        if (promptRoot) promptRoot.SetActive(false);
    }

    void OnEnable()
    {
        // cache handlers so unsubscribe works
        onFoundHandler     = _ => RefreshPromptVisibility();
        onBothFoundHandler = _ => RefreshPromptVisibility();

        if (GameState.I != null)
        {
            GameState.I.OnFound     += onFoundHandler;
            GameState.I.OnBothFound += onBothFoundHandler;
        }

        RefreshPromptVisibility(); // handle scene reloads / initial state
    }

    void OnDisable()
    {
        if (GameState.I != null)
        {
            GameState.I.OnFound     -= onFoundHandler;
            GameState.I.OnBothFound -= onBothFoundHandler;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        RefreshPromptVisibility();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        if (promptRoot) promptRoot.SetActive(false);
    }

    private void RefreshPromptVisibility()
    {
        if (!promptRoot) return;

        bool bothFound   = GameState.I.P1HasController && GameState.I.P2HasController;
        bool allowPrompt = !bothFound;                 // 👉 only while still searching
        bool show        = playerInside && allowPrompt;

        if (promptLabel) promptLabel.text = show ? activeText : string.Empty;
        promptRoot.SetActive(show);
    }
}

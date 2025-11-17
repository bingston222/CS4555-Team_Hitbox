using UnityEngine;
using TMPro;
using System.Collections;

public class IntroDialogue : MonoBehaviour
{
    public TMP_Text DialogueText;
    public GameObject DialoguePanel;

    int index = 0;

    string[] lines = new string[]
    {
        "Player 1: Uhhh, that's strange… it's never made that sound before.",
        "Player 2: Did your PC just break?",
        "Player 1: I think it might… maybe I just need to get rid of the whole thing.",
        "Player 2: I guess we can just work on our homework.",
        "Player 1: Ugh I guess you're right… I'll shut my PC down, and if it's still not working by tomorrow, I’ll get rid of it."
    };

    bool waitingForNext = false;

    void Start()
    {
        DialoguePanel.SetActive(true);
        StartCoroutine(ShowLine());
    }

    void Update()
    {
        if (waitingForNext && Input.anyKeyDown)
        {
            waitingForNext = false;
            index++;

            if (index < lines.Length)
                StartCoroutine(ShowLine());
            else
                DialoguePanel.SetActive(false); // end cutscene
        }
    }

    IEnumerator ShowLine()
    {
        DialogueText.text = "";
        string fullLine = lines[index];

        // typewriter effect
        foreach (char c in fullLine)
        {
            DialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        waitingForNext = true;
    }
}

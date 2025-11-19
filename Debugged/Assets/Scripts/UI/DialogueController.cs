using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public Sprite characterIcon;
        [TextArea] public string text;
    }

    public DialogueLine[] lines;

    public Image iconUI;
    public TMP_Text dialogueUI;

    int index = 0;
    bool isTyping = false;

    void Start()
    {
        ShowLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        if (index >= lines.Length)
        {
            StartCoroutine(DoGlitchThenLoadSelect());
            return;
        }

        iconUI.sprite = lines[index].characterIcon;

        StopAllCoroutines();
        StartCoroutine(TypeText(lines[index].text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueUI.text = "";

        foreach (char c in text)
        {
            dialogueUI.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    void NextLine()
    {
        index++;
        ShowLine();
    }

    IEnumerator DoGlitchThenLoadSelect()
    {
        Debug.Log("Glitch transition starting...");
        yield return new WaitForSeconds(1.5f);
        //SceneManager.LoadScene("Level1");
    }
}

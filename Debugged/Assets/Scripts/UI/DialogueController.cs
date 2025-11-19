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

        // NEW ➜ Optional unique sound for this specific dialogue line
        public AudioClip customSound;
    }

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    [Header("UI References")]
    public Image iconUI;
    public TMP_Text dialogueUI;

    [Header("Sound Settings")]
    public AudioSource audioSource;     // Audio source component on the DialogueManager
    public AudioClip blipSound;         // Default sound for most dialogue lines
    public float volume = 1f;

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

        // Update icon
        iconUI.sprite = lines[index].characterIcon;

        // 🔊 Play sound for this line
        PlaySoundForLine(lines[index]);

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

    // 🔊 NEW FUNCTION — decides which sound to play
    void PlaySoundForLine(DialogueLine line)
    {
        if (line.customSound != null)
        {
            audioSource.PlayOneShot(line.customSound, volume);
        }
        else
        {
            audioSource.PlayOneShot(blipSound, volume);
        }
    }

    IEnumerator DoGlitchThenLoadSelect()
    {
        Debug.Log("Glitch transition starting...");
        yield return new WaitForSeconds(1.5f);
        //SceneManager.LoadScene("Level1");
    }
}

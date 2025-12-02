using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public Sprite characterIcon;
        [TextArea] public string text;
        public AudioClip customSound;  // Optional per-line sound
    }

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    [Header("UI References")]
    public Image iconUI;
    public TMP_Text dialogueUI;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip blipSound;
    public float volume = 1f;

    [Header("Scene Settings")]
    public string nextSceneName;   // <-- Add this in inspector

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
            LoadNextScene();
            return;
        }

        iconUI.sprite = lines[index].characterIcon;

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

    void PlaySoundForLine(DialogueLine line)
    {
        if (line.customSound != null)
            audioSource.PlayOneShot(line.customSound, volume);
        else
            audioSource.PlayOneShot(blipSound, volume);
    }

    void LoadNextScene()
    {
        Debug.Log("Loading next scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
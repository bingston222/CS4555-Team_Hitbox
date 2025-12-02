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
        public AudioClip customSound;
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
    public string nextSceneName;

    // OPTIONAL — only enabled in scenes that need freezing and no scene transition
    [Header("Optional Behavior")]
    public bool stayInScene = false;
    public bool freezePlayers = false;

    int index = 0;
    bool isTyping = false;

    void Start()
    {
        if (freezePlayers)
            BlockAllPlayerInput();

        ShowLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
            NextLine();
    }

    void ShowLine()
    {
        if (index >= lines.Length)
        {
            FinishDialogue();
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

    void FinishDialogue()
    {
        if (freezePlayers)
            UnblockAllPlayerInput();

        if (stayInScene)
        {
            // Hide the entire dialogue UI panel
            dialogueUI.transform.parent.gameObject.SetActive(false);
            Debug.Log("Dialogue finished — staying in scene.");
            return;
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("Loading next scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty — no scene to load.");
        }
    }

    // ----------------------------------------------------------
    // Freezing / Unfreezing Player Input
    // ----------------------------------------------------------

    void BlockAllPlayerInput()
    {
        foreach (var blocker in FindObjectsOfType<PlayerInputBlocker>())
            blocker.inputBlocked = true;
    }

    void UnblockAllPlayerInput()
    {
        foreach (var blocker in FindObjectsOfType<PlayerInputBlocker>())
            blocker.inputBlocked = false;
    }
}

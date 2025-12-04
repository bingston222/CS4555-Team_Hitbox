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

    [Header("Optional Behavior")]
    public bool stayInScene = true;    
    public bool freezePlayers = true;  

    [Header("UI To Hide During Dialogue")]
    public GameObject[] uiToHide;       

    int index = 0;
    bool isTyping = false;

    void Start()
    {
        // Prevent audio muting during Pause states
        if (audioSource != null)
        {
            audioSource.ignoreListenerPause = true;
            audioSource.ignoreListenerVolume = true;
        }

        // Freeze players if needed
        if (freezePlayers)
            BlockAllPlayerInput();

        // Hide HUD
        foreach (var ui in uiToHide)
            if (ui != null)
                ui.SetActive(false);

        ShowLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
            NextLine();
    }

    // ------------------------ DIALOGUE HANDLING ------------------------

    void ShowLine()
    {
        if (index >= lines.Length)
        {
            FinishDialogue();
            return;
        }

        // Set portrait
        if (iconUI != null)
            iconUI.sprite = lines[index].characterIcon;

        // Play SFX
        PlaySoundForLine(lines[index]);

        StopAllCoroutines();
        StartCoroutine(TypeText(lines[index].text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueUI.text = "";

        if (string.IsNullOrEmpty(text))
            text = " "; // prevents instant-skip

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
        if (audioSource == null) return;

        if (line.customSound != null)
            audioSource.PlayOneShot(line.customSound, volume);
        else if (blipSound != null)
            audioSource.PlayOneShot(blipSound, volume);
    }

    // ------------------------ END DIALOGUE ------------------------

    void FinishDialogue()
    {
        // Restore movement
        if (freezePlayers)
            UnblockAllPlayerInput();

        // Restore HUD
        foreach (var ui in uiToHide)
            if (ui != null)
                ui.SetActive(true);

        // 🔥 Hide the ENTIRE DialogueCanvas object
        gameObject.SetActive(false);

        // Load next scene if assigned
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // ------------------------ PLAYER INPUT BLOCKING ------------------------

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


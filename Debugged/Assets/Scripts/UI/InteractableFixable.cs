using UnityEngine;

public class InteractableFixable : MonoBehaviour
{
    [Header("Fix State")]
    public bool isFixed = false;
    public int requiredChecks = 3;
    public float failCooldown = 20f;

    float cooldownTimer = 0f;
    public bool IsOnCooldown => cooldownTimer > 0;

    [Header("Visuals")]
    // Drag the Fusebox_Glow child object here
    public GameObject glowObject;

    [Header("Audio")]
    // Add an AudioSource on the fusebox (or child) and drag it here
    public AudioSource audioSource;
    // Drag your success sound clip here
    public AudioClip successClip;

    void Start()
    {
        // Show glow at start if not already fixed
        if (glowObject != null)
        {
            glowObject.SetActive(!isFixed);
        }
    }

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    // Call this once the player has successfully completed all required checks
    public void OnCompleteSuccess()
    {
        if (isFixed) return;

        isFixed = true;
        Debug.Log("✔ Object fully repaired!");

        // Turn off the glow outline
        if (glowObject != null)
        {
            glowObject.SetActive(false);
        }

        // Play success sound
        if (audioSource != null && successClip != null)
        {
            audioSource.PlayOneShot(successClip);
        }
    }

    public void OnFail()
    {
        cooldownTimer = failCooldown;
        Debug.Log("❌ Failed skill check. Cooldown started.");
    }
}

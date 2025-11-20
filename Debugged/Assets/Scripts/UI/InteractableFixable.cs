using UnityEngine;

public class InteractableFixable : MonoBehaviour
{
    public bool isFixed = false;
    public int requiredChecks = 3;
    public float failCooldown = 20f;

    float cooldownTimer = 0f;

    public bool IsOnCooldown => cooldownTimer > 0;

    void Update()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    public void OnCompleteSuccess()
    {
        isFixed = true;
        Debug.Log("✔ Object fully repaired!");
    }

    public void OnFail()
    {
        cooldownTimer = failCooldown;
        Debug.Log("❌ Failed skill check. Cooldown started.");
    }
}

using UnityEngine;

public class FreezeAbility : MonoBehaviour
{
    public KeyCode key = KeyCode.LeftShift;
    public float duration = 7f;
    public float cooldown = 45f;

    public GameObject freezeSphere;

    private PlayerStatus status;
    private bool onCooldown = false;

    void Start()
    {
        status = GetComponent<PlayerStatus>();
        freezeSphere.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && !onCooldown)
        {
            ActivateFreeze();
        }
    }

    void ActivateFreeze()
    {
        onCooldown = true;

        // Activate shield sphere
        freezeSphere.SetActive(true);

        // Make player invulnerable
        status.SetInvulnerable(true);

        // Disable abilities for duration
        status.DisableAbilities(duration);

        // End freeze visuals and invulnerability
        Invoke(nameof(EndFreeze), duration);

        // End cooldown timer
        Invoke(nameof(ResetCooldown), cooldown);
    }

    void EndFreeze()
    {
        freezeSphere.SetActive(false);
        status.SetInvulnerable(false);
        // DO NOT call DisableAbilities(false) — coroutine resets automatically
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }
}

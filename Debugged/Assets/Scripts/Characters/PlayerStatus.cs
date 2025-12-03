using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    [Header("Respawn Settings")]
    public Transform spawnPoint;
    public float respawnDelay = 1.2f;

    // =======================
    // Movement & Status Flags
    // =======================
    [Header("Movement Modifiers")]
    public bool ReverseControls = false;     // used by RedirectProtocol
    public float moveMultiplier = 1f;        // used by speed effects

    // =======================
    // Ability Flags / Toggles
    // =======================
    [HideInInspector] public bool guaranteedHit = false;
    [HideInInspector] public bool turnEnemiesFriendly = false;
    [HideInInspector] public bool abilitiesDisabled = false;
    private bool isRespawning = false;

    void Start()
    {
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find(gameObject.name + "_Spawn");
            if (sp != null) spawnPoint = sp.transform;
            else spawnPoint = transform;
        }

        currentHP = maxHP;
    }

    // ============================================
    // DAMAGE + DEATH (PLAYERS NEVER DESPAWN)
    // ============================================
    public void TakeDamage(float dmg)
    {
        if (isRespawning) return;

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            StartCoroutine(PlayerRespawn());
        }
    }

    private IEnumerator PlayerRespawn()
    {
        isRespawning = true;

        DisableAbilities(0.5f);

        yield return new WaitForSeconds(respawnDelay);

        // reset everything
        currentHP = maxHP;
        ReverseControls = false;
        moveMultiplier = 1f;

        // teleport back to spawn
        transform.position = spawnPoint.position;

        isRespawning = false;
    }

    // ============================================
    // RESET EVERYTHING TO FULL (Patchnotes heal)
    // ============================================
    public void ResetFull()
    {
        currentHP = maxHP;
        ReverseControls = false;
        moveMultiplier = 1f;
    }

    // ============================================
    // TEMPORARY REVERSE CONTROLS (RedirectProtocol)
    // ============================================
    public IEnumerator TempReverseControls(float duration)
    {
        ReverseControls = true;
        yield return new WaitForSeconds(duration);
        ReverseControls = false;
    }

    // ============================================
    // ABILITY DISABLING SYSTEM
    // ============================================
    public void DisableAbilities(float duration)
    {
        StartCoroutine(DisableAbilitiesRoutine(duration));
    }

    private IEnumerator DisableAbilitiesRoutine(float duration)
    {
        abilitiesDisabled = true;
        yield return new WaitForSeconds(duration);
        abilitiesDisabled = false;
    }
}

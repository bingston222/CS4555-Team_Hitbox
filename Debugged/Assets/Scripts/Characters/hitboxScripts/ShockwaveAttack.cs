using System.Collections;
using UnityEngine;

public class ShockwaveAttack : MonoBehaviour
{
    [Header("Input & Timing")]
    public KeyCode key = KeyCode.Mouse0;
    public float cooldown = 0.45f;

    [Header("Animation")]
    public Animator animator;            
    public string attackTrigger = "Attack";
    public bool useAnimationEvent = false; // if true, call SpawnProjectile() from the anim

    [Header("Projectile")]
    public ShockwaveProjectile projectilePrefab; // your prefab type
    public Transform spawnPoint;

    [Header("UI")]
    public AbilityUI abilityUI; // drag Player1_Ability (with AbilityUI) here

    bool ready = true;

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(FireRoutine());
    }

    IEnumerator FireRoutine()
    {
        ready = false;

        // start cooldown timer for UI
        float cdLeft = cooldown;
        abilityUI?.UpdateBaseCooldown(1f); // 1 = just started cooling

        // animation & spawn
        if (animator) animator.SetTrigger(attackTrigger);
        if (!useAnimationEvent)
        {
            yield return new WaitForSeconds(0.08f);
            SpawnProjectile();
        }

        // tick cooldown → 1..0
        while (cdLeft > 0f)
        {
            cdLeft -= Time.deltaTime;
            float percent = Mathf.Clamp01(cdLeft / cooldown);
            abilityUI?.UpdateBaseCooldown(percent);
            yield return null;
        }

        abilityUI?.UpdateBaseCooldown(0f); // ready
        ready = true;
    }

    // call via animation event if useAnimationEvent = true
    public void SpawnProjectile()
    {
        if (!projectilePrefab) return;
        Transform p = spawnPoint ? spawnPoint : transform;
        Instantiate(projectilePrefab, p.position, p.rotation);
    }
}

using System.Collections;
using UnityEngine;

public class FreezeAbility : MonoBehaviour
{
    [Header("Input & Timing")]
    public KeyCode key = KeyCode.Q;
    [Tooltip("How long you stay invulnerable")]
    public float duration = 7f;
    [Tooltip("How long before you can cast again")]
    public float cooldown = 120f;

    [Header("Behavior")]
    public bool ignoreEnemyCollisionsWhileFrozen = false;

    [Header("Animation")]
    public Animator animator;
    public string freezeTrigger = "Freeze";

    [Header("VFX / SFX")]
    public ParticleSystem castVFX;
    public float castVFXLifetime = 2f;
    public ParticleSystem loopVFX;
    public float vfxCleanupDelay = 0.25f;
    public AudioSource audioSource;
    public AudioClip castClip;
    public AudioClip endClip;

    [Header("UI")]
    public AbilityUI ui; // drag Player1_Ability (with AbilityUI) here

    bool ready = true;
    int playerLayer, enemyLayer;
    ParticleSystem loopInstance;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer  = LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(DoFreeze());
    }

    IEnumerator DoFreeze()
{
    ready = false;
    ui?.UpdateAbilityCooldown(1f);

    if (animator && !string.IsNullOrEmpty(freezeTrigger))
        animator.SetTrigger(freezeTrigger);

    if (castVFX)
    {
        var fx = Instantiate(castVFX, transform.position, transform.rotation, transform);
        Destroy(fx.gameObject, castVFXLifetime);
    }
    if (loopVFX) loopInstance = Instantiate(loopVFX, transform.position, transform.rotation, transform);
    if (audioSource && castClip) audioSource.PlayOneShot(castClip);

    // ✅ Turn on true invulnerability
    var hp = GetComponent<Health>();
    if (hp) hp.SetInvulnerable(true);

    // Optional: also ignore collisions so projectiles/triggers don't fire
    if (ignoreEnemyCollisionsWhileFrozen && playerLayer >= 0 && enemyLayer >= 0)
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);

    // freeze duration
    float d = duration;
    while (d > 0f) { d -= Time.deltaTime; yield return null; }

    // end VFX/SFX
    if (loopInstance)
    {
        loopInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(loopInstance.gameObject, vfxCleanupDelay);
        loopInstance = null;
    }
    if (audioSource && endClip) audioSource.PlayOneShot(endClip);

    // ✅ Restore invulnerability & collisions
    if (hp) hp.SetInvulnerable(false);
    if (ignoreEnemyCollisionsWhileFrozen && playerLayer >= 0 && enemyLayer >= 0)
        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);

    // cooldown ticks → 1..0
    float cd = cooldown;
    while (cd > 0f)
    {
        cd -= Time.deltaTime;
        ui?.UpdateAbilityCooldown(Mathf.Clamp01(cd / cooldown));
        yield return null;
    }

    ui?.UpdateAbilityCooldown(0f);
    ready = true;
}

// Also restore if disabled/destroyed mid-freeze
void OnDisable()
{
    int player = LayerMask.NameToLayer("Player");
    int enemy  = LayerMask.NameToLayer("Enemy");
    if (player >= 0 && enemy >= 0)
        Physics.IgnoreLayerCollision(player, enemy, false);

    var hp = GetComponent<Health>();
    if (hp) hp.SetInvulnerable(false);
}


}

using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class InstantHitboxUltimate : MonoBehaviour
{
    [Header("Input & Timing")]
    public KeyCode key = KeyCode.LeftShift;
    public float duration = 8f;

    [Header("Animation")]
    public Animator animator;
    public string triggerName = "Ultimate";

    [Header("VFX / SFX")]
    public ParticleSystem castVFX;
    public ParticleSystem loopVFX;
    public float vfxCleanupDelay = 0.25f;
    public AudioSource audioSource;
    public AudioClip castClip;
    public AudioClip endClip;

    [Header("Gameplay")]
    public bool enlargeEnemyHitboxes = true;
    [Min(1f)] public float enemyHitboxScale = 2.0f;

    bool usingUlt = false;
    ParticleSystem loopInstance;
    UltimateCharge charge;  // cached

    void Awake()
    {
        // Find animator anywhere on this object or its children
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        // Charge can be on this object or any parent
        charge = GetComponentInParent<UltimateCharge>();

        EnsurePlayerEnemyCollisionsOn();
    }

    void OnEnable()  => EnsurePlayerEnemyCollisionsOn();

    void OnDisable()
    {
        EnemyHitboxExpander.RestoreAll();
        EnsurePlayerEnemyCollisionsOn();
    }

    void OnDestroy()
    {
        EnemyHitboxExpander.RestoreAll();
        EnsurePlayerEnemyCollisionsOn();
    }

    void Update()
    {
        if (!charge)
        {
            // Try to (re)acquire if hierarchy changed
            charge = GetComponentInParent<UltimateCharge>();
            if (!charge) return;
        }

        if (Input.GetKeyDown(key) && charge.IsFull && !usingUlt)
            StartCoroutine(DoUltimate());
    }

    IEnumerator DoUltimate()
    {
        usingUlt = true;

        if (animator && !string.IsNullOrEmpty(triggerName))
            animator.SetTrigger(triggerName);

        if (castVFX) Instantiate(castVFX, transform.position, transform.rotation, transform);
        if (loopVFX) loopInstance = Instantiate(loopVFX, transform.position, transform.rotation, transform);
        if (audioSource && castClip) audioSource.PlayOneShot(castClip);

        var status = GetComponentInParent<PlayerStatus>();
        if (status) status.guaranteedHit = true;

        if (enlargeEnemyHitboxes)
            EnemyHitboxExpander.ApplyAll(enemyHitboxScale);

        yield return new WaitForSeconds(duration);

        if (loopInstance)
        {
            loopInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(loopInstance.gameObject, vfxCleanupDelay);
            loopInstance = null;
        }
        if (audioSource && endClip) audioSource.PlayOneShot(endClip);

        EnemyHitboxExpander.RestoreAll();
        if (status) status.guaranteedHit = false;

        charge.ResetCharge();
        usingUlt = false;

        EnsurePlayerEnemyCollisionsOn();
    }

    void EnsurePlayerEnemyCollisionsOn()
    {
        int player = LayerMask.NameToLayer("Player");
        int enemy  = LayerMask.NameToLayer("Enemy");
        if (player >= 0 && enemy >= 0)
            Physics.IgnoreLayerCollision(player, enemy, false);
    }
}

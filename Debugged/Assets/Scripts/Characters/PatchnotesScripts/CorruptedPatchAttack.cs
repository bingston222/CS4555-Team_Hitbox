using System.Collections;
using UnityEngine;

public class CorruptedPatchAttack : MonoBehaviour
{
    [Header("Input & Timing")]
    public KeyCode key = KeyCode.O;
    public float cooldown = 0.3f;

    [Header("Animation")]
    public Animator animator;
    public string attackTrigger = "Attack";
    public bool useAnimationEvent = false; // if true, call FireProjectile() from animation event

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Audio & VFX")]
    public AudioClip attackSfx;
    public ParticleSystem attackVfxPrefab;

    [Header("UI (Optional)")]
    public AbilityUI abilityUI; // drag your ability UI (if you use one)

    private bool ready = true;
    private AudioSource audioSrc;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        audioSrc = GetComponentInChildren<AudioSource>();
        if (!audioSrc)
        {
            audioSrc = gameObject.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;
        }
    }

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        ready = false;
        abilityUI?.UpdateBaseCooldown(1f); // start cooldown

        if (animator)
        {
            animator.ResetTrigger(attackTrigger);
            animator.SetTrigger(attackTrigger);
        }

        if (!useAnimationEvent)
        {
            // Wait slightly for the animation wind-up before spawning
            yield return new WaitForSeconds(0.08f);
            FireProjectile();
        }

        // cooldown handling
        float cd = cooldown;
        while (cd > 0f)
        {
            cd -= Time.deltaTime;
            abilityUI?.UpdateBaseCooldown(cd / cooldown);
            yield return null;
        }

        abilityUI?.UpdateBaseCooldown(0f);
        ready = true;
    }

    // This can be called directly or by Animation Event
    public void FireProjectile()
    {
        if (!firePoint) firePoint = transform;

        // Spawn projectile
        if (projectilePrefab)
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Play VFX
        if (attackVfxPrefab)
        {
            var fx = Instantiate(attackVfxPrefab, firePoint.position, firePoint.rotation);
            fx.Play();
            Destroy(fx.gameObject, fx.main.duration);
        }

        // Play SFX
        if (attackSfx)
            audioSrc.PlayOneShot(attackSfx);
    }
}
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthBarAutoBinder : MonoBehaviour
{
    [Header("How to find the UI (choose ONE)")]
    [Tooltip("If set, this exact HealthBarUI will be used and other options are ignored.")]
    public HealthBarUI directReference;

    [Tooltip("Optional tag of the bar to bind to, e.g. P1_HealthBar, P2_HealthBar.")]
    public string healthBarTag;

    [Tooltip("If > 0, will look for a tag named P{PlayerIndex}_HealthBar. Ignored if Direct Reference is set.")]
    public int playerIndex = 0;

    [Header("Debug")]
    public bool logBinding = false;

    private HealthBarUI ui;
    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        ui = ResolveHealthBar();

        if (!ui)
        {
            Debug.LogWarning(
                $"[HealthBarAutoBinder:{name}] Could not resolve a HealthBarUI. " +
                "Assign 'directReference', or set 'healthBarTag' / 'playerIndex'."
            );
        }
    }

    void OnEnable()
    {
        if (health) health.onHealthChanged.AddListener(OnHealthChanged);
    }

    void OnDisable()
    {
        if (health) health.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    void Start()
    {
        if (ui && health) ui.UpdateBar(health.maxHP, health.maxHP);
        if (logBinding)
        {
            string uiName = ui ? ui.name : "NULL";
            Debug.Log($"[HealthBarAutoBinder:{name}] Bound to UI: {uiName}");
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (ui) ui.UpdateBar(current, max);
    }

    // --------- Helpers ---------

    private HealthBarUI ResolveHealthBar()
    {
        // 1) Direct reference wins
        if (directReference) return directReference;

        // 2) Tag (explicit)
        if (!string.IsNullOrEmpty(healthBarTag))
        {
            GameObject go = GameObject.FindGameObjectWithTag(healthBarTag);
            if (go) return go.GetComponent<HealthBarUI>();
        }

        // 3) Player index → P{index}_HealthBar
        if (playerIndex > 0)
        {
            string autoTag = $"P{playerIndex}_HealthBar";
            GameObject go = SafeFindWithTag(autoTag);
            if (go) return go.GetComponent<HealthBarUI>();
        }

        // 4) Fallback: first HealthBarUI in scene
        return FindObjectOfType<HealthBarUI>();
    }

    // Safer tag finder (prevents exception if tag doesn't exist)
    private GameObject SafeFindWithTag(string tag)
    {
        try
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
        catch
        {
            if (logBinding)
                Debug.LogWarning($"[HealthBarAutoBinder:{name}] Tag '{tag}' does not exist in this project.");
            return null;
        }
    }

#if UNITY_EDITOR
    // Editor convenience: warn if you configured overlapping methods
    void OnValidate()
    {
        if (directReference && (!string.IsNullOrEmpty(healthBarTag) || playerIndex > 0))
        {
            Debug.LogWarning($"[HealthBarAutoBinder:{name}] 'directReference' is set; tag/index settings will be ignored.");
        }
    }
#endif

    // ---- Optional debug shortcut (remove if undesired) ----
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            health?.TakeDamage(10f);
            if (logBinding) Debug.Log("[HealthBarAutoBinder] test damage -10");
        }
    }
}

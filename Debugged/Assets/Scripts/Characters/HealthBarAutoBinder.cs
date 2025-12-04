using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthBarAutoBinder : MonoBehaviour
{
    [Header("How to find the UI (choose ONE)")]
    public HealthBarUI directReference;
    public string healthBarTag;
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
            Debug.LogWarning($"[HealthBarAutoBinder:{name}] Could not resolve a HealthBarUI.");
        }
    }

    void OnEnable()
    {
        if (health)
            health.onHealthChanged += OnHealthChanged;   // FIXED
    }

    void OnDisable()
    {
        if (health)
            health.onHealthChanged -= OnHealthChanged;   // FIXED
    }

    void Start()
    {
        if (ui && health)
            ui.UpdateBar(health.maxHP, health.maxHP);
    }

    private void OnHealthChanged(float current, float max)
    {
        if (ui)
            ui.UpdateBar(current, max);
    }

    private HealthBarUI ResolveHealthBar()
    {
        if (directReference) return directReference;

        if (!string.IsNullOrEmpty(healthBarTag))
        {
            GameObject go = GameObject.FindGameObjectWithTag(healthBarTag);
            if (go) return go.GetComponent<HealthBarUI>();
        }

        if (playerIndex > 0)
        {
            string autoTag = $"P{playerIndex}_HealthBar";
            GameObject go = SafeFindWithTag(autoTag);
            if (go) return go.GetComponent<HealthBarUI>();
        }

        return FindObjectOfType<HealthBarUI>();
    }

    private GameObject SafeFindWithTag(string tag)
    {
        try { return GameObject.FindGameObjectWithTag(tag); }
        catch { return null; }
    }
}

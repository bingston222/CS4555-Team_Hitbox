using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHealthNumber : MonoBehaviour
{
    public FloatingText floatingTextPrefab;
    public bool showDamageNumbers = true;
    public bool showCurrentHP = false;
    public bool showOnHeal = false;

    public Transform spawnPoint;
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public Color damageColor = new Color(1f, 0.35f, 0.35f);
    public Color healColor = new Color(0.35f, 1f, 0.35f);
    public Color hpColor = Color.white;

    Health health;
    float lastHP;

    void Start()
    {
        health = GetComponent<Health>();
        lastHP = health.CurrentHP;

        health.onHealthChanged += OnHealthChanged;   // FIXED
        health.onDeath += () => { };
    }

    void OnHealthChanged(float current, float max)
    {
        Vector3 pos = (spawnPoint ? spawnPoint.position : transform.position) + offset;

        float delta = current - lastHP;
        lastHP = current;

        if (delta < -Mathf.Epsilon && showDamageNumbers && floatingTextPrefab != null)
        {
            var ft = Instantiate(floatingTextPrefab, pos, Quaternion.identity);
            ft.Set($"-{Mathf.RoundToInt(Mathf.Abs(delta))}", damageColor);
        }
    }
}

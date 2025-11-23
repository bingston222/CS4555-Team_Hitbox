using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyHealthNumber : MonoBehaviour
{
    [Header("Prefab")]
    public FloatingText floatingTextPrefab;

    [Header("What to show")]
    public bool showDamageNumbers = true;  // "-12"
    public bool showCurrentHP = false;     // "88/100" or "88"
    public bool showOnHeal = false;

    [Header("Look/Placement")]
    public Transform spawnPoint;           // optional (e.g., head)
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public Color damageColor = new Color(1f, 0.35f, 0.35f);
    public Color healColor   = new Color(0.35f, 1f, 0.35f);
    public Color hpColor     = Color.white;

    Health health;
    float lastHP;

    void Start()
    {
        health = GetComponent<Health>();
        lastHP = health.CurrentHP;

        // subscribe
        health.onHealthChanged.AddListener(OnHealthChanged);
        health.onDeath += () => { /* nothing needed, numbers auto-destroy */ };
    }

    void OnHealthChanged(float current, float max)
{
    Debug.Log($"[{name}] HP changed: {current}/{max}");

    // TEMP: spawn a tiny cube where we expect the number to be
    Vector3 pos = (spawnPoint ? spawnPoint.position : transform.position) + offset;
    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    cube.transform.position = pos;
    cube.transform.localScale = Vector3.one * 0.15f;
    Destroy(cube, 0.5f);

    // --- existing code below ---
    float delta = current - lastHP;
    lastHP = current;

    if (delta < -Mathf.Epsilon && showDamageNumbers && floatingTextPrefab != null)
    {
        var ft = Instantiate(floatingTextPrefab, pos, Quaternion.identity);
        ft.Set($"-{Mathf.RoundToInt(Mathf.Abs(delta))}", damageColor);
    }
    // (heal + current HP logic if you kept it)
}

}
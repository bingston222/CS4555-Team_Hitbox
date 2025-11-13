using System.Collections;
using UnityEngine;

public class TestOrb : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 3;
    int hp;

    [Header("Visuals")]
    public Renderer orbRenderer;          // drag MeshRenderer here
    public Color baseEmissive = new Color(0.3f, 1f, 1f);
    public Color hitEmissive = new Color(1f, 0.6f, 0.2f);
    public float hitFlashTime = 0.12f;

    [Header("FX")]
    public GameObject hitSparkPrefab;     // optional
    public GameObject destroyBurstPrefab; // optional
    public AudioSource pingSfx;           // optional
    public AudioSource destroySfx;        // optional

    [Header("Objective")]
    public string objectiveId = "ZoneA_Orbs";  // tie both orbs to same id

    Material _mat;
    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    bool _dead;

    void Awake() {
        hp = maxHP;
        if (orbRenderer == null) orbRenderer = GetComponentInChildren<Renderer>();
        if (orbRenderer != null) {
            _mat = orbRenderer.material;
            if (_mat.HasProperty(EmissionColor)) _mat.SetColor(EmissionColor, baseEmissive);
        }
    }

    public void TakeHit(int dmg = 1, Vector3? hitPos = null)
    {
        if (_dead) return;

        hp -= dmg;
        if (pingSfx) pingSfx.Play();
        if (hitSparkPrefab) Instantiate(hitSparkPrefab, hitPos ?? transform.position, Quaternion.identity);

        if (_mat && _mat.HasProperty(EmissionColor)) StartCoroutine(HitFlash());

        if (hp <= 0) StartCoroutine(Die());
    }

    IEnumerator HitFlash()
    {
        _mat.SetColor(EmissionColor, hitEmissive);
        yield return new WaitForSeconds(hitFlashTime);
        _mat.SetColor(EmissionColor, baseEmissive);
    }

    IEnumerator Die()
    {
        _dead = true;
        // disable hits
        var col = GetComponent<Collider>(); if (col) col.enabled = false;

        if (destroySfx) destroySfx.Play();
        if (destroyBurstPrefab) Instantiate(destroyBurstPrefab, transform.position, Quaternion.identity);

        // simple dissolve/scale down
        float t = 0f;
        Vector3 start = transform.localScale;
        while (t < 0.25f) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(start, Vector3.zero, t/0.25f); yield return null; }

        // tell objective manager
        CombatTestObjective.OrbDestroyed(objectiveId);

        Destroy(gameObject);
    }

    // Simple trigger-based damage: anything tagged "PlayerAttack" hurts it.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            Vector3 p = other.ClosestPoint(transform.position);
            TakeHit(1, p);
        }
    }
}

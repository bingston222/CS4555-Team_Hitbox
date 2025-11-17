using System.Collections;
using UnityEngine;

public class HealingPatchAbility : MonoBehaviour
{
    public KeyCode key = KeyCode.Q;
    public float cooldown = 45f;
    public GameObject healingProjectilePrefab;
    public Transform firePoint;

    bool ready = true;
    AbilityUI ui;

    void Start()
    {
        ui = GetComponent<AbilityUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && ready)
            StartCoroutine(UseHealingPatch());
    }

    IEnumerator UseHealingPatch()
    {
        ready = false;
        ui.UpdateAbilityCooldown(1f);

        Instantiate(healingProjectilePrefab, firePoint.position, firePoint.rotation);

        float t = cooldown;
        while (t > 0)
        {
            t -= Time.deltaTime;
            ui.UpdateAbilityCooldown(t / cooldown);
            yield return null;
        }

        ui.UpdateAbilityCooldown(0f);
        ready = true;
    }
}

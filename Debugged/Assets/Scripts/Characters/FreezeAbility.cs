using System.Collections;
using UnityEngine;

public class FreezeAbility : MonoBehaviour
{
    public KeyCode key = KeyCode.E;
    public float duration = 7f;
    public float cooldown = 45f;

    bool ready = true;
    AbilityUI ui;

    void Start()
    {
        ui = GetComponent<AbilityUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && ready)
            StartCoroutine(DoFreeze());
    }

    IEnumerator DoFreeze()
    {
        ready = false;
        ui.UpdateAbilityCooldown(1f);

        GetComponent<PlayerStatus>().invulnerable = true;

        yield return new WaitForSeconds(duration);

        GetComponent<PlayerStatus>().invulnerable = false;

        float t = cooldown;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            ui.UpdateAbilityCooldown(t / cooldown);
            yield return null;
        }

        ui.UpdateAbilityCooldown(0f);
        ready = true;
    }
}

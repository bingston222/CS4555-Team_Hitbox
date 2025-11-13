using UnityEngine;

public class FreezeAbility : MonoBehaviour
{
    public float duration = 7f, cooldown = 16f;
    public KeyCode key = KeyCode.E;
    Health health; bool ready = true;

    void Awake(){ health = GetComponent<Health>(); }

    void Update(){ if (ready && Input.GetKeyDown(key)) StartCoroutine(DoFreeze()); }

    System.Collections.IEnumerator DoFreeze()
    {
        ready = false; health.SetInvulnerable(true);
        yield return new WaitForSeconds(duration);
        health.SetInvulnerable(false);
        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}

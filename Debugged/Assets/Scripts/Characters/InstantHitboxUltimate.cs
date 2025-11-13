using UnityEngine;

public class InstantHitboxUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
    public float factor = 1.8f, duration = 8f, cooldown = 30f;
    bool ready = true;

    void Update(){ if (ready && Input.GetKeyDown(key)) StartCoroutine(DoUlt()); }

    System.Collections.IEnumerator DoUlt()
    {
        ready = false;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
            foreach (var hb in e.GetComponentsInChildren<EnemyHitbox>())
                hb.Enlarge(factor);

        yield return new WaitForSeconds(duration);

        foreach (var e in enemies)
            foreach (var hb in e.GetComponentsInChildren<EnemyHitbox>())
                hb.Restore();

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}

using UnityEngine;

public class SystemRestoreUltimate : MonoBehaviour
{
    public KeyCode key = KeyCode.R;
    public float duration = 10f;
    public float cooldown = 30f;

    bool ready = true;
    Health hp;
    DamageReflector reflect;

    void Awake()
    {
        hp = GetComponent<Health>();
        reflect = GetComponent<DamageReflector>();
    }

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(DoUlt());
    }

    System.Collections.IEnumerator DoUlt()
    {
        ready = false;
        hp.SetInvulnerable(true);
        reflect.enableReflect = true;

        yield return new WaitForSeconds(duration);

        hp.SetInvulnerable(false);
        reflect.enableReflect = false;

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}

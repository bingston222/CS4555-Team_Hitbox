using UnityEngine;

public class DamageReflector : MonoBehaviour
{
    public bool enableReflect = false;

    public void ReflectDamage(GameObject attacker, float amount)
    {
        if (!enableReflect) return;

        var h = attacker.GetComponent<Health>();
        if (h) 
            h.TakeDamage(amount);
    }
}

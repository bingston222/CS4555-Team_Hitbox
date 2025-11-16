using UnityEngine;

public class FirewallShield : MonoBehaviour
{
    public bool active = false;
    public float damageReduction = 0.5f; 

    public void ActivateShield()
    {
        active = true;
    }

    public int ReduceDamage(int dmg)
    {
        if (!active) return dmg;
        return Mathf.RoundToInt(dmg * (1f - damageReduction));
    }
}

using UnityEngine;

public class Defragment : MonoBehaviour
{
    public TrojanArchitect boss;
    public float healPercent = 0.15f;
    public float radius = 10f;

    public void TryHeal()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var h in hits)
        {
            if (h.CompareTag("Glitch"))
            {
                // Calculate heal amount (float)
                float healAmountFloat = boss.maxHP * healPercent;

                // Convert float → int
                int healAmount = Mathf.RoundToInt(healAmountFloat);

                // Apply heal safely
                boss.currentHP = Mathf.Min(boss.maxHP, boss.currentHP + healAmount);

                Destroy(h.gameObject);
            }
        }
    }
}

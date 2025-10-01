using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 25; // amount restored

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(healthAmount);
            }

            // destroy after pickup
            Destroy(gameObject);
        }
    }
}

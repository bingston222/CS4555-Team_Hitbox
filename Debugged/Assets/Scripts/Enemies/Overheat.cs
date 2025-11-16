using UnityEngine;

public class Overheat : MonoBehaviour
{
    public bool hasTriggered = false;
    public GameObject flamePrefab;
    public Transform[] flameSpots;

    public void TriggerOverheat()
    {
        hasTriggered = true;

        foreach (var spot in flameSpots)
            Instantiate(flamePrefab, spot.position, Quaternion.identity);
    }
}

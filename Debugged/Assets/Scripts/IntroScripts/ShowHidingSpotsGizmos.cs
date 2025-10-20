using UnityEngine;

public class ShowHidingSpotsGizmos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform t in transform)
        {
            Gizmos.DrawSphere(t.position, 0.15f);
        }
    }
}

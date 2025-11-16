using UnityEngine;
using System.Collections;

public class MineDropper : MonoBehaviour
{
    public GameObject minePrefab;
    public Transform[] dropPoints;
    public float cooldown = 4f;
    private bool canDrop = true;

    public void DropIfReady()
    {
        if (canDrop)
            StartCoroutine(DropMine());
    }

    IEnumerator DropMine()
    {
        canDrop = false;

        int index = Random.Range(0, dropPoints.Length);
        Instantiate(minePrefab, dropPoints[index].position, Quaternion.identity);

        yield return new WaitForSeconds(cooldown);
        canDrop = true;
    }
}

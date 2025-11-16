using UnityEngine;

public class HealingPatchAbility : MonoBehaviour
{
    public KeyCode key = KeyCode.E;
    public GameObject healingProjectilePrefab;
    public Transform throwPoint;

    public float throwForce = 10f;
    public float cooldown = 12f;

    bool ready = true;

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(Fire());
    }

    System.Collections.IEnumerator Fire()
    {
        ready = false;

        GameObject obj = Instantiate(healingProjectilePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}

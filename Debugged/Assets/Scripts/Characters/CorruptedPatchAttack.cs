using UnityEngine;

public class CorruptedPatchAttack : MonoBehaviour
{
    public KeyCode key = KeyCode.Mouse0; // left click
    public GameObject projectilePrefab;
    public Transform throwPoint;

    public float throwForce = 13f;
    public float cooldown = 0.3f;

    bool ready = true;

    void Update()
    {
        if (ready && Input.GetKeyDown(key))
            StartCoroutine(Fire());
    }

    System.Collections.IEnumerator Fire()
    {
        ready = false;

        GameObject obj = Instantiate(projectilePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);

        yield return new WaitForSeconds(cooldown);
        ready = true;
    }
}

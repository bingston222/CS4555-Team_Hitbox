using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AlarmOnTouch : MonoBehaviour
{
    public string playerTag = "Player";
    public AudioSource sfx;     // assign in Inspector
    public AudioClip clip;      // assign in Inspector
    public float cooldown = 1f; // seconds

    float nextTime;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (Time.time < nextTime) return;

        if (sfx && clip) sfx.PlayOneShot(clip);
        nextTime = Time.time + cooldown;
    }
}

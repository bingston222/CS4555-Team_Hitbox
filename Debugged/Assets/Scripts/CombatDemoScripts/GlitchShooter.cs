using UnityEngine;

public class GlitchShooter : MonoBehaviour
{
    public GameObject enemyProjectilePrefab;
    public Transform muzzle;
    public float interval = 1.2f;
    public float speed = 12f;
    public Transform player;

    float nextTime;

    void Update()
    {
        if (player == null) return;
        if (Time.time < nextTime) return;
        nextTime = Time.time + interval;

        var go = Instantiate(enemyProjectilePrefab, muzzle ? muzzle.position : transform.position, Quaternion.identity);
        var dir = (player.position - (muzzle ? muzzle.position : transform.position)).normalized;
        go.transform.forward = dir;

        var rb = go.GetComponent<Rigidbody>();
        if (rb == null) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearVelocity = dir * speed;
    }
}

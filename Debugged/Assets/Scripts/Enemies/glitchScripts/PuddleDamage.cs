using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuddleDamage : MonoBehaviour
{
    public float damagePerSecond = 8f;
    private readonly System.Collections.Generic.HashSet<Health> inside = new();

    void Awake(){ var c = GetComponent<Collider>(); c.isTrigger = true; }
    void OnTriggerEnter(Collider other){ if (other.CompareTag("Player")) { var h = other.GetComponent<Health>(); if (h) inside.Add(h); } }
    void OnTriggerExit(Collider other){ if (other.CompareTag("Player")) { var h = other.GetComponent<Health>(); if (h) inside.Remove(h); } }

    void Update()
    {
        if (inside.Count == 0) return;
        float amt = damagePerSecond * Time.deltaTime;
        foreach (var h in inside) if (h) h.TakeDamage(amt);
    }
}

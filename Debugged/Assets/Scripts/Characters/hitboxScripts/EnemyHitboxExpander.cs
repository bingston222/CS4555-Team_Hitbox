using System.Collections.Generic;
using UnityEngine;

public static class EnemyHitboxExpander
{
    struct Saved
    {
        public Collider col;
        public Vector3 center;
        public Vector3 size;
        public float radius;
        public float height;
        public bool isBox, isSphere, isCapsule;
    }

    // what we modified this activation
    static readonly List<Saved> touched = new();

    /// <summary>Scales all Enemy colliders (Box/Sphere/Capsule) by 'scale'. Enemies are found by tag "Enemy" first, then by layer "Enemy".</summary>
    public static void ApplyAll(float scale)
    {
        RestoreAll(); // if something lingered from a previous run
        touched.Clear();

        // Find enemies by tag
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // If none tagged, try by layer
        if (enemies == null || enemies.Length == 0)
        {
            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer >= 0)
            {
                var all = Object.FindObjectsOfType<Collider>(includeInactive: true);
                var list = new List<GameObject>();
                foreach (var c in all)
                    if (c.gameObject.layer == enemyLayer)
                        list.Add(c.gameObject);
                enemies = list.ToArray();
            }
        }

        // Iterate
        foreach (var e in enemies)
        {
            if (!e) continue;

            // you may have multiple colliders; scale each
            foreach (var col in e.GetComponentsInChildren<Collider>(includeInactive: true))
            {
                if (!col || !col.enabled) continue;

                var s = new Saved { col = col };

                if (col is BoxCollider b)
                {
                    s.isBox = true;
                    s.center = b.center;
                    s.size = b.size;
                    b.size *= scale;
                }
                else if (col is SphereCollider sph)
                {
                    s.isSphere = true;
                    s.radius = sph.radius;
                    sph.radius *= scale;
                }
                else if (col is CapsuleCollider cap)
                {
                    s.isCapsule = true;
                    s.radius = cap.radius;
                    s.height = cap.height;
                    cap.radius *= scale;
                    cap.height *= scale;
                }
                else
                {
                    continue; // unknown collider type
                }

                touched.Add(s);
            }
        }
    }

    /// <summary>Restores every collider we scaled.</summary>
    public static void RestoreAll()
    {
        if (touched.Count == 0) return;

        for (int i = 0; i < touched.Count; i++)
        {
            var s = touched[i];
            if (!s.col) continue;

            if (s.isBox && s.col is BoxCollider b) { b.center = s.center; b.size = s.size; }
            else if (s.isSphere && s.col is SphereCollider sph) { sph.radius = s.radius; }
            else if (s.isCapsule && s.col is CapsuleCollider cap) { cap.radius = s.radius; cap.height = s.height; }
        }

        touched.Clear();
    }
}

// LifeLogger.cs
using UnityEngine;
using System.Diagnostics; // for StackTrace

public class LifeLogger : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.Debug.Log($"[LifeLogger] {name} Awake (scene={gameObject.scene.name})", this);
    }

    void Start()
    {
        UnityEngine.Debug.Log($"[LifeLogger] {name} Start", this);
    }

    void OnDisable()
    {
        UnityEngine.Debug.LogWarning($"[LifeLogger] {name} OnDisable", this);
    }

    void OnDestroy()
    {
        // If StackTrace still complains, comment out the next line and keep the LogError.
        UnityEngine.Debug.LogError($"[LifeLogger] {name} DESTROYED\n{new StackTrace(true)}", this);
    }
}

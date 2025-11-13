using System.Collections;
using UnityEngine;

public class DoorSlide : MonoBehaviour
{
    [Header("Parts")]
    public Transform panel;          // ← drag DoorPanel here
    public Collider blocker;         // ← optional: BoxCollider that blocks players

    [Header("Motion")]
    public float openDistanceY = 3.0f;   // how far up to slide
    public float duration = 1.2f;        // seconds
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio (optional)")]
    public AudioSource motor;        // attach an AudioSource on the root
    public AudioClip startClip;      // short servo start
    public AudioClip loopClip;       // looping hum/servo
    public AudioClip endClip;        // clunk at the end

    Vector3 _closedLocalPos;
    bool _isOpening;

    void Awake()
    {
        if (panel == null) panel = transform;
        _closedLocalPos = panel.localPosition;
        if (blocker == null) blocker = panel.GetComponent<Collider>();
    }

    public void Open()                       { if (!_isOpening) StartCoroutine(OpenRoutine(1f)); }
    public void OpenToPercent(float pct01)   { if (!_isOpening) StartCoroutine(OpenRoutine(Mathf.Clamp01(pct01))); }

    IEnumerator OpenRoutine(float percent)
    {
        _isOpening = true;

        // audio: start + loop
        if (motor && startClip) motor.PlayOneShot(startClip);
        if (motor && loopClip) { motor.clip = loopClip; motor.loop = true; motor.PlayDelayed(startClip ? startClip.length * 0.9f : 0f); }

        float t = 0f;
        float targetDist = openDistanceY * percent;
        Vector3 start = _closedLocalPos;
        Vector3 end = _closedLocalPos + new Vector3(0f, targetDist, 0f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = curve.Evaluate(Mathf.Clamp01(t / duration));
            panel.localPosition = Vector3.LerpUnclamped(start, end, k);
            yield return null;
        }
        panel.localPosition = end;

        // audio: stop loop + end clip
        if (motor && motor.loop) { motor.loop = false; motor.Stop(); }
        if (motor && endClip) motor.PlayOneShot(endClip);

        // disable blocker when fully open
        if (percent >= 0.999f && blocker) blocker.enabled = false;

        _isOpening = false;
    }

    // Convenience: resets the door to fully closed at runtime/editor
    public void SnapClosed()
    {
        panel.localPosition = _closedLocalPos;
        if (blocker) blocker.enabled = true;
    }
}

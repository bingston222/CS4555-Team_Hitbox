using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("How far up the door should travel (in local units).")]
    public float openDistance = 3f;
    [Tooltip("Units per second.")]
    public float speed = 2f;

    [Header("State (read-only at runtime)")]
    public bool isOpen;

    Vector3 closedLocalPos;
    Vector3 openLocalPos;
    Coroutine moveRoutine;

    void Awake()
    {
        closedLocalPos = transform.localPosition;
        openLocalPos = closedLocalPos + Vector3.up * openDistance;
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        StartMove(openLocalPos);
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        StartMove(closedLocalPos);
    }

    public void SetOpen(bool value)
    {
        if (value) Open(); else Close();
    }

    void StartMove(Vector3 target)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveTo(target));
    }

    System.Collections.IEnumerator MoveTo(Vector3 target)
    {
        // Move smoothly at constant speed
        while ((transform.localPosition - target).sqrMagnitude > 0.0001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = target;
        moveRoutine = null;
    }
}

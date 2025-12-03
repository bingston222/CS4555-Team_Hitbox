using System.Collections;
using UnityEngine;

public class CoopDoorController : MonoBehaviour
{
    [Header("References")]
    public PressurePad[] pads;
    public DoorMover door;
    public Collider doorBlocker;          // optional
    public DoorThroughWatcher through;

    [Header("Objective (optional)")]
    public InteractableFixable levelObjective;   // NEW: fuse box

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    [Range(0,1)] public float volume = 1f;

    [Header("Timing")]
    public float closeDelay = 0.5f;

    [Header("Dialogue")]
    [SerializeField] SceneBeats beats;

    bool opened, isOpening, isClosing;

    void Update()
    {
        if (pads == null || pads.Length == 0 || !door) return;

        // count occupied pads
        int occupied = 0;
        foreach (var p in pads) if (p && p.IsOccupied) occupied++;

        // 🔹 objective condition:
        // - if levelObjective is NOT set -> treat as true (old behavior)
        // - if it IS set -> require isFixed == true
        bool objectiveOk = (levelObjective == null) || levelObjective.isFixed;

        // open when both on pads AND objective is ok
        if (!opened && occupied >= 2 && objectiveOk)
            StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        if (isOpening) yield break;
        isOpening = true;

        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound, volume);

        door.Open();
        if (doorBlocker) doorBlocker.enabled = false;

        // Zone B Intro dialogue
        if (beats)
            StartCoroutine(beats.PlayOnce("zoneB.intro", beats.zoneB_Intro));

        opened = true;
        isOpening = false;

        if (through) through.ResetCount();
        if (through)
            yield return new WaitUntil(() => through.BothThrough);
        else
            yield break;

        yield return new WaitForSeconds(closeDelay);
        StartCoroutine(CloseSequence());
    }

    IEnumerator CloseSequence()
    {
        if (isClosing) yield break;
        isClosing = true;

        if (audioSource && closeSound) audioSource.PlayOneShot(closeSound, volume);
        door.Close();
        if (doorBlocker) doorBlocker.enabled = true;

        opened = false;
        isClosing = false;
    }
}

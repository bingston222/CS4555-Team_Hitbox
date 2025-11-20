using System.Collections;
using UnityEngine;

public class CoopDoorController : MonoBehaviour
{
    [Header("References")]
    public PressurePad[] pads;     // <-- supports any number of pads
    public DoorMover door;
    public Collider doorBlocker;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Timing")]
    public float openHoldTime = 3f;
    public float closeDelay = 1.5f;

    [Header("Dialogue")]
    [SerializeField] SceneBeats beats;

    bool isOpening, isClosing, opened;

    [Header("Debug")]
public bool forceTestOpen = false;

    void Update()
{
    Debug.Log("[CoopDoor] Update tick"); // should spam every frame
    if (pads == null || pads.Length == 0 || door == null)
    {
        Debug.LogWarning("[CoopDoor] Missing refs: pads/door");
        return;
    }
    
    if (pads == null || pads.Length == 0 || door == null) return;

    int occupied = 0;
    foreach (var p in pads) if (p && p.IsOccupied) occupied++;

    // TEMP DEBUG:
    Debug.Log($"[CoopDoor] occupied={occupied}, opened={opened}");

    if (occupied >= 2 && !opened) StartCoroutine(OpenSequence());
    if (opened && occupied == 0 && !isClosing) StartCoroutine(CloseSequence());
}


    IEnumerator OpenSequence()
    {
        if (isOpening) yield break;
        isOpening = true;

        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound, volume);

        door.Open();
        if (doorBlocker) doorBlocker.enabled = false;

        if (beats)
            StartCoroutine(beats.Play(beats.doorOpen));

        opened = true;
        isOpening = false;

        yield return new WaitForSeconds(openHoldTime);
    }

    IEnumerator CloseSequence()
    {
        isClosing = true;
        yield return new WaitForSeconds(closeDelay);

        if (audioSource && closeSound)
            audioSource.PlayOneShot(closeSound, volume);

        door.Close();
        if (doorBlocker) doorBlocker.enabled = true;

        opened = false;
        isClosing = false;
    }

    

void Start()
{
    Debug.Log("[CoopDoor] Start()");
    if (forceTestOpen && door)
    {
        Debug.Log("[CoopDoor] Force test open");
        door.Open(); // Door should slide up immediately on play
    }
}

}

using System.Collections;
using UnityEngine;

/// <summary>
/// Put this on each hiding spot (or its Glow).
/// </summary>
public class SpotInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Index this spot gets from the spawner.")]
    public int spotIndex = -1;

    [Header("Prefabs to optionally spawn when correct")]
    public GameObject p1ControllerPrefab;
    public GameObject p2ControllerPrefab;

    [Header("Visuals / Audio")]
    public GameObject glowRoot;     // drag the Glow object here
    public AudioSource foundSfx;    // optional ping
    public AudioSource missSfx;     // optional blip

    [Tooltip("If > 0, show spawned controller briefly before auto-collect.")]
    public float autoPickupDelay = 0f;

    void Awake()
    {
        // Ensure a trigger collider so Interactor can detect us
        var col = GetComponent<Collider>();
        if (col == null)
        {
            var sc = gameObject.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.9f;
        }
        else col.isTrigger = true;
    }

    void Start()
    {
        if (glowRoot && !glowRoot.activeSelf) glowRoot.SetActive(true);
    }

    void OnEnable()
    {
        if (GameState.I != null) GameState.I.OnBothFound += HandleBothFound;
        else StartCoroutine(WaitAndSubscribe());
    }

    void OnDisable()
    {
        if (GameState.I != null) GameState.I.OnBothFound -= HandleBothFound;
    }

    IEnumerator WaitAndSubscribe()
    {
        while (GameState.I == null) yield return null;
        GameState.I.OnBothFound += HandleBothFound;
    }

    // Interactor will usually override this with "Search (key)", but keep a fallback:
    public string GetPromptText() => "Search (E)";

    public void Interact(Interactor interactor)
    {
        var id = interactor.GetComponent<PlayerId>();
        if (id == null) return;

        bool correct = GameState.I.IsCorrectSpot(id.playerIndex, spotIndex);
        // Debug.Log($"[{name}] Player{id.playerIndex} searched spot {spotIndex}. correct={correct}");

        if (correct)
        {
            // Optional: spawn a controller briefly for feedback
            GameObject prefab = (id.playerIndex == 1) ? p1ControllerPrefab : p2ControllerPrefab;
            if (prefab != null)
            {
                if (autoPickupDelay <= 0f)
                {
                    var go = Instantiate(prefab, transform.position, transform.rotation);
                    Destroy(go);
                }
                else
                {
                    StartCoroutine(SpawnThenDestroy(prefab, autoPickupDelay));
                }
            }

            GameState.I.SetFound(id.playerIndex);
            if (foundSfx) foundSfx.Play();
            if (glowRoot) glowRoot.SetActive(false); // turn off this glow
        }
        else
        {
            if (missSfx) missSfx.Play();

            if (DialogueManager.I != null)
                DialogueManager.I.ShowShort($"Player {id.playerIndex}: Oops, not here!");
            else
                 Debug.Log($"Player {id.playerIndex}: Oops, not here!");
        }
    }

    IEnumerator SpawnThenDestroy(GameObject prefab, float delay)
    {
        var go = Instantiate(prefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }

    void HandleBothFound(int lastFinder)
    {
        if (glowRoot) glowRoot.SetActive(false); // hide any remaining glow
    }
}

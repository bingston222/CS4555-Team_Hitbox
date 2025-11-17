using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class IntroController : MonoBehaviour
{
    [Header("Players & Spawns")]
    public Transform player1;
    public Transform player2;
    public Transform spawn1;
    public Transform spawn2;

    [Header("VFX prefab")]
    public GameObject spawnVFX;

    [Header("Audio")]
    public AudioSource sfxWoosh;
    public AudioSource sfxHum;

    [Header("Camera")]
    // New (CM3)
    public CinemachineCamera vcamIntro;     // Priority 20
    public CinemachineCamera vcamGameplay;

    [Header("Light Flash")]
    public LightFlash flash;   // reference to LightFlash component

    [Header("Timings")]
    public float holdIntroCam = 2.5f;  // how long to hold intro cam before blend
    public float blendDelay = 1.5f;    // time for the blend animation
    public float radioDelay = 0.5f;    // delay before radio starts


    [Header("Dialogue Hook (optional)")]
    public bool triggerDialogueOnEnd = true;

    [Header("Combat Test")]
public GameObject testOrbPrefab;    // ← drag your TestOrb prefab here
public Transform orbSpawn1;         // ← assign two spawn points in the inspector
public Transform orbSpawn2;

    [Header("Door")]
public DoorSlide firewallDoor;   // ← drag the object with DoorSlide here
public AudioSource doorWhoosh;   // (optional) extra whoosh when opening


    void Start()
    {
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // 0) place players at spawns (if you want materialize at exact point)
        if (spawn1) player1.position = spawn1.position;
        if (spawn2) player2.position = spawn2.position;

        // 1) spawn VFX
        if (spawnVFX)
        {
            GameObject vfx1 = Instantiate(spawnVFX, player1.position, Quaternion.identity);
            GameObject vfx2 = Instantiate(spawnVFX, player2.position, Quaternion.identity);
            Destroy(vfx1, 2f); // destroy after 2 seconds
            Destroy(vfx2, 2f);
        }

        // 2) audio
        if (sfxWoosh) sfxWoosh.Play();

        if (sfxHum)
        {
            sfxHum.volume = 0f;
            sfxHum.Play();

            // fade in quickly
            StartCoroutine(FadeIn(sfxHum, 0.8f, 1f));

            // fade out after 4 seconds, over 1.5 seconds
            StartCoroutine(FadeOutEarly(sfxHum, 4f, 1.5f));
        }


        // 3) light flash
        if (flash) flash.PlayFlash();

        // 4) lock input briefly (replace with your own input gate)
        SetPlayerInput(false);

        // 5) show intro cam, then blend to gameplay cam
        if (vcamIntro) vcamIntro.Priority = 20;
        if (vcamGameplay) vcamGameplay.Priority = 10;
        yield return new WaitForSeconds(holdIntroCam);   // hold the establishing shot
        // raise gameplay priority to trigger blend (Brain handles 1s ease in/out)
        if (vcamGameplay) vcamGameplay.Priority = 30;
        yield return new WaitForSeconds(blendDelay);

        // 6) tiny pause, then radio starts
        yield return new WaitForSeconds(radioDelay);
        SetPlayerInput(false); // keep locked during first radio line if desired

        if (triggerDialogueOnEnd && Adialogue.Instance)
            Adialogue.Instance.StartBootSectorIntro();


        // 7) finally unlock input for tutorial when dialogue says so.
        // You can also wait on a dialogue callback instead of a timer.
        // For now, unlock after a short safety delay:
       
        
    }

    IEnumerator FadeIn(AudioSource src, float targetVol, float time)
    {
        float t = 0f; float start = src.volume;
        while (t < time)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, targetVol, t / time);
            yield return null;
        }
        src.volume = targetVol;
    }

    IEnumerator FadeOutEarly(AudioSource src, float delay, float fadeTime)
    {
        // wait before starting fade-out
        yield return new WaitForSeconds(delay);

        float t = 0f;
        float start = src.volume;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(start, 0f, t / fadeTime);
            yield return null;
        }

        src.Stop();
    }


    void SetPlayerInput(bool enabled)
    {
        // Replace with your own player input enabling/disabling
        // Example if using Unity Input System: toggle PlayerInput.enabled
        var p1 = player1.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        var p2 = player2.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (p1) p1.enabled = enabled;
        if (p2) p2.enabled = enabled;
        // Or call into your custom controller scripts
    }

    public void EnableTutorialPhase()
    {
        if (MovementTutorial.Instance)
    MovementTutorial.Instance.StartTutorial();

    }

    public IEnumerator OpenDoorInSteps()
{
    if (!firewallDoor) yield break;

    firewallDoor.OpenToPercent(0.33f);  // peek
    yield return new WaitForSeconds(0.35f);

    firewallDoor.OpenToPercent(0.66f);  // halfway
    yield return new WaitForSeconds(0.35f);

    firewallDoor.Open();                // full
}

public void SpawnCombatOrbs()
{
    Debug.Log("Spawning Combat Test orbs...");

    // --- HQ line before the fight ---
    if (Adialogue.Instance)
    {
        Adialogue.Instance.StartCoroutine(
            Adialogue.Instance.ShowSubtitle("Combat check: give those test orbs a tap.", 1.8f, 0.18f)
        );
    }

    // --- Spawn both orbs ---
    if (testOrbPrefab)
    {
        if (orbSpawn1)
            Instantiate(testOrbPrefab, orbSpawn1.position, Quaternion.identity);
        if (orbSpawn2)
            Instantiate(testOrbPrefab, orbSpawn2.position, Quaternion.identity);
    }
    else
    {
        Debug.LogWarning("⚠️ No TestOrb prefab assigned in IntroController!");
    }
}

}

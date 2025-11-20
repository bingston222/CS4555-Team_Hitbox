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
    public CinemachineCamera vcamIntro;     // Priority 20
    public CinemachineCamera vcamGameplay;  // Priority 30 when active

    [Header("Light Flash")]
    public LightFlash flash;

    [Header("Timings")]
    public float holdIntroCam = 2.5f;
    public float blendDelay   = 1.5f;
    public float radioDelay   = 0.5f;

    [Header("Dialogue")]
    public UnifiedDialogueController introDialogue;   // <-- drag your DialoguePanel here
    public bool lockControlsDuringDialogue = true;

    // runtime flag to know when dialogue ended (if we subscribe in code)
    bool introDialogueFinished = false;

    [Header("Calibration")]
public CalibrationTest calibration;


    void Start()
    {
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // 0) spawn players
        if (spawn1 && player1) player1.position = spawn1.position;
        if (spawn2 && player2) player2.position = spawn2.position;

        // 1) spawn VFX
        if (spawnVFX && player1 && player2)
        {
            var vfx1 = Instantiate(spawnVFX, player1.position, Quaternion.identity);
            var vfx2 = Instantiate(spawnVFX, player2.position, Quaternion.identity);
            Destroy(vfx1, 2f);
            Destroy(vfx2, 2f);
        }

        // 2) audio
        if (sfxWoosh) sfxWoosh.Play();
        if (sfxHum)
        {
            sfxHum.volume = 0f;
            sfxHum.Play();
            StartCoroutine(FadeIn(sfxHum, 0.8f, 1f));
            StartCoroutine(FadeOutEarly(sfxHum, 4f, 1.5f));
        }

        // 3) light flash
        if (flash) flash.PlayFlash();

        // 4) lock input during intro
        SetPlayerInput(false);

        // 5) intro cam -> gameplay cam
        if (vcamIntro)    vcamIntro.Priority    = 20;
        if (vcamGameplay) vcamGameplay.Priority = 10;

        yield return new WaitForSeconds(holdIntroCam);

        if (vcamGameplay) vcamGameplay.Priority = 30; // CMBrain will blend
        yield return new WaitForSeconds(blendDelay);

        // 6) small pause then start the dialogue
        yield return new WaitForSeconds(radioDelay);

        if (introDialogue)
        {
            if (lockControlsDuringDialogue) SetPlayerInput(false);

            // subscribe to the end event (in case you prefer code over inspector)
            introDialogueFinished = false;
            introDialogue.OnConversationFinished.AddListener(OnIntroDialogueFinished);

            introDialogue.gameObject.SetActive(true);  // ensure the panel object is active
            introDialogue.StartConversation();

            // wait until dialogue finishes
            yield return new WaitUntil(() => introDialogueFinished);

            // optional: unsubscribe to avoid duplicate calls next time
            introDialogue.OnConversationFinished.RemoveListener(OnIntroDialogueFinished);
        }

        // 7) done—unlock controls in case they’re still locked
        SetPlayerInput(true);
    }

    void OnIntroDialogueFinished()
{
    SetPlayerInput(true);
    if (calibration) calibration.StartCalibration();  // <-- THE HOOK
    introDialogueFinished = true;
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
        yield return new WaitForSeconds(delay);
        float t = 0f, start = src.volume;
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
        var p1 = player1 ? player1.GetComponent<UnityEngine.InputSystem.PlayerInput>() : null;
        var p2 = player2 ? player2.GetComponent<UnityEngine.InputSystem.PlayerInput>() : null;
        if (p1) p1.enabled = enabled;
        if (p2) p2.enabled = enabled;
    }
}

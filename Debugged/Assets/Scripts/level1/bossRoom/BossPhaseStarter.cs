using UnityEngine;

public class BossPhaseStarter : MonoBehaviour
{
    [Header("Enemies required to start boss phase")]
    public GameObject[] miniBosses;      // basicAnim (2), (3), (4)

    [Header("Boss")]
    public GameObject bossObject;        // root object of Firewall Overseer
    public FirewallOverseer bossScript;  // same boss object

    [Header("Music (optional)")]
    public AudioSource musicSource;      // your MusicController’s AudioSource
    public AudioClip bossMusic;          // intense boss track

    private bool bossStarted = false;

    void Start()
    {
        // Make sure boss is hidden until phase 2
        if (bossObject != null)
            bossObject.SetActive(false);
    }

    void Update()
    {
        if (bossStarted) return;

        // Check if all minibosses are destroyed
        bool allDead = true;
        foreach (GameObject mb in miniBosses)
        {
            if (mb != null)   // still alive in scene
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
            StartBossPhase();
    }

    void StartBossPhase()
    {
        bossStarted = true;

        if (bossObject != null)
            bossObject.SetActive(true);

        if (bossScript != null)
            bossScript.ActivateBoss();  // you use the AI logic we wrote earlier

        if (musicSource != null && bossMusic != null)
        {
            musicSource.clip = bossMusic;
            musicSource.Play();
        }

        // optional: destroy this script after starting
        // Destroy(this);
    }
}

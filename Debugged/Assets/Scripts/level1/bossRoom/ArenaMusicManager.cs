using UnityEngine;
using System.Collections;

public class ArenaMusicManager : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioSource bossMusic;       // intense battle theme
    public AudioSource purifiedMusic;   // calm victory theme

    [Header("Settings")]
    [Tooltip("How long (in seconds) to fade between tracks.")]
    public float fadeDuration = 2f;

    void Start()
{
    if (bossMusic)
    {
        bossMusic.loop = true;
        bossMusic.Stop();
        bossMusic.volume = 0.8f;
    }
    if (purifiedMusic)
    {
        purifiedMusic.loop = true;
        purifiedMusic.Stop();
        purifiedMusic.volume = 0.8f;
    }
}


    public void SetPurified(bool purified)
    {
        // If true -> fade boss music out and purified in
        // If false -> fade purified out and boss music in
        StopAllCoroutines();

        if (purified)
            StartCoroutine(FadeMusic(bossMusic, purifiedMusic, fadeDuration));
        else
            StartCoroutine(FadeMusic(purifiedMusic, bossMusic, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioSource fadeOut, AudioSource fadeIn, float duration)
    {
        if (fadeOut == null && fadeIn == null)
            yield break;

        float timer = 0f;
        float startVolOut = fadeOut ? fadeOut.volume : 0f;
        float startVolIn = fadeIn ? fadeIn.volume : 0.8f; // target volume

        if (fadeIn && !fadeIn.isPlaying)
        {
            fadeIn.volume = 0f;
            fadeIn.Play();
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            if (fadeOut)
                fadeOut.volume = Mathf.Lerp(startVolOut, 0f, t);

            if (fadeIn)
                fadeIn.volume = Mathf.Lerp(0f, startVolIn, t);

            yield return null;
        }

        if (fadeOut)
        {
            fadeOut.Stop();
            fadeOut.volume = startVolOut; // reset for reuse
        }

        if (fadeIn)
            fadeIn.volume = startVolIn;
    }
}

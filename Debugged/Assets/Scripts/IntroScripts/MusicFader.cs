using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicFader : MonoBehaviour
{
    public AudioSource musicSource;
    public float fadeDuration = 1.5f; // seconds

    private void Awake()
    {
        // Keep this across scenes
        DontDestroyOnLoad(gameObject);

        // Optional: listen for scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // You can automatically fade out for specific scenes if you want
        // Example: fade when reaching menu or cutscene
    }

    public void FadeOutAndStop()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();
    }
}

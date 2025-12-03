using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    public AudioSource musicSource;      // Looping music
    public AudioClip zoneMusic;          // One clip for all zones

    void Awake()
    {
        if (!musicSource)
            musicSource = GetComponent<AudioSource>();
    }

    public void PlayZoneMusic()
    {
        if (!musicSource || !zoneMusic) return;

        if (!musicSource.isPlaying)      // prevent restarting if already playing
        {
            musicSource.clip = zoneMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (!musicSource) return;
        musicSource.Stop();
    }
}

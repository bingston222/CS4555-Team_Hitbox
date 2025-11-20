using UnityEngine;
using UnityEngine.SceneManagement;

public class AlarmManager : MonoBehaviour
{
    public static AlarmManager Instance;

    [Header("Alarm Rules")]
    [SerializeField] private int maxAlarms = 2;
    [SerializeField] private float restartDelay = 0.75f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] alarmClips; // size 2 (or more)
    [SerializeField] private float alarmVolume = 0.9f;

    private int alarmCount = 0;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TriggerAlarm(Vector3 atPosition)
    {
        alarmCount++;

        // pick a clip (wrap if fewer than alarms)
        if (alarmClips != null && alarmClips.Length > 0)
        {
            var clip = alarmClips[(alarmCount - 1) % alarmClips.Length];
            audioSource.transform.position = atPosition;
            audioSource.PlayOneShot(clip, alarmVolume);
        }

        if (alarmCount >= maxAlarms)
        {
            // restart scene
            Invoke(nameof(RestartScene), restartDelay);
        }
    }

    private void RestartScene()
    {
        alarmCount = 0;
        var active = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(active);
    }
}

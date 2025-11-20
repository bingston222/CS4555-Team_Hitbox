using UnityEngine;
using System.Collections;

public class AlarmManager : MonoBehaviour
{
    public static AlarmManager Instance { get; private set; }

    [Header("Response")]
    public AudioSource sfx;
    public AudioClip alarmClip;

    [Header("Timing")]
    public float minAlarmInterval = 1.25f;
    public float playerFreeze     = 1.25f;
    public float postAlarmJam     = 1.25f;
    public float sceneStartGrace  = 1.0f;

    float _lastAlarmTime = -999f;

    void Awake() => Instance = this;
    void Start() => StartCoroutine(JamAllFor(sceneStartGrace));

    public void OnSpotted(SecurityCamera cam)
    {
        if (Time.time - _lastAlarmTime < minAlarmInterval) return;
        _lastAlarmTime = Time.time;

        if (sfx && alarmClip) sfx.PlayOneShot(alarmClip);

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in players)
        {
            var pc = p.GetComponent<PlayerCaught>();
            if (pc) pc.OnCaught(playerFreeze, cam.head ? cam.head.position : cam.transform.position);
        }

        StartCoroutine(JamAllFor(postAlarmJam));
    }

    IEnumerator JamAllFor(float seconds)
    {
        var cams = FindObjectsOfType<SecurityCamera>();
        foreach (var c in cams) c.SetJammed(true);
        yield return new WaitForSeconds(seconds);
        foreach (var c in cams) c.SetJammed(false);
    }
}

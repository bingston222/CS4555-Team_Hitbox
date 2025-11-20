using UnityEngine;
using System.Collections;

public class TwoPadJammer : MonoBehaviour
{
    public PressurePad[] pads;
    public SecurityCamera[] camerasToJam;
    public float jamDuration = 5f;
    public AudioSource sfx;
    public AudioClip startJamClip;

    bool active;

    void Update()
    {
        if (active || pads == null || pads.Length < 2) return;
        int occ = 0;
        foreach (var p in pads) if (p && p.IsOccupied) occ++;
        if (occ >= 2) StartCoroutine(Jam());
    }

    IEnumerator Jam()
    {
        active = true;
        if (sfx && startJamClip) sfx.PlayOneShot(startJamClip);

        var cams = (camerasToJam != null && camerasToJam.Length > 0)
            ? camerasToJam
            : GameObject.FindObjectsOfType<SecurityCamera>();

        foreach (var c in cams) c.SetJammed(true);
        yield return new WaitForSeconds(jamDuration);
        foreach (var c in cams) c.SetJammed(false);
        active = false;
    }
}

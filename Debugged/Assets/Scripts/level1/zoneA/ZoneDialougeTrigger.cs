using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneDialogueTrigger : MonoBehaviour
{
    [Header("Refs")]
    public SceneBeats beats;  // drag your SceneBeats here

    [Header("Behavior")]
    public string onceKey = "zoneC.intro"; // used by PlayOnce guard
    public bool onlyOnce = true;
    public string playerTag = "Player";    // whatever your player uses

    public enum ZoneSet { ZoneC_Intro, ZoneC_Complete }

[Header("Which Lines To Play")]
public ZoneSet setToPlay = ZoneSet.ZoneC_Intro;

    bool used = false;

    void Reset()
    {
        // make it a trigger by default
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (onlyOnce && used) return;
        used = true;

        if (!beats) return;

        // choose the right DialogueLine[] based on inspector selection
        UnifiedDialogueController.DialogueLine[] set = null;
        switch (setToPlay)
        {
            case ZoneSet.ZoneC_Intro:    set = beats.zoneC_Intro;    break;
            case ZoneSet.ZoneC_Complete: set = beats.zoneC_Complete; break;
        }

        // Play with the "one-shot" guard using the key
        beats.StartCoroutine(beats.PlayOnce(onceKey, set));
    }
}

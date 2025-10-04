using UnityEngine;

public class ConnectToPC : MonoBehaviour
{
    public KeyCode p1Connect = KeyCode.LeftShift;
    public KeyCode p2Connect = KeyCode.RightShift;
    public AudioSource crashAudio;
    public Animator pcAnimator; // trigger "Crash"

    bool listening;
    bool p1Pressed, p2Pressed;

    void Start()
    {
        // Either of these lines works; pick one and delete the other:
        // GameState.I.OnBothFound += _ => BeginListening();          // Option A
        GameState.I.OnBothFound += BeginListening;                    // Option B (with int param)
    }

    void BeginListening(int lastFinder) // if you used Option B
    {
        listening = true;
        p1Pressed = p2Pressed = false;
        // You could show: $"Player {lastFinder}: I found mine too—let’s connect!"
    }

    void Update()
    {
        if (!listening) return;

        if (Input.GetKeyDown(p1Connect)) p1Pressed = true;
        if (Input.GetKeyDown(p2Connect)) p2Pressed = true;

        if (p1Pressed && p2Pressed)
        {
            listening = false;
            if (crashAudio) crashAudio.Play();
            if (pcAnimator) pcAnimator.SetTrigger("Crash");
            // TODO: transition to next scene, fade out, etc.
        }
    }
}

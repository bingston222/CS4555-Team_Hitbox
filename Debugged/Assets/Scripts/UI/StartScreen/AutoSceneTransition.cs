using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneTransition : MonoBehaviour
{
    public Animator fadeAnimator;
    public float waitTime = 12f;   // match glitch duration
    public string nextScene = "Intro Level 1";

    void Start()
    {
        StartCoroutine(BeginSequence());
    }

    IEnumerator BeginSequence()
    {
        yield return new WaitForSeconds(waitTime);

        fadeAnimator.Play("FadeOut");

        yield return new WaitForSeconds(1f); // fade length
        SceneManager.LoadScene(nextScene);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator animator;
    public string fadeAnimationName;
    public float fadeDuration = 1f;

    [Header("Scene Settings")]
    public string nextSceneName;

    [Header("Optional Sound")]
    public bool playSound = false;           // <--- NEW
    public AudioClip transitionSound;        // <--- NEW
    public AudioSource audioSource;          // <--- NEW

    public void BeginTransition()
    {
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        // OPTIONAL sound (only if playSound == true)
        if (playSound && audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
            yield return new WaitForSeconds(transitionSound.length);
        }

        // Fade to black
        animator.Play(fadeAnimationName);
        yield return new WaitForSeconds(fadeDuration);

        // Load scene
        SceneManager.LoadScene(nextSceneName);
    }
}

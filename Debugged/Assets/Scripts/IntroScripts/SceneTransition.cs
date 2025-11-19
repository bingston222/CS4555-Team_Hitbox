using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    public Animator animator;          // The Animator on FadeCanvas
    public string fadeAnimationName;   // Usually "FadeToBlack"
    public float fadeDuration = 1f;    // Length of the FadeToBlack clip

    [Header("Scene Settings")]
    public string nextSceneName;       // Name of the scene to load

    // You will call THIS method when both players find their controllers
    public void BeginTransition()
    {
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        // Play the fade animation
        animator.Play(fadeAnimationName);

        // Wait for the fade animation to finish
        yield return new WaitForSeconds(fadeDuration);

        // Switch scenes
        SceneManager.LoadScene(nextSceneName);
    }
}
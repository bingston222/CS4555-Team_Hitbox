using System.Collections;           // ✔ REQUIRED for IEnumerator
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    public ScreenGlitchEffect glitchEffect;
    public Animator fadeAnimator;
    public string nextScene = "Intro Level 1";

    public void StartGame()
    {
        // stop the glitch
        if (glitchEffect != null)
            glitchEffect.glitchActive = false;

        // fade out
        if (fadeAnimator != null)
            fadeAnimator.Play("FadeOut");

        // load after fade
        StartCoroutine(LoadAfterFade());
    }

    IEnumerator LoadAfterFade()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextScene);
    }
}

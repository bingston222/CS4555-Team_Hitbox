using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Intro Level 1"); 
    }

    public void LoadGame()
    {
        Debug.Log("Load game clicked!");
        // add load logic later
    }
}

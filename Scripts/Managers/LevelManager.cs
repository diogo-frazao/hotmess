using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetryGameLevel();
        }
    }
    public void LoadGameLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("level loaded");
    }
    public void RetryGameLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("level loaded");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("menu loaded");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit game");
    }
}

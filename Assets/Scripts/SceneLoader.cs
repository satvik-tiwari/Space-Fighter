using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    // Start is called before the first frame update
    public void LoadNextScene()
    {
        int currentSceneindex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadScene(currentSceneindex + 1));
       
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
        var totalScenes = SceneManager.sceneCountInBuildSettings;
        if(sceneIndex == 0 || sceneIndex == totalScenes - 1)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LoadStartScene()
    {
        StartCoroutine(LoadScene(1));
        //FindObjectOfType<GameSession>().ResetGame();    
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene(0));
       
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

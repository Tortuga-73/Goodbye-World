using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void SelectLevel()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void OptionsMenu()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}

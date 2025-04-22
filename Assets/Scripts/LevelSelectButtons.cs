using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButtons : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadSceneAsync(0);
    }
    
    public void Level1()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void Level2()
    {
        SceneManager.LoadSceneAsync(4);
    }
}

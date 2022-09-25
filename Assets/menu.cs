using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void enterGame()
    {
        PlayerPrefs.SetInt("Points", 4);
        SceneManager.LoadScene(1);
    }

    public void nextLevel()
    {
        PlayerPrefs.SetInt("Points", PlayerPrefs.GetInt("Points") + 1);
        SceneManager.LoadScene(1);
    }
}

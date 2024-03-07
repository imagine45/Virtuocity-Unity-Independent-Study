using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuButton : MonoBehaviour
{
    public void loadFirstScene()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void endGame()
    {
        Application.Quit();
    }
}

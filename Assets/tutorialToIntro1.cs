using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialToIntro1 : MonoBehaviour
{
    public void nextScene()
    {
        SceneManager.LoadScene("Intro1");
    }
}

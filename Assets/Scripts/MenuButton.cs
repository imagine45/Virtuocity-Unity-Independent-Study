using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButton : MonoBehaviour
{
    public GameObject screenA;
    public GameObject screenB;
    public TMP_Dropdown resDropDown;
    public TMP_Dropdown fullscreenDropDown;

    private bool isFullScreen = true;
    public void loadFirstScene()
    {
        SceneManager.LoadScene("Tutorial Scene");
    }

    public void Start()
    {
        if (resDropDown != null)
        {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, isFullScreen);
        }
                
    }

    public void goTo()
    {
        screenA.SetActive(false);
        screenB.SetActive(true);
    }

    public void dropDownValueChange()
    {
        switch(fullscreenDropDown.value)
        {
            case 0:
                isFullScreen = true;
                break;
            case 1:
                isFullScreen = false;
                break;
        }
        switch(resDropDown.value)
        {
            case 0:
                Screen.SetResolution(1360, 768, isFullScreen);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, isFullScreen);
                break;
            case 2:
                Screen.SetResolution(2560, 1440, isFullScreen);
                break;
            case 3:
                Screen.SetResolution(2560, 1600, isFullScreen);
                break;
        }
    }

    

    public void endGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public GameObject screenA;
    public GameObject screenB;
    public TMP_Dropdown resDropDown;
    public TMP_Dropdown fullscreenDropDown;
    public Toggle toggleButton;
    private Color color; 

    private bool isFullScreen = true;


    public void Start()
    {
        color = this.GetComponent<Image>().color;
        Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, isFullScreen);
        SettingsManagement.instance.curResolution = Screen.resolutions[Screen.resolutions.Length - 1];
        if (toggleButton != null)
        {
            if (toggleButton.name.Equals("Speed Warping"))
            {
                toggleButton.SetIsOnWithoutNotify(SettingsManagement.instance.warpingActive);
            }
            //add more toggle effects here
        }
    }

    public void loadFirstScene()
    {
        StartCoroutine(exitAnim(4, "Tutorial Scene"));
    }

    IEnumerator exitAnim(int time, string scene)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Loading scene");
        SceneManager.LoadScene(scene);
    }

    public void sizeOnHover()
    {
        this.GetComponent<Image>().color = new Color(0.1f, 0.8f, 0.97f);
    }

    public void sizeOnExit()
    {
        this.GetComponent<Image>().color = color; 
    }

    public void exitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void continueGame()
    {
        SettingsManagement.instance.loadedFromContinue = true;
        StartCoroutine(exitAnim(4, SceneManager.GetSceneAt(SettingsManagement.instance.currentScene).name));
    }

    public void goTo()
    {
        screenA.SetActive(false);
        screenB.SetActive(true);
    }

    public void warpOn()
    {
        SettingsManagement.instance.warpingActive = !SettingsManagement.instance.warpingActive;
        Debug.Log(SettingsManagement.instance.warpingActive);
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
        SettingsManagement.instance.saveSettings();
        Application.Quit();
    }
}

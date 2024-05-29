using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{

    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] GameObject player;
    private Color color;


    private void Start()
    {
        player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().isPaused = false;
    }

    public void Pause()
    {
        Debug.Log("pausing");
        player.GetComponent<PlayerController>().isPaused = true;
        m_pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void sizeOnHover()
    {
        this.GetComponent<Image>().color = new Color(0.1f, 0.8f, 0.97f);
    }

    public void sizeOnExit()
    {
        this.GetComponent<Image>().color = color;
    }

    public void Resume()
    {
        player.GetComponent<PlayerController>().isPaused = false;
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        player.GetComponent<PlayerController>().isPaused = false;
        m_pauseMenu.SetActive(false);
        //player.transform.position = new Vector3(-60, 3, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        SettingsManagement.instance.saveSettings();
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

    public void ExitToDesktop()
    {
        SettingsManagement.instance.saveSettings();
        Application.Quit();
    }
}

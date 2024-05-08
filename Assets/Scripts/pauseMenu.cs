using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{

    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    public void Pause()
    {
        player.GetComponent<PlayerController>().isPaused = true;
        m_pauseMenu.SetActive(true);
        Time.timeScale = 0f;
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
        player.transform.position = new Vector3(-60, 3, 0);
        Time.timeScale = 1f;
    }

    public void ExitToMenu()
    {
        SettingsManagement.instance.saveSettings();
        SceneManager.LoadScene("Menu");
    }

    public void ExitToDesktop()
    {
        SettingsManagement.instance.saveSettings();
        Application.Quit();
    }
}

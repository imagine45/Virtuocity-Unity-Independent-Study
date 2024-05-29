using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SettingsManagement : MonoBehaviour
{
    public static SettingsManagement instance;

    public bool warpingActive = true;
    public int currentScene;
    public Vector2 checkpoint;
    public bool loadedFromContinue;
    public float sfxVolume = 1;
    public float musicVolume = 1;
    public int resDropdownIndex;
    public int fullscreenDropdownIndex;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        loadSettings();
    }

    [System.Serializable]
    class SaveData : MonoBehaviour
    {
        //Display
        public bool warpingActive = true;
        public int resDropdownIndex;
        public int fullscreenDropdownIndex;
        public int currentScene;
        public Vector2 checkpoint;
        public float sfxVolume = 1;
        public float musicVolume = 1;
    }

    public void saveSettings()
    {
        SaveData data = new SaveData();

        data.warpingActive = warpingActive;
        data.resDropdownIndex = resDropdownIndex;
        data.fullscreenDropdownIndex = fullscreenDropdownIndex;
        data.checkpoint = checkpoint;
        data.sfxVolume = sfxVolume;
        data.musicVolume = musicVolume;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            data.currentScene = SceneManager.GetActiveScene().buildIndex;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void loadSettings()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            SaveData data = new SaveData();
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, data);

            warpingActive = data.warpingActive;
            resDropdownIndex = data.resDropdownIndex;
            fullscreenDropdownIndex = data.fullscreenDropdownIndex;
            checkpoint = data.checkpoint;
            currentScene = data.currentScene;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;
        }
        else
        {
            warpingActive = true;
            resDropdownIndex = 0;
            fullscreenDropdownIndex = 0;
            musicVolume = 1;
            sfxVolume = 1;
        }
    }
}

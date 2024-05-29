using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SettingsManagement : MonoBehaviour
{
    public static SettingsManagement instance;

    public bool warpingActive = true;
    public Resolution curResolution;
    public int currentScene;
    public Vector2 checkpoint;
    public bool loadedFromContinue;
    public float sfxVolume;
    public float musicVolume;

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
        public Resolution curResolution;
        public int currentScene;
        public Vector2 checkpoint;
        public float sfxVolume;
        public float musicVolume;
    }

    public void saveSettings()
    {
        SaveData data = new SaveData();

        data.warpingActive = warpingActive;
        data.curResolution = curResolution;
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
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            warpingActive = data.warpingActive;
            curResolution = data.curResolution;
            checkpoint = data.checkpoint;
            currentScene = data.currentScene;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;
        }
    }
}

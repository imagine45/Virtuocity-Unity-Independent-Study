using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class audioSettings : MonoBehaviour
{

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private float musicVal = 0.75f;
    private float sfxVal = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music Volume", musicVal);
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX Volume", sfxVal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeSFXVolume()
    {
        SettingsManagement.instance.sfxVolume = sfxSlider.GetComponent<Slider>().value;
        Debug.Log(sfxSlider.GetComponent<Slider>().value);
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX Volume", sfxSlider.GetComponent<Slider>().value);
    }

    public void changeMusicVolume()
    {
        SettingsManagement.instance.sfxVolume = musicSlider.GetComponent<Slider>().value;
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music Volume", musicSlider.GetComponent<Slider>().value);
    }
}

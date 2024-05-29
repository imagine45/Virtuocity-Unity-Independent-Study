using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Setting : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.GetComponent<TMP_Dropdown>() != null)
        {
            if (this.gameObject.name.Equals("Resolution Dropdown"))
            {
                this.gameObject.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(SettingsManagement.instance.resDropdownIndex);
            }
            if (this.gameObject.name.Equals("isFullScreen"))
            {
                this.gameObject.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(SettingsManagement.instance.fullscreenDropdownIndex);
            }
            
        }
        if (this.gameObject.GetComponent<Slider>() != null)
        {
            if (gameObject.name.Equals("Music Volume Slider"))
            {
                this.gameObject.GetComponent<Slider>().SetValueWithoutNotify(SettingsManagement.instance.musicVolume);
                Debug.Log("set it and forgot it");
            }
            if (gameObject.name.Equals("SFX Volume Slider"))
            {
                this.gameObject.GetComponent<Slider>().SetValueWithoutNotify(SettingsManagement.instance.sfxVolume);
            }
        }
        if (this.gameObject.GetComponent<Toggle>() != null)
        {
            if (this.gameObject.name.Equals("Speed Warping"))
            {
                this.gameObject.GetComponent<Toggle>().SetIsOnWithoutNotify(SettingsManagement.instance.warpingActive);
            }
        }
    }

}

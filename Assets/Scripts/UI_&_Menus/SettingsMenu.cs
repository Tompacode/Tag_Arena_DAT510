using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++){
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height){
                    currentResolutionIndex = i;
                }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
        for (int i = 0; i < options.Count; i++){
            resolutionDropdown.AddOptions(options);
        }

    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log($"Set resolution to {resolution.width} x {resolution.height}");
    }

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private bool hasAutoScrolledOpenList;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    private void Update()
    {
        AutoScrollOpenDropdownToCurrentSelection();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log($"Set resolution to {resolution.width} x {resolution.height}");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void AutoScrollOpenDropdownToCurrentSelection()
    {
        GameObject openList = GameObject.Find("Dropdown List");

        if (openList == null)
        {
            hasAutoScrolledOpenList = false;
            return;
        }

        if (hasAutoScrolledOpenList)
        {
            return;
        }

        ScrollRect scrollRect = openList.GetComponentInChildren<ScrollRect>();
        if (scrollRect == null || resolutionDropdown.options.Count <= 1)
        {
            hasAutoScrolledOpenList = true;
            return;
        }

        float t = (float)resolutionDropdown.value / (resolutionDropdown.options.Count - 1);
        scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(t);

        hasAutoScrolledOpenList = true;
    }
}
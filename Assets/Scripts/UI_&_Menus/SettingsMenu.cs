using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private readonly List<Resolution> uniqueResolutions = new List<Resolution>();

    private const int DefaultWidth = 1920;
    private const int DefaultHeight = 1080;

    private void Start()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(DefaultWidth, DefaultHeight, false);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> seen = new HashSet<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string key = $"{resolutions[i].width}x{resolutions[i].height}";
            if (!seen.Add(key))
            {
                continue;
            }

            uniqueResolutions.Add(resolutions[i]);
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

            if (resolutions[i].width == DefaultWidth &&
                resolutions[i].height == DefaultHeight)
            {
                currentResolutionIndex = uniqueResolutions.Count - 1;
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
        if (resolutionIndex < 0 || resolutionIndex >= uniqueResolutions.Count)
        {
            return;
        }

        Resolution resolution = uniqueResolutions[resolutionIndex];
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(resolution.width, resolution.height, false);
        Debug.Log($"Set resolution to {resolution.width} x {resolution.height} (Windowed)");
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
            return;
        }

        ScrollRect scrollRect = openList.GetComponentInChildren<ScrollRect>();
        if (scrollRect == null || resolutionDropdown.options.Count <= 1)
        {
            return;
        }

        int selectedIndex = GetOpenDropdownSelectedIndex(openList);
        float t = (float)selectedIndex / (resolutionDropdown.options.Count - 1);
        scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(t);
    }

    private int GetOpenDropdownSelectedIndex(GameObject openList)
    {
        int fallback = Mathf.Clamp(resolutionDropdown.value, 0, resolutionDropdown.options.Count - 1);

        if (EventSystem.current == null)
        {
            return fallback;
        }

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || !selected.transform.IsChildOf(openList.transform))
        {
            return fallback;
        }

        Toggle toggle = selected.GetComponent<Toggle>();
        if (toggle == null)
        {
            return fallback;
        }

        // TMP dropdown content can contain a hidden template item at index 0.
        int siblingIndex = toggle.transform.GetSiblingIndex();
        int logicalIndex = siblingIndex;

        Transform parent = toggle.transform.parent;
        if (parent != null && parent.childCount > 0)
        {
            Toggle firstChildToggle = parent.GetChild(0).GetComponent<Toggle>();
            if (firstChildToggle != null && !firstChildToggle.gameObject.activeInHierarchy)
            {
                logicalIndex = siblingIndex - 1;
            }
        }

        return Mathf.Clamp(logicalIndex, 0, resolutionDropdown.options.Count - 1);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class PickMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI menuPanelTextField;
    const int MAX_TEAM = 2;

    enum Picker
    {
        Player1,
        Player2
    }

    int pickStep = 0;

    Picker[] pickOrder =
    {
    Picker.Player1,
    Picker.Player2,
    Picker.Player2,
    Picker.Player1
    };

    Picker currentPicker = Picker.Player1;

    List<string> player1Team = new List<string>();
    List<string> player2Team = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePickerText();
    }

    // Update is called once per frame
    // void Update()
    // {}

    void UpdatePickerText()
    {
        if (menuPanelTextField != null) // Undviker NullReferenceException
        {
            if (currentPicker == Picker.Player1)
            {
                menuPanelTextField.text = "Player 1 is picking";
            }
            else
            {
                menuPanelTextField.text = "Player 2 is picking";
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Borde fortfarande funka för snabba scener?
        // Alt. SceneManager.LoadSceneAsync("GameScene"); för load in background 

        Debug.Log("Back to main menu");
    }

    public void finishedSelectionPlay()
    {
        // Alt. SceneManager.LoadSceneAsync("PlayScene");
        SceneManager.LoadScene("PlayScene");
        Debug.Log("To PlayScene"); 
    }

    public void HandlePick()
    {
        // Varannan spelare väljer 
        // if (currentPicker == Picker.Player1)
        //    currentPicker = Picker.Player2;
        // else
        //    currentPicker = Picker.Player1;

        // 1 2 2 1 

        if (pickStep >= pickOrder.Length - 1)
        {
            Debug.Log("Picking complete!");
            return;
        }

        pickStep++;
        currentPicker = pickOrder[pickStep];

        UpdatePickerText();
    }

    public void Viking1Clicked()
    {
        Debug.Log("Viking 1 selected");

        HandlePick();
    }

    public void Viking2Clicked()
    {
        Debug.Log("Viking 2 selected");

        HandlePick();
    }




}

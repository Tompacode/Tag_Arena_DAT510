using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PlayGame(){
        SceneManager.LoadScene("TeamPickMenu");
    }

    public void Settings(){
        SceneManager.LoadScene("SettingsMenu");
    }

    public void QuitGame(){
        Debug.Log("Quit!");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

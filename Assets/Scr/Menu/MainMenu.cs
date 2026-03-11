using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    void Start()
    {
        mainMenuUI.SetActive(true);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void OpenOptions()
    {
        // Implement options menu functionality here
        mainMenuUI.SetActive(true);
    }
    public void CloseOptions()
    {
        // Implement options menu functionality here
        mainMenuUI.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

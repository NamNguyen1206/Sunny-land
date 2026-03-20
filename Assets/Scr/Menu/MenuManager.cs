using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject aboutMenuUI;
    private bool isMenuOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        mainMenuUI.SetActive(isMenuOpen);

        // Khi mở menu → tắt about
        if (isMenuOpen)
        {
            aboutMenuUI.SetActive(false);
            Time.timeScale = 0f; // pause
        }
        else
        {
            Time.timeScale = 1f; // resume
        }
    }

    public void ResumeGame()
    {
        isMenuOpen = false;
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

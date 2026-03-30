using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject aboutMenuUI;

    [Header("About Pages")]
    public GameObject page1; // Kéo Object 'Page 1' vào đây
    public GameObject page2; // Kéo Object 'Page 2' vào đây
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
            // Đảm bảo cửa sổ About luôn bắt đầu từ Trang 1
            if (page1 != null) page1.SetActive(true);
            if (page2 != null) page2.SetActive(false);
            Time.timeScale = 0f; // pause
        }
        else
        {
            Time.timeScale = 1f; // resume
        }
    }
    public void GoToNextPage()
    {
    page1.SetActive(false); // Ẩn trang 1
    page2.SetActive(true);  // Hiện trang 2
    Debug.Log("Switched to Page 2");
    }
    public void GoToPreviousPage()
    {
    page1.SetActive(true); // Hiện trang 1
    page2.SetActive(false); // Ẩn trang 2
    }

    public void ResumeGame()
    {
        isMenuOpen = false;
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void QuitToMainMenu()
    {
        Debug.Log("Quit to Main Menu");
        Time.timeScale = 1f; // reset thời gian về 1 trước khi chuyển cảnh
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

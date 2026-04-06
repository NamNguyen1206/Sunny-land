using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;
    [Header("UI Panels")]
    public GameObject mainMenuUI;
    public GameObject aboutMenuUI;
    public GameObject pauseMenuUI;
    public GameObject levelMenuUI;
    public GameObject BackGroundUI;
    public GameObject TittleUI;

    [Header("About Pages")]
    public GameObject[] pages; // Kéo các pages vào đây
    //public GameObject page1; // Cách cũ - Kéo Object 'Page 1' vào đây
    //public GameObject page2; // Cách cũ - Kéo Object 'Page 2' vào đây
    public int TotalPageNumber = 3;
    private int currentPageIndex = 0;

    private bool isMenuOpen = false;
    private Scene _currentScene;
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        //PlayButton.onClick.AddListener(PlayGame);
        //QuitButton.onClick.AddListener(QuitGame);
        if (PlayButton != null)
    {
    PlayButton.onClick.AddListener(PlayGame);
    }

    if (QuitButton != null)
    {
        QuitButton.onClick.AddListener(QuitGame);
    }
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //PlayButton.onClick.RemoveListener(PlayGame);
        //QuitButton.onClick.RemoveListener(QuitGame);
        if (PlayButton != null)
    {
    PlayButton.onClick.RemoveListener(PlayGame);
    }

    if (QuitButton != null)
    {
        QuitButton.onClick.RemoveListener(QuitGame);
    }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log($"{_currentScene.name}");
            if (_currentScene.name != "MainMenu")
            {
                ToggleMenu();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene;
        if(scene.name == "MainMenu")
        {
            BackGroundUI.SetActive(true);
            mainMenuUI.SetActive(true);
            TittleUI.SetActive(true);
            aboutMenuUI.SetActive(false);
        }
        else
        {
            BackGroundUI.SetActive(false);
            mainMenuUI.SetActive(false);
            aboutMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            TittleUI.SetActive(false);
        }
    }

    public void PlayGame ()
    {
        BackGroundUI.SetActive(false);
        TittleUI.SetActive(false);
        SceneManager.LoadScene("Lv_1");
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        //mainMenuUI.SetActive(isMenuOpen);
        pauseMenuUI.SetActive(isMenuOpen);

        // Khi mở menu → tắt about
        if (isMenuOpen)
        {
            aboutMenuUI.SetActive(false);
            currentPageIndex = 0;
            // Cách cũ: if (page1 != null) page1.SetActive(true);
            // Cách cũ: if (page2 != null) page2.SetActive(false);
            UpdatePageDisplay();
            Time.timeScale = 0f; // pause
        }
        else
        {
            Time.timeScale = 1f; // resume
        }
    }
    public void GoToNextPage()
    {
        if (currentPageIndex < TotalPageNumber - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
            Debug.Log("Switched to Page " + (currentPageIndex + 1));
        }
        // Cách cũ:
        //page1.SetActive(false); // Ẩn trang 1
        //page2.SetActive(true);  // Hiện trang 2
    }
    public void GoToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
            Debug.Log("Switched to Page " + (currentPageIndex + 1));
        }
        // Cách cũ:
        //page1.SetActive(true); // Hiện trang 1
        //page2.SetActive(false); // Ẩn trang 2
    }
    private void UpdatePageDisplay()
    {
        if (pages == null || pages.Length == 0) return;
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
    }

    public void ResumeGame()
    {
        isMenuOpen = false;
        mainMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    public void LoadLevel(int levelIndex)
    {
        if (levelMenuUI != null)
        {
            levelMenuUI.SetActive(false);  // Tắt levelMenuUI
            TittleUI.SetActive(false); // Tắt TittleUI nếu cần thiết
        }
        Time.timeScale = 1f; // Đảm bảo thời gian trở lại bình thường khi tải level mới
        SceneManager.LoadScene(levelIndex);
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

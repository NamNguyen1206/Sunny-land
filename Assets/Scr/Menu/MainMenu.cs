using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button QuitButton;
    private void Awake()
    {
        PlayButton.onClick.AddListener(PlayGame);
        QuitButton.onClick.AddListener(QuitGame);
    }
    private void OnDestroy()
    {
        PlayButton.onClick.RemoveListener(PlayGame);
        QuitButton.onClick.RemoveListener(QuitGame);
    }
    public void PlayGame ()
    {
        SceneManager.LoadScene("Lv_1");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

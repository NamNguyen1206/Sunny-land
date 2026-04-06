using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f; // Đảm bảo thời gian trở lại bình thường khi tải level mới
        SceneManager.LoadScene(levelIndex);
    }
}

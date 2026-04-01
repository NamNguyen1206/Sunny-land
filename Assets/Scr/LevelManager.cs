using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    public static void LoadLevel(int currentCoins)
    {
        if (currentCoins > 5)
        {
            Debug.Log("So coin phai lon hon 5 de chuyen sang level 2");
            SceneManager.LoadScene("Lv_2");
        }
    }
}

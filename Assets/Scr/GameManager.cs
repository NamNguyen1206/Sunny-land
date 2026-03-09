using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = true;
    public static GameManager instance;

    public Transform currentCheckpoint;
    public GameObject playerPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void RespawnPlayer()
    {
        Instantiate(playerPrefab, currentCheckpoint.position, Quaternion.identity);
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        currentCheckpoint = newCheckpoint;
    }
    public void ResetAllEnemies()
    {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in allEnemies)
        {
            enemy.Reset();
        }
    }
}

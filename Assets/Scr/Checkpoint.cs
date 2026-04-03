using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated)
        {
            return;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            player = other.GetComponentInParent<Player>();
        }

        if (player == null)
        {
            return;
        }

        GameManager gameManager = GameManager.instance;
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (gameManager == null)
        {
            Debug.LogWarning("Checkpoint: Khong tim thay GameManager de luu checkpoint.", this);
            return;
        }

        gameManager.SetCheckpoint(transform);
        isActivated = true;
        Debug.Log("Checkpoint activated: " + transform.position, this);
    }
}

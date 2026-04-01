using UnityEngine;

public class BringerOfDeathHitBox : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.CompareTag("Player"))
        // {
        //     Player player = other.GetComponent<Player>();
        //     if (player != null)
        //     {
        //         player.TakeDamage(damage);
        //     }
        // }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            player = other.GetComponentInParent<Player>();
        }

        if (player != null)
        {
            Debug.Log("Bringer Of Death HitBox: Trigger hit player for " + damage + " damage.", player);
            player.TakeDamage(damage);
        }
    }
}

using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public Transform[] points;
    public float moveTime = 1.5f;
    public int damage = 20;

    int currentPoint = 0;

    void Update()
    {
        // Di chuyển saw
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[currentPoint].position,
            Time.deltaTime * (1f / moveTime)
        );

        // Khi đến điểm thì đổi hướng
        if (Vector2.Distance(transform.position, points[currentPoint].position) < 0.01f)
        {
            currentPoint++;

            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
    if (collision.CompareTag("Player"))
        {
        collision.GetComponent<Player>().TakeDamage(damage);
        }
    }
}

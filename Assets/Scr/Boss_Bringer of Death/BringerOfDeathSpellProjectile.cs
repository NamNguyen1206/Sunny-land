using UnityEngine;

public class BringerOfDeathSpellProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed;
    private float hitRadius;
    private int damage;
    private float reachThreshold;
    private LayerMask playerLayer;
    private bool initialized;
    private bool hasHit;

    public void Initialize(
        Vector3 targetPosition,
        float moveSpeed,
        float hitRadius,
        int damage,
        LayerMask playerLayer,
        float lifetime)
    {
        this.targetPosition = targetPosition;
        this.moveSpeed = moveSpeed;
        this.hitRadius = hitRadius;
        this.damage = damage;
        this.playerLayer = playerLayer;
        reachThreshold = 0.05f;
        initialized = true;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!initialized || hasHit)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        TryHitPlayer();

        if (!hasHit && Vector2.Distance(transform.position, targetPosition) <= reachThreshold)
        {
            TryHitPlayer();
            Destroy(gameObject);
        }
    }

    private void TryHitPlayer()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, hitRadius, playerLayer);

        for (int i = 0; i < hitPlayers.Length; i++)
        {
            Player player = hitPlayers[i].GetComponent<Player>();
            if (player == null)
            {
                player = hitPlayers[i].GetComponentInParent<Player>();
            }

            if (player != null)
            {
                hasHit = true;
                player.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
    }
}

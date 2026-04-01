using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BringerOfDeathSpellArea : MonoBehaviour
{
    private int damage;
    private LayerMask playerLayer;
    private bool initialized;
    private bool hasDamagedPlayer;

    public void Initialize(int damage, LayerMask playerLayer, float lifetime)
    {
        this.damage = damage;
        this.playerLayer = playerLayer;
        hasDamagedPlayer = false;
        initialized = true;

        Collider2D spellCollider = GetComponent<Collider2D>();
        if (spellCollider != null)
        {
            spellCollider.isTrigger = true;
        }

        TryDamagePlayersAlreadyInside();
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamageCollider(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TryDamageCollider(other);
    }

    private void TryDamagePlayersAlreadyInside()
    {
        CircleCollider2D spellCollider = GetComponent<CircleCollider2D>();
        if (spellCollider == null)
        {
            return;
        }

        float worldScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        if (Mathf.Approximately(worldScale, 0f))
        {
            worldScale = 1f;
        }

        Vector2 worldCenter = spellCollider.bounds.center;
        float worldRadius = spellCollider.radius * worldScale;
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(worldCenter, worldRadius, playerLayer);

        for (int i = 0; i < hitPlayers.Length; i++)
        {
            if (TryDamageCollider(hitPlayers[i]))
            {
                return;
            }
        }
    }

    private bool TryDamageCollider(Collider2D other)
    {
        if (!initialized || hasDamagedPlayer || other == null)
        {
            return false;
        }

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            player = other.GetComponentInParent<Player>();
        }

        if (player == null)
        {
            return false;
        }

        hasDamagedPlayer = true;
        player.TakeDamage(damage);
        return true;
    }
}

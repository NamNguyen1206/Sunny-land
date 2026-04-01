using UnityEngine;

public class BringerOfDeathBoss : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float chaseRange = 6f;
    public float chaseLoseRange = 8f;
    public float movementDeadZone = 0.05f;

    [Header("Attack")]
    public Transform attackCenter;
    public float attackRadius = 1.5f;
    public LayerMask playerLayer;
    public int damage = 10;

    [Header("Spell Attack")]
    public int spellDamage = 10;
    public float spellRadius = 2f;
    public float spellMinSpawnDistance = 3f;
    public float spellMaxSpawnDistance = 6f;
    public float spellSpawnForwardOffset = 1.5f;
    public float spellProjectileSpeed = 8f;
    public float spellVerticalOffset = 0.5f;
    public float spellLifetime = 1.6f;
    public float spellCastDuration = 0.9f;
    public float spellSpawnDelay = 0.45f;
    public int spellSortingOrderOffset = 1;
    public GameObject spellAreaPrefab;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("References")]
    public Animator animator;
    public Rigidbody2D rb;
    public GameObject chestPrefab;
    public Vector2 chestSpawnOffset = new Vector2(0f, -1.25f);
    public Transform groundCheck;
    public float checkDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Animation Names")]
    public string idleAnimationName = "Idle";
    public string walkAnimationName = "Walk";
    public string attackAnimationName = "Attack";
    public string castAnimationName = "Cast";
    public string spellAnimationName = "Spell";
    public string hurtAnimationName = "Hurt";
    public string deathAnimationName = "Death";

    [Header("Visual Direction")]
    public bool spriteFacesRightByDefault = false;

    private bool movingRight = true;
    private float desiredHorizontalSpeed;

    [HideInInspector] public BringerOfDeathStateMachine stateMachine;

    [HideInInspector] public BringerOfDeathIdleState idleState;
    [HideInInspector] public BringerOfDeathChaseState chaseState;
    [HideInInspector] public BringerOfDeathAttackState attackState;
    [HideInInspector] public BringerOfDeathCastState castState;
    [HideInInspector] public BringerOfDeathDeathState deathState;
    [HideInInspector] public BringerOfDeathTakeHitState takeHitState;
    [HideInInspector] public BringerOfDeathPatrolState patrolState;

    void Awake()
    {
        CacheReferences();
        SyncLogicalFacingFromScale();
    }

    void Start()
    {
        CacheReferences();
        currentHealth = maxHealth;
        stateMachine = new BringerOfDeathStateMachine();

        idleState = new BringerOfDeathIdleState(this);
        chaseState = new BringerOfDeathChaseState(this);
        attackState = new BringerOfDeathAttackState(this);
        castState = new BringerOfDeathCastState(this);
        deathState = new BringerOfDeathDeathState(this);
        takeHitState = new BringerOfDeathTakeHitState(this);
        patrolState = new BringerOfDeathPatrolState(this);

        stateMachine.ChangeState(patrolState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        ApplyHorizontalVelocity();
    }

    public void TakeDamage(int incomingDamage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= incomingDamage;
        PlayAnimation(hurtAnimationName, true);

        if (currentHealth <= 0)
        {
            stateMachine.ChangeState(deathState);
        }
        else
        {
            stateMachine.ChangeState(takeHitState);
        }
    }

    public void SpawnChest()
    {
        // SpawnChest cua Demon_Boss: tao chest tai transform.position roi huy boss.
        if (chestPrefab != null)
        {
            Vector3 chestSpawnPosition = transform.position + (Vector3)chestSpawnOffset;
            // Instantiate(chestPrefab, transform.position, Quaternion.identity);
            Instantiate(chestPrefab, chestSpawnPosition, Quaternion.identity);
            // Cach cu: dat chest xuong dung chan boss.
            // GameObject spawnedChest = Instantiate(chestPrefab, transform.position, Quaternion.identity);
            // spawnedChest.transform.position = GetChestSpawnPosition(spawnedChest);
            Destroy(gameObject); // optional: xoá boss
        }

        Destroy(gameObject); // optional: xoa boss
    }

    public void Patrol()
    {
        // float direction = movingRight ? 1f : -1f;
        // rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        // RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
        // if (!groundInfo.collider)
        // {
        //     Flip();
        // }

        if (rb == null || groundCheck == null)
        {
            StopMoving();
            return;
        }

        float direction = movingRight ? 1f : -1f;
        FaceDirection(direction);

        if (!HasGroundAhead())
        {
            StopMoving();
            Flip();
            return;
        }

        SetHorizontalDirection(direction);
    }

    public float DistanceToPlayer()
    {
        if (player == null)
        {
            return Mathf.Infinity;
        }

        // return Vector2.Distance(transform.position, player.position);
        return Mathf.Abs(player.position.x - transform.position.x);
    }

    public void MoveToPlayer()
    {
        if (player == null)
        {
            StopMoving();
            return;
        }

        // Vector2 dir = (player.position - transform.position).normalized;
        // rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
        // if (dir.x > 0f)
        // {
        //     transform.localScale = new Vector3(1f, 1f, 1f);
        //     movingRight = true;
        // }
        // else if (dir.x < 0f)
        // {
        //     transform.localScale = new Vector3(-1f, 1f, 1f);
        //     movingRight = false;
        // }

        float directionToPlayer = player.position.x - transform.position.x;

        if (Mathf.Abs(directionToPlayer) <= movementDeadZone)
        {
            StopMoving();
            return;
        }

        FaceDirection(directionToPlayer);

        if (!HasGroundAhead())
        {
            StopMoving();
            return;
        }

        SetHorizontalDirection(directionToPlayer);
    }

    public void StopMoving()
    {
        if (rb != null)
        {
            // rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            desiredHorizontalSpeed = 0f;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    public void DealDamage()
    {
        if (player == null || attackCenter == null)
        {
            Debug.LogWarning("Bringer Of Death: Missing player or attackCenter, cannot deal damage.", this);
            return;
        }

        // float distance = Vector2.Distance(attackCenter.position, player.position);
        // if (distance <= attackRadius)
        // {
        //     Player playerComponent = player.GetComponent<Player>();
        //     if (playerComponent != null)
        //     {
        //         playerComponent.TakeDamage(damage);
        //     }
        // }

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackCenter.position, attackRadius, playerLayer);
        Debug.Log("Bringer Of Death: DealDamage called. Hits found = " + hitPlayers.Length, this);

        if (hitPlayers.Length == 0)
        {
            Debug.Log("Bringer Of Death: No player inside attack radius at " + attackCenter.position + " with radius " + attackRadius, this);
        }

        for (int i = 0; i < hitPlayers.Length; i++)
        {
            Player playerComponent = hitPlayers[i].GetComponent<Player>();
            if (playerComponent == null)
            {
                playerComponent = hitPlayers[i].GetComponentInParent<Player>();
            }

            if (playerComponent != null)
            {
                Debug.Log("Bringer Of Death: Hit player and applied " + damage + " damage.", playerComponent);
                playerComponent.TakeDamage(damage);
                break;
            }
        }
    }

    public void OnParried()
    {
        stateMachine.ChangeState(takeHitState);

        if (rb != null && player != null)
        {
            Vector2 pushDir = (transform.position - player.position).normalized;
            rb.AddForce(pushDir * 5f, ForceMode2D.Impulse);
        }
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation(idleAnimationName);
    }

    public void PlayWalkAnimation()
    {
        PlayAnimation(walkAnimationName);
    }

    public void PlayAttackAnimation()
    {
        PlayAnimation(attackAnimationName);
    }

    public void PlayCastAnimation()
    {
        PlayAnimation(castAnimationName, true);
    }

    public void PlayHurtAnimation()
    {
        PlayAnimation(hurtAnimationName, true);
    }

    public void PlayDeathAnimation()
    {
        PlayAnimation(deathAnimationName);
    }

    public void PlayAnimation(string animationName, bool restart = false)
    {
        if (animator == null || string.IsNullOrEmpty(animationName))
        {
            return;
        }

        animator.Play(animationName, -1, restart ? 0f : float.NegativeInfinity);
    }

    public void SpawnSpellAttack()
    {
        if (player == null)
        {
            Debug.LogWarning("Bringer Of Death: Missing player, cannot spawn spell attack.", this);
            return;
        }

        Vector3 spellSpawnPosition = GetSpellSpawnPosition();
        CreateSpellArea(spellSpawnPosition);

        // Vector3 spellSpawnPosition = GetSpellSpawnPosition();
        // Vector3 spellTargetPosition = GetSpellTargetPosition();
        // CreateSpellProjectile(spellSpawnPosition, spellTargetPosition);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);
        }

        if (attackCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCenter.position, attackRadius);
        }

        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(GetSpellSpawnPosition(), GetSpellAreaRadiusForGizmo());
            // Gizmos.DrawLine(GetSpellSpawnPosition(), GetSpellTargetPosition());
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        ApplyFacing(movingRight);
    }

    public void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        FaceDirection(player.position.x - transform.position.x);
    }

    private void CacheReferences()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (attackCenter == null)
        {
            Transform attackCenterTransform = transform.Find("AttackCenter");
            if (attackCenterTransform != null)
            {
                attackCenter = attackCenterTransform;
            }
        }

        if (groundCheck == null)
        {
            Transform groundCheckTransform = transform.Find("GroundCheck");
            if (groundCheckTransform != null)
            {
                groundCheck = groundCheckTransform;
            }
        }
    }

    private void SetHorizontalDirection(float direction)
    {
        if (Mathf.Abs(direction) <= movementDeadZone)
        {
            StopMoving();
            return;
        }

        desiredHorizontalSpeed = Mathf.Sign(direction) * moveSpeed;
    }

    private void FaceDirection(float direction)
    {
        if (Mathf.Abs(direction) <= movementDeadZone)
        {
            return;
        }

        movingRight = direction > 0f;
        ApplyFacing(movingRight);
    }

    private bool HasGroundAhead()
    {
        if (groundCheck == null)
        {
            return true;
        }

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
        return groundInfo.collider != null;
    }

    private void ApplyHorizontalVelocity()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(desiredHorizontalSpeed, rb.linearVelocity.y);
    }

    private void ApplyFacing(bool shouldFaceRight)
    {
        Vector3 scale = transform.localScale;
        float scaleMagnitude = Mathf.Abs(scale.x);

        // Neu sprite goc quay sang trai thi scale duong se la quay trai, can dao logic lai.
        bool usePositiveScale = spriteFacesRightByDefault ? shouldFaceRight : !shouldFaceRight;
        scale.x = usePositiveScale ? scaleMagnitude : -scaleMagnitude;
        transform.localScale = scale;
    }

    private void SyncLogicalFacingFromScale()
    {
        bool hasPositiveScale = transform.localScale.x >= 0f;
        movingRight = spriteFacesRightByDefault ? hasPositiveScale : !hasPositiveScale;
    }

    private Vector3 GetChestSpawnPosition(GameObject spawnedChest)
    {
        float bossFeetY = transform.position.y;
        Collider2D bossCollider = GetComponent<Collider2D>();

        if (bossCollider != null)
        {
            bossFeetY = bossCollider.bounds.min.y;
        }
        else if (groundCheck != null)
        {
            bossFeetY = groundCheck.position.y;
        }

        float chestHalfHeight = 0f;
        if (spawnedChest != null)
        {
            SpriteRenderer chestRenderer = spawnedChest.GetComponent<SpriteRenderer>();
            if (chestRenderer != null && chestRenderer.sprite != null)
            {
                chestHalfHeight = chestRenderer.sprite.bounds.extents.y * Mathf.Abs(spawnedChest.transform.lossyScale.y);
            }
            else
            {
                Collider2D chestCollider = spawnedChest.GetComponent<Collider2D>();
                if (chestCollider != null)
                {
                    chestHalfHeight = chestCollider.bounds.extents.y;
                }
            }
        }

        return new Vector3(transform.position.x, bossFeetY + chestHalfHeight, transform.position.z);
    }

    private Vector3 GetSpellSpawnPosition()
    {
        if (player == null)
        {
            return transform.position;
        }

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        if (Mathf.Abs(directionToPlayer) <= movementDeadZone)
        {
            directionToPlayer = movingRight ? 1f : -1f;
        }

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float spawnDistance = Mathf.Clamp(distanceToPlayer, spellMinSpawnDistance, spellMaxSpawnDistance);

        return new Vector3(
            transform.position.x + directionToPlayer * spawnDistance,
            player.position.y + spellVerticalOffset,
            transform.position.z);

        // return new Vector3(
        //     transform.position.x + directionToPlayer * spellSpawnForwardOffset,
        //     transform.position.y + spellVerticalOffset,
        //     transform.position.z);
    }

    private Vector3 GetSpellTargetPosition()
    {
        if (player == null)
        {
            return transform.position;
        }

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float clampedDistance = Mathf.Clamp(distanceToPlayer, spellMinSpawnDistance, spellMaxSpawnDistance);
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (Mathf.Abs(directionToPlayer) <= movementDeadZone)
        {
            directionToPlayer = movingRight ? 1f : -1f;
        }

        float targetX = transform.position.x + directionToPlayer * clampedDistance;
        float targetY = player.position.y + spellVerticalOffset;

        return new Vector3(targetX, targetY, transform.position.z);
    }

    private float GetSpellAreaRadiusForGizmo()
    {
        if (spellAreaPrefab != null)
        {
            CircleCollider2D prefabCollider = spellAreaPrefab.GetComponent<CircleCollider2D>();
            if (prefabCollider != null)
            {
                float scaleMagnitude = Mathf.Max(
                    Mathf.Abs(spellAreaPrefab.transform.localScale.x),
                    Mathf.Abs(spellAreaPrefab.transform.localScale.y));

                if (Mathf.Approximately(scaleMagnitude, 0f))
                {
                    scaleMagnitude = 1f;
                }

                return prefabCollider.radius * scaleMagnitude;
            }
        }

        return spellRadius;
    }

    private void CreateSpellArea(Vector3 spellSpawnPosition)
    {
        GameObject spellArea;
        bool spawnedFromPrefab = spellAreaPrefab != null;

        if (spawnedFromPrefab)
        {
            spellArea = Instantiate(spellAreaPrefab, spellSpawnPosition, Quaternion.identity);
        }
        else
        {
            spellArea = new GameObject("Bringer Of Death Spell Area");
            spellArea.transform.position = spellSpawnPosition;

            CircleCollider2D fallbackCollider = spellArea.AddComponent<CircleCollider2D>();
            fallbackCollider.isTrigger = true;
            fallbackCollider.radius = spellRadius;

            spellArea.AddComponent<BringerOfDeathSpellArea>();
        }

        spellArea.transform.position = spellSpawnPosition;

        float directionToPlayer = player != null ? Mathf.Sign(player.position.x - transform.position.x) : 0f;
        if (Mathf.Abs(directionToPlayer) <= movementDeadZone)
        {
            directionToPlayer = movingRight ? 1f : -1f;
        }

        Vector3 spellScale = spellArea.transform.localScale;
        float spellScaleX = Mathf.Approximately(spellScale.x, 0f) ? 1f : Mathf.Abs(spellScale.x);
        float spellScaleY = Mathf.Approximately(spellScale.y, 0f) ? 1f : Mathf.Abs(spellScale.y);
        float spellScaleZ = Mathf.Approximately(spellScale.z, 0f) ? 1f : spellScale.z;
        bool shouldFaceRight = directionToPlayer > 0f;
        bool usePositiveScale = spriteFacesRightByDefault ? shouldFaceRight : !shouldFaceRight;

        spellScale.x = usePositiveScale ? spellScaleX : -spellScaleX;
        spellScale.y = spellScaleY;
        spellScale.z = spellScaleZ;
        spellArea.transform.localScale = spellScale;

        CircleCollider2D spellCollider = spellArea.GetComponent<CircleCollider2D>();
        if (spellCollider == null)
        {
            spellCollider = spellArea.AddComponent<CircleCollider2D>();
            spellCollider.radius = spellRadius;
        }
        spellCollider.isTrigger = true;

        SpriteRenderer spellRenderer = spellArea.GetComponent<SpriteRenderer>();
        bool addedRenderer = false;
        if (spellRenderer == null)
        {
            spellRenderer = spellArea.AddComponent<SpriteRenderer>();
            addedRenderer = true;
        }

        SpriteRenderer bossRenderer = GetComponent<SpriteRenderer>();
        if (bossRenderer != null && addedRenderer)
        {
            spellRenderer.sortingLayerID = bossRenderer.sortingLayerID;
            spellRenderer.sortingOrder = bossRenderer.sortingOrder + spellSortingOrderOffset;
            spellRenderer.sharedMaterial = bossRenderer.sharedMaterial;
        }

        Animator spellAnimator = spellArea.GetComponent<Animator>();
        if (spellAnimator == null)
        {
            spellAnimator = spellArea.AddComponent<Animator>();
        }

        if (spellAnimator.runtimeAnimatorController == null && animator != null)
        {
            spellAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
        }

        if (spellAnimator.runtimeAnimatorController != null && !string.IsNullOrEmpty(spellAnimationName))
        {
            spellAnimator.Play(spellAnimationName, -1, 0f);
        }

        BringerOfDeathSpellArea spellAreaComponent = spellArea.GetComponent<BringerOfDeathSpellArea>();
        if (spellAreaComponent == null)
        {
            spellAreaComponent = spellArea.AddComponent<BringerOfDeathSpellArea>();
        }

        spellAreaComponent.Initialize(spellDamage, playerLayer, spellLifetime);
    }

    private void CreateSpellVisual(Vector3 spellSpawnPosition)
    {
        if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(spellAnimationName))
        {
            Debug.LogWarning("Bringer Of Death: Missing animator controller or spell animation.", this);
            return;
        }

        GameObject spellEffect = new GameObject("Bringer Of Death Spell");
        spellEffect.transform.position = spellSpawnPosition;
        spellEffect.transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), 1f);

        SpriteRenderer spellRenderer = spellEffect.AddComponent<SpriteRenderer>();
        SpriteRenderer bossRenderer = GetComponent<SpriteRenderer>();
        if (bossRenderer != null)
        {
            spellRenderer.sortingLayerID = bossRenderer.sortingLayerID;
            spellRenderer.sortingOrder = bossRenderer.sortingOrder + spellSortingOrderOffset;
            spellRenderer.sharedMaterial = bossRenderer.sharedMaterial;
        }

        Animator spellAnimator = spellEffect.AddComponent<Animator>();
        spellAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
        spellAnimator.Play(spellAnimationName, -1, 0f);

        Destroy(spellEffect, spellLifetime);
    }

    private void CreateSpellProjectile(Vector3 spellSpawnPosition, Vector3 spellTargetPosition)
    {
        if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(spellAnimationName))
        {
            Debug.LogWarning("Bringer Of Death: Missing animator controller or spell animation.", this);
            return;
        }

        GameObject spellProjectile = new GameObject("Bringer Of Death Spell Projectile");
        spellProjectile.transform.position = spellSpawnPosition;

        Vector3 projectileScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), 1f);
        if (spellTargetPosition.x < spellSpawnPosition.x)
        {
            projectileScale.x = -Mathf.Abs(projectileScale.x);
        }
        else
        {
            projectileScale.x = Mathf.Abs(projectileScale.x);
        }
        spellProjectile.transform.localScale = projectileScale;

        SpriteRenderer spellRenderer = spellProjectile.AddComponent<SpriteRenderer>();
        SpriteRenderer bossRenderer = GetComponent<SpriteRenderer>();
        if (bossRenderer != null)
        {
            spellRenderer.sortingLayerID = bossRenderer.sortingLayerID;
            spellRenderer.sortingOrder = bossRenderer.sortingOrder + spellSortingOrderOffset;
            spellRenderer.sharedMaterial = bossRenderer.sharedMaterial;
        }

        Animator spellAnimator = spellProjectile.AddComponent<Animator>();
        spellAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
        spellAnimator.Play(spellAnimationName, -1, 0f);

        BringerOfDeathSpellProjectile projectile = spellProjectile.AddComponent<BringerOfDeathSpellProjectile>();
        projectile.Initialize(
            spellTargetPosition,
            spellProjectileSpeed,
            spellRadius,
            spellDamage,
            playerLayer,
            spellLifetime);
    }

    private void DealSpellDamage(Vector3 spellSpawnPosition)
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(spellSpawnPosition, spellRadius, playerLayer);

        for (int i = 0; i < hitPlayers.Length; i++)
        {
            Player playerComponent = hitPlayers[i].GetComponent<Player>();
            if (playerComponent == null)
            {
                playerComponent = hitPlayers[i].GetComponentInParent<Player>();
            }

            if (playerComponent != null)
            {
                playerComponent.TakeDamage(spellDamage);
                break;
            }
        }
    }
}

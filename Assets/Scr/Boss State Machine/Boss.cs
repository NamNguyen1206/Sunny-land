using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float chaseRange = 6f;

    public int maxHealth = 100;
    public int currentHealth;

    public Animator animator;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float checkDistance = 0.2f;
    public LayerMask groundLayer;
    private bool movingRight = true;

    [HideInInspector] public BossStateMachine stateMachine;

    // States
    public IdleState idleState;
    public ChaseState chaseState;
    public AttackState attackState;
    public DeathState deathState;
    public TakeHitState takeHitState;
    public PatrolState patrolState;

    void Start()
    {
        currentHealth = maxHealth;

        stateMachine = new BossStateMachine();

        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        deathState = new DeathState(this);
        takeHitState = new TakeHitState(this);
        patrolState = new PatrolState(this);

        stateMachine.ChangeState(patrolState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            stateMachine.ChangeState(deathState);
        }
        else
        {
            stateMachine.ChangeState(takeHitState);
        }
    }
    public void Patrol()
    {
    // Di chuyển
    float direction = movingRight ? 1 : -1;
    rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

    // Check ground phía trước
    RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);

    // Check tường phía trước
    //RaycastHit2D wallInfo = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, checkDistance, groundLayer);

    if (!groundInfo.collider)
        {
            Flip();
        }
    }
    void Flip()
    {
    movingRight = !movingRight;

    Vector3 scale = transform.localScale;
    scale.x *= -1;
    transform.localScale = scale;
    }

    public float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    public void MoveToPlayer()
    {
        Debug.Log("Move to player");
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
        // transform.position += moveSpeed * Time.deltaTime * new Vector3(dir.x, 0, 0);

    // // Flip
    if (dir.x > 0)
        transform.localScale = new Vector3(1, 1, 1);
    else
        transform.localScale = new Vector3(-1, 1, 1);
    }
    void OnDrawGizmos()
{
    if (groundCheck != null)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * checkDistance);
    }
}
}

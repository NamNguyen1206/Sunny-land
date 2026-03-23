using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health & Status")]
    public int maxHealth = 50;
    private int currentHealth; // Dùng biến riêng thay vì trừ trực tiếp vào maxHealth
    private bool isDead = false;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3f;
    private bool facingLeft = true;
    private Vector3 initialPosition;
    private bool initialFacingLeft;

    [Header("Detection & Combat")]
    public Transform player;
    public float attackRange = 10f; // Tầm phát hiện
    public float retreatRange = 2f; // Tầm dừng lại để đánh (nên nhỏ thôi)
    public LayerMask layerMask; // Layer mặt đất
    public Transform checkPoint; // Để check vực thẳm
    public float distance = 1f;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask AttackLayer;
    public int damage = 10;

    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
        initialFacingLeft = facingLeft;

        // Tự động tìm Player nếu chưa kéo thả vào Inspector
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // 1. Kiểm tra trạng thái Game hoặc Cái chết
        if (isDead || !FindObjectOfType<GameManager>().isGameActive) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. Logic Đuổi theo hoặc Tuần tra
        if (distToPlayer <= attackRange)
        {
            HandleChasing(distToPlayer);
        }
        else
        {
            HandlePatrol();
        }
    }

    void HandleChasing(float distToPlayer)
    {
        // Quay mặt về phía Player
        if (player.position.x > transform.position.x && facingLeft)
        {
            Flip(false);
        }
        else if (player.position.x < transform.position.x && !facingLeft)
        {
            Flip(true);
        }

        // Di chuyển hoặc Tấn công
        if (distToPlayer > retreatRange)
        {
            animator.SetBool("Attack1", false);
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("Attack1", true);
        }
    }

    void HandlePatrol()
    {
        animator.SetBool("Attack1", false);
        float moveDir = facingLeft ? -1 : 1;
        transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

        // Check vực thẳm/tường để quay đầu
        RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);
        if (hit.collider == null)
        {
            Flip(!facingLeft);
        }
    }

    void Flip(bool left)
    {
        facingLeft = left;
        float angle = left ? 0f : 180f;
        transform.eulerAngles = new Vector3(0f, angle, 0f);
    }

    // Hàm này gọi từ Animation Event
    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, AttackLayer);
        if (collInfo != null)
        {
            Player p = collInfo.GetComponent<Player>();
            if (p != null)
            {
                // Khi đánh, Player sẽ tự kiểm tra xem có đang Parry không
                p.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        // Tìm Player và cộng điểm diệt địch
        Player p = FindObjectOfType<Player>();
        if (p != null) p.AddSkeletonKill();
        // 2. Chạy Animation chết
        animator.SetBool("Attack1", false);
        animator.SetBool("Death", true);
        // 3. XỬ LÝ VẬT LÝ ĐỂ KHÔNG RƠI XUYÊN ĐẤT
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
        rb.linearVelocity = Vector2.zero; // Dừng mọi chuyển động ngay lập tức
        rb.bodyType = RigidbodyType2D.Static; // Chuyển sang Static để không chịu trọng lực và không bị đẩy
        }

        // 4. Tắt Collider để Player có thể đi xuyên qua xác quái (nhưng không bị rơi vì đã là Static)
        if (GetComponent<Collider2D>()) 
        {
            GetComponent<Collider2D>().enabled = false;
        }
        // 5. Hủy Object sau một khoảng thời gian
        //StartCoroutine(DestroyAfterDeath());
        Object.Destroy(gameObject, 3f);
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }

    public void Reset()
    {
        isDead = false;
        currentHealth = maxHealth;
        transform.position = initialPosition;
        Flip(initialFacingLeft);
        // Trả lại Rigidbody về Dynamic để quái có thể di chuyển và rơi lại
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        if(GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = true;
        animator.SetBool("Death", false);
        animator.SetBool("Attack1", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
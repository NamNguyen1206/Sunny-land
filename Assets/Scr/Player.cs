using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Cần thiết cho Coroutine

public class Player : MonoBehaviour
{
    public int skeletonsKilled = 0;
    public int comboStage = 0; // 0: Chỉ Attack 1, 1: Mở Attack 2, 2: Mở Attack 3
    private int comboStep = 0; // 0: Attack1, 1: Attack2, 2: Attack3
    public GameObject player;
    public Transform spawnPosistion;
    public GameObject explosionEffect;
    public Transform explosionSpawnPoint;
    public Text coinText;
    public int currentCoins = 0;
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;
    public Rigidbody2D rb;
    public Animator animator;
    public float JumpHeight = 5f;
    public bool isGrounded = true;
    private float movement;
    public float moveSpeed = 5f;
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask attackLayers;

    private bool facingRight = true;
    private float lastClickTime;
    public float comboDelay = 0.5f; // Thời gian tối đa giữa 2 lần bấm để tính combo

    // --- PARRY SYSTEM [NEW] ---
    [Header("Parry Settings")]
    public bool isParrying = false; // Trạng thái đang đỡ
    public float parryWindowDuration = 0.2f; // Thời gian "vàng" để Parry thành công (giây)
    public float blockDuration = 0.5f; // Tổng thời gian giơ khiên/kiếm lên đỡ (giây)
    private Coroutine parryCoroutine; // Lưu Coroutine để có thể hủy nếu cần

    void Start()
    {
        currentHealth = maxHealth;
        RefreshUI();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("Player is missing a Health Slider reference.", this);
        }

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
        }
        else
        {
            Debug.LogWarning("Player is missing an Animator reference.", this);
        }

        GameManager gameManager = GameManager.instance;
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (gameManager != null && gameManager.currentCheckpoint == null && spawnPosistion != null)
        {
            gameManager.SetCheckpoint(spawnPosistion);
        }
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        RefreshUI();

        // Không cho di chuyển/nhảy/tấn công khi đang đỡ
        if (isParrying)
        {
            movement = 0f;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Dừng di chuyển ngang
        }
        else
        {
            movement = Input.GetAxis("Horizontal");
        }

        // --- FLIP LOGIC ---
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        // --- JUMP LOGIC ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isParrying) // [MODIFIED] Thêm !isParrying
        {
            Jump();
            isGrounded = false;
        }

        // --- ANIMATION STATES ---
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(movement));
            animator.SetBool("isGrounded", isGrounded);
        }

        // --- ATTACK LOGIC (Mouse 0 - Chuột trái) ---
        if (Input.GetMouseButtonDown(0) && !isParrying) // [MODIFIED] Thêm !isParrying
        {
            HandleAttack();
        }

        // --- PARRY/BLOCK LOGIC (Mouse 1 - Chuột phải) [NEW] ---
        if (Input.GetMouseButtonDown(1) && isGrounded && !isParrying)
        {
            if (parryCoroutine != null) StopCoroutine(parryCoroutine);
            parryCoroutine = StartCoroutine(ParryRoutine());
        }
    }
    void HandleAttack()
    {
        if (animator == null) return;

    float timeSinceLastClick = Time.time - lastClickTime;

    // Nếu người chơi bấm quá chậm, reset chuỗi combo về đòn đầu tiên
    if (timeSinceLastClick > comboDelay)
    {
        comboStep = 0;
    }

    // Thực hiện đòn đánh dựa trên comboStep và comboStage đã mở khóa
    if (comboStep == 0)
    {
        animator.SetTrigger("Attack");
        comboStep = (comboStage >= 1) ? 1 : 0; // Nếu đã mở khóa stage 1, chuẩn bị cho đòn tiếp theo
    }
    else if (comboStep == 1 && comboStage >= 1)
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack2");
        comboStep = (comboStage >= 2) ? 2 : 0; // Nếu đã mở khóa stage 2, chuẩn bị cho đòn 3
    }
    else if (comboStep == 2 && comboStage >= 2)
    {
        animator.ResetTrigger("Attack2");
        animator.SetTrigger("Attack3");
        comboStep = 0; // Kết thúc chuỗi, quay về đòn 1
    }
    else
    {
        // Trường hợp mặc định nếu có lỗi hoặc chưa đủ stage
        animator.SetTrigger("Attack");
        comboStep = (comboStage >= 1) ? 1 : 0;
    }

    lastClickTime = Time.time;
    }
    // Hàm để Enemy gọi khi chết
    public void AddSkeletonKill()
    {
    skeletonsKilled++;
    Debug.Log("Skeletons diệt được: " + skeletonsKilled);

    if (skeletonsKilled >= 5) comboStage = 2;
    else if (skeletonsKilled >= 3) comboStage = 1;
    }

    // --- PARRY COROUTINE [NEW] ---
    IEnumerator ParryRoutine()
    {
        isParrying = true;

        if (animator != null)
        {
            // Bật animation Block. Đảm bảo Trigger "Block" đã được tạo trong Animator.
            animator.SetTrigger("Block");
            // Nếu dùng Bool, hãy thay bằng: animator.SetBool("isBlocking", true);
        }

        // Cửa sổ "vàng" để Parry thành công. Nếu Boss đánh trúng trong thời gian này, Parry tính là thành công.
        yield return new WaitForSeconds(parryWindowDuration);

        // Sau thời gian này, Player vẫn đang đỡ (animation vẫn chạy) nhưng không còn tính là Parry thành công nữa.
        // Bạn có thể cho Player nhận sát thương giảm đi trong thời gian này nếu muốn.
        
        // Đợi nốt thời gian còn lại của animation Block
        yield return new WaitForSeconds(blockDuration - parryWindowDuration);

        isParrying = false;
        
        // Nếu dùng Bool cho animation, hãy tắt nó ở đây:
        // if (animator != null) animator.SetBool("isBlocking", false);
    }

    private void FixedUpdate()
    {
        // Không di chuyển khi đang đỡ
        if (!isParrying)
        {
            transform.position += new Vector3(movement, 0, 0) * Time.fixedDeltaTime * moveSpeed;
        }
    }

    void Jump()
    {
        if (rb != null)
        {
            rb.AddForce(new Vector3(0f, JumpHeight, 0f), ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("Player is missing a Rigidbody2D reference.", this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Player is missing an Attack Point reference.", this);
            return;
        }

        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayers);
        if (collInfo)
        {
            Enemy e = collInfo.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(10);

            Boss b = collInfo.GetComponent<Boss>();
            if (b != null) b.TakeDamage(10);

            BringerOfDeathBoss bringerOfDeathBoss = collInfo.GetComponent<BringerOfDeathBoss>();
            if (bringerOfDeathBoss != null) bringerOfDeathBoss.TakeDamage(10);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    // --- TAKE DAMAGE LOGIC [MODIFIED] ---
    public void TakeDamage(int damage)
    {
        if (isParrying)
        {
            // Parry thành công!
            Debug.Log("PARRY THÀNH CÔNG! Không mất máu.");
            
            // Bạn có thể thêm hiệu ứng ở đây:
            // 1. Tiếng "Keng" (SFX)
            // 2. Nháy màu trắng (Sprite Flash)
            // 3. Rung màn hình nhẹ (Camera Shake)

            return; // Thoát hàm, không trừ máu
        }
        // --- KHÓA LỰC ĐẨY  ---
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Triệt tiêu 
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        RefreshUI();

        if (animator != null)
        {
            animator.SetTrigger("Hurt"); // Đảm bảo Trigger "Hurt" đã được tạo
        }

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            Debug.Log(other.transform.name);
            currentCoins++;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }
        if (other.gameObject.tag == "Hearth")
        {
            Debug.Log(other.transform.name);
            currentHealth += 50;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // [MODIFIED] Giới hạn máu
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }
        if (other.gameObject.tag == "VictoryPoint")
        {
            LevelManager.LoadLevel(currentCoins);
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Die()
    {
        Debug.Log("Player has died.");
        if (explosionEffect != null && explosionSpawnPoint != null)
        {
            GameObject tempExplosion = Instantiate(explosionEffect, explosionSpawnPoint.position, Quaternion.identity);
            Destroy(tempExplosion, 0.5f);
        }

        currentHealth = maxHealth;
        GameManager gameManager = GameManager.instance;
        if (gameManager != null)
        {
            // if (spawnPosistion != null)
            // {
            //     transform.position = spawnPosistion.position;
            // }

            if (gameManager.currentCheckpoint != null)
            {
                transform.position = gameManager.currentCheckpoint.position;
            }
            else if (spawnPosistion != null)
            {
                transform.position = spawnPosistion.position;
            }

            gameManager.ResetAllEnemies();
        }
        else if (spawnPosistion != null)
        {
            transform.position = spawnPosistion.position;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void RefreshUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}

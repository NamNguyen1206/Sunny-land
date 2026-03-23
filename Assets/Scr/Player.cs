using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
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
    }

    void Update()
    {
        if(currentHealth <= 0)
        {
            Die();
        }
        RefreshUI();
        
        movement = Input.GetAxis("Horizontal");

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

    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
        }
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(movement));
            animator.SetBool("isGrounded", isGrounded);
        }
    if (Input.GetMouseButtonDown(0))
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
    private void FixedUpdate()
        {
            transform.position += new Vector3(movement, 0, 0) * Time.fixedDeltaTime * moveSpeed;
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

        // Implement attack logic here (e.g., detect enemies in range, apply damage, etc.)
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayers); // Example: Detect enemies within a radius of 1 unit
        if (collInfo)
        {
            //if(collInfo.gameObject.GetComponent<Enemy>() != null)
            //{
            //collInfo.gameObject.GetComponent<Enemy>().TakeDamage(10); // Example: Apply 10 damage to the enemy
            //}
            
            // 1. Đánh Quái thường
            Enemy e = collInfo.GetComponent<Enemy>();
            if (e != null) e.TakeDamage(10);
            // 2. Đánh Boss (Chỉ thêm dòng này)
            Boss b = collInfo.GetComponent<Boss>();
            if (b != null) b.TakeDamage(10);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        RefreshUI();
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
        if(other.gameObject.tag == "Hearth")
        {
            Debug.Log(other.transform.name);
            currentHealth += 10;
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Collected");
            Destroy(other.gameObject, 1f);
        }
        if(other.gameObject.tag == "VictoryPoint")
        {
            FindObjectOfType<SceneManagement>().LoadLevel();
        }
    }
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Die()
    {
        {
        // Implement death logic here (e.g., play animation, disable player controls, etc.)
        Debug.Log("Player has died.");
        //FindObjectOfType<GameManager>().isGameActive = false;
        if (explosionEffect != null && explosionSpawnPoint != null)
        {
            GameObject tempExplosion = Instantiate(explosionEffect, explosionSpawnPoint.position, Quaternion.identity);
            Destroy(tempExplosion, 0.5f);
        }
        
        //Instantiate(player, spawnPosistion.position, Quaternion.identity);
        currentHealth = maxHealth;
        if (spawnPosistion != null)
        {
            transform.position = spawnPosistion.position;
        }
        
        // Reset all enemies when player dies
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ResetAllEnemies();
        }
        
        //FindObjectOfType<GameManager>().isGameActive = true;
        //Destroy(this.gameObject);
        //Invoke("RestartGame", 1f);
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

    

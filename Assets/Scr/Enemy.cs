using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    private int initialHealth;
    private bool facingLeft = true;
    private bool initialFacingLeft = true;
    private Vector3 initialPosition;
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    private bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retreatRange = 5f;
    public float cheseSpeed = 3f;
    public Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask AttackLayer;
    bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialHealth = maxHealth;
        initialPosition = transform.position;
        initialFacingLeft = facingLeft;
    }

    // Update is called once per frame
    void Update()
    {
        if(FindObjectOfType<GameManager>().isGameActive == false)
        {
            return;
        }
        if(maxHealth <= 0)
        {
            Die();
            return;
        }
        if(Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
        if (inRange)
        {
            if (player.position.x > transform.position.x && facingLeft == true)
            {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingLeft = false;
            }
            else if (player.position.x < transform.position.x && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingLeft = true;
            }
            if (Vector2.Distance(transform.position, player.position) > retreatRange)
            {
                animator.SetBool("Attack1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, cheseSpeed * Time.deltaTime);
            }
            else 
            {
                animator.SetBool("Attack1", true);
            }    
        }
        else
        {
            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);
            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);
        if(hit == false && facingLeft)
            {
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
                facingLeft = false;
            }
        else if(hit == false && facingLeft == false)
            {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingLeft = true;
            }
        }
    }
    public void Attack()
    {
        if (FindObjectOfType<GameManager>().isGameActive == false)
        return;

        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, AttackLayer);

    if (collInfo != null)
    {
        Player playerScript = collInfo.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.TakeDamage(10);
        }
    }
    }
    public void TakeDamage(int damage)
    {
        maxHealth -= damage;
        animator.SetTrigger("Hurt");
        if (maxHealth <= 0)
        {
            Die();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPoint.position, attackRange);

        if(attackPoint == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
    void Die()
    {
        animator.SetBool("Death", true);
        StartCoroutine(DestroyAfterDeath());
    }
    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }
    public void Reset()
    {
        maxHealth = initialHealth;
        transform.position = initialPosition;
        facingLeft = initialFacingLeft;
        if (facingLeft)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        animator.SetBool("Attack1", false);
        inRange = false;
    }
}

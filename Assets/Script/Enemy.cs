using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable
{
    [Header("Enemy Settings")]
    [SerializeField] protected string ENEMY_HIT = "enemy_hit";
    [SerializeField] protected string ENEMY_IDLE = "enemy_idle";
    [SerializeField] protected string ENEMY_WALK = "enemy_walk";
    protected float moveSpeed = 2f;
    [SerializeField] protected float hitDuration = 2f; // Durasi hit (misalnya 2 detik)
    [SerializeField] private float knockbackForce = 5f;

    private Transform player;
    protected Rigidbody2D rb;
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool facingRight;
    public bool isGrounded;
    public float circleRadius;

    public string currentAnimaton;
    protected float hitTimer = 0f;
    protected Animator animator;

    protected bool isHit = false;
    private float distanceFromPlayer;
    private float moveEnemyPos;
    [SerializeField] protected float lineofSite = 4f;
    [SerializeField] private float stopDistance = 2f;
    private bool isPlayerInRange = false;

    // TODO : make enemy jump and attack
    [Header("Enemy Jump")]
    [SerializeField] float jumpHeight;


    [Header("Enemy Health")]
    [SerializeField] private int health;
    [SerializeField] private int MAX_HEALTH;
    public HealthBarBehavior healthBar;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    
    [Header("References")]
    public LayerMask playerLayer;
    
    private bool canAttack = true;
    [SerializeField] private Health playerHealth;


    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        health = MAX_HEALTH;
        // playerHealth = player.GetComponent<Health>();
        healthBar.SetHealth(health, MAX_HEALTH);

        // GameObject player = GameObject.FindWithTag("Player");
    }


    protected virtual void Update()
    {
        if(player == null) return;
        distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        isPlayerInRange = distanceFromPlayer <= attackRange;
        // Patrol(); // default behavior
        // rb.linearVelocity = Vector2.right * moveSpeed * Time.deltaTime;
        moveEnemyPos = player.position.x - this.transform.position.x;

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, circleRadius, groundLayer);

        FlipSprite();

        if(isPlayerInRange && canAttack)
        {
            StartCoroutine(Attack());
        }
        // play animation idle
        // if (!isHit)
        // {
        //     ChangeAnimationState(ENEMY_IDLE);
        // }
    }
    protected virtual void FixedUpdate()
    {
        if (distanceFromPlayer < lineofSite && distanceFromPlayer > stopDistance)
        {
            isPlayerInRange = true;
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        }
        else
        {
            isPlayerInRange = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }


        if (!isHit)
        {
            if ((moveEnemyPos <= -0.1f || moveEnemyPos >= 0.1f) && isGrounded && isPlayerInRange)
            {
                ChangeAnimationState(ENEMY_WALK);
            }
            else
            {
                ChangeAnimationState(ENEMY_IDLE);
            }
        }

        

    }
    void FlipSprite()
    {
        if (facingRight && (player.position.x - this.transform.position.x) > 0f || !facingRight && (player.position.x - this.transform.position.x) < 0f)
        {

            facingRight = !facingRight;

            Vector3 ls = transform.localScale;

            ls.x *= -1f;

            transform.localScale = ls;
        }




    }
    
    
    // Visualisasi attack range di Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // FIXME
    protected virtual void Patrol()
    {
        // contoh sederhana
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {

        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        health -= amount;
        healthBar.SetHealth(health, MAX_HEALTH);

        // IHittable hittable = GetComponent<IHittable>();
        // if (hittable != null)
        // {
        //     hittable.OnHit(hitDirection);
        // }
        OnHit(hitDirection);
        if (health <= 0)
        {
            Die();
        }
        // if (damagePopupPrefab != null)
        // {
        //     GameObject popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        //     popup.GetComponentInChildren<DamagePopup>().Setup(amount);
        // }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        
        Debug.Log("attack player");
        // Logika serangan
        if(playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked player! Remaining health: " + playerHealth.currentHealth);
        }
        
        // Cooldown serangan
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public virtual void OnHit(Vector2 hitDirection)
    {
        isHit = true;
        ChangeAnimationState(ENEMY_HIT);

        if (rb != null)
        {
            rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
   
    public virtual void Respawn(Vector2 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Die()
    {
        Debug.Log("i am dead!");
        Destroy(gameObject);
    }

    void ChangeAnimationState(string newAnimation, bool forcePlay = false)
    {
        if (!forcePlay && currentAnimaton == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player keluar dari trigger, musuh tidak lagi terkena hit
            isHit = false;
        }
    }
}


using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    
    private Rigidbody2D rb;
    public bool isGrounded= false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public Transform groundCheck; // Assign di Unity Editor
    private bool isTouchingWall;
    private int wallDirection = 0; // -1 jika menyentuh kiri, 1 jika menyentuh kanan, 0 jika tidak menyentuh
    //float moveInput = 0f;
    public float horizontalInput;
    bool isFacingRight = false;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isTapAttack = false;
    public float attackDuration = 0.2f; // Durasi serangan saat ditekan sekali
    public float attackHoldDuration = 0.5f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ambil komponen Animator
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        //if (Input.GetKey(KeyCode.D) ) // Cegah gerakan hanya ke kanan jika menyentuh dinding kanan
        //{
        //    moveInput = 1f;
        //    spriteRenderer.flipX = false;
        //}
        //else if (Input.GetKey(KeyCode.A)) // Cegah gerakan hanya ke kiri jika menyentuh dinding kiri
        //{
        //    moveInput = -1f;
        //    spriteRenderer.flipX = true;
        //}
        
        
        FlipSprite();

        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, LayerMask.GetMask("Ground"));
       

        // Lompat jika tombol space ditekan dan karakter di tanah
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);


        }
        if (Input.GetKeyDown(KeyCode.J)) // Jika tombol ditekan sekali
        {
            isAttacking = true;
            animator.SetBool("IsAttack", true);
            attackTimer = attackDuration; // Timer untuk attack sekali
            Debug.Log("1 attack");
            isTapAttack = true;

        }

        if (Input.GetKey(KeyCode.J) && !isTapAttack) // Jika tombol terus ditekan
        {
            Debug.Log("hold attack");

            attackTimer = attackHoldDuration; // Reset timer agar animasi bertahan lebih lama
        }

        if (isAttacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            //attackTimer -= Time.deltaTime;
            //Debug.Log(attackTimer);
            else
            {
                isAttacking = false;
                animator.SetBool("IsAttack", false);
            }
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        if (horizontalInput >= 0.1f || horizontalInput <= -0.1f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
      
        //animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x)); // Menggunakan absolut agar tidak negatif
        //animator.SetFloat("yVelocity", rb.linearVelocity.y);

    }

    void FlipSprite()
    {
        if (isFacingRight && horizontalInput > 0f || !isFacingRight && horizontalInput < 0f)
        {

            isFacingRight = !isFacingRight;

            Vector3 ls = transform.localScale;

            ls.x *= -1f;

            transform.localScale = ls;
        }
    }


    void Attack()
    {
        isAttacking = true;
        animator.SetBool("IsAttack", true);

        // Mematikan animasi setelah durasi tertentu
        Invoke("ResetAttack", attackDuration);
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttack", false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
              
                if (Mathf.Abs(contact.normal.x) > 0.5f) // Jika normal X lebih dari 0.5, berarti menyentuh dinding vertikal
                {
                    isTouchingWall = true;
                    wallDirection = contact.normal.x > 0 ? -1 : 1; // 1 jika dinding di kanan, -1 jika di kiri
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true; 
        animator.SetBool("IsJumping", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;
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
    public int jumpCount = 1;
    private int maxJumps = 2;
    public bool isJumpPressed;
    public bool isJumping = false;
    public bool isGrounded = false;


    public string currentAnimaton;

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_JUMP = "Player_jump";
    const string PLAYER_ATTACK = "Player_attack";
    const string PLAYER_AIR_ATTACK = "Player_air_attack";
    const string PLAYER_RUN_ATTACK = "Player_run_attack";
    const string PLAYER_FALLING = "Player_fall";


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ambil komponen Animator
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
            jumpCount = 1;
            isJumping = false; // Matikan flag lompat saat menyentuh tanah
//            isJumpPressed =false;

        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            isJumpPressed = true;
            isJumping = true; // Set flag lompat
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;

             // Set animasi jump langsung saat tombol ditekan
            // ChangeAnimationState(PLAYER_JUMP, true);
            animator.Play(PLAYER_JUMP, 0, 0.0f);
            // animator.speed = 0;
            // return;
            // Debug.Log("jump");
        }

        // // === Attack logic tetap seperti sebelumnya ===
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     isAttacking = true;
        //     animator.SetBool("IsAttack", true);
        //     attackTimer = attackDuration;
        //     isTapAttack = true;
        // }

        // if (Input.GetKey(KeyCode.J) && !isTapAttack)
        // {
        //     attackTimer = attackHoldDuration;
        // }

        // if (isAttacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
                animator.SetBool("IsAttack", false);
                isTapAttack = false; // Reset agar bisa deteksi hold lagi
            }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Prioritaskan animasi jump selama sedang melompat
        if (isJumping || isJumpPressed)
        {
            // Jika masih di udara, tetap animasi lompat
            if (!isGrounded)
            {
                ChangeAnimationState(PLAYER_JUMP);
                return;
            }
            else if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.1f) // sudah benar-benar mendarat
            {
                isJumping = false; // Selesai lompat
                isJumpPressed = false;
            }
        }

        // Urutan setelah selesai lompat:
        if ((horizontalInput >= 0.1f || horizontalInput <= -0.1f) && isGrounded && isJumping == false)
        {
            ChangeAnimationState(PLAYER_RUN);
        }
        else if (isGrounded && isJumping == false && isJumpPressed == false)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

        // Debug.Log(rb.linearVelocity.y);
        //player fall animation
        if(!isJumping && !isJumpPressed && !isGrounded && rb.linearVelocity.y < 0.1f)
        {

            ChangeAnimationState(PLAYER_FALLING);
        }
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

    void ChangeAnimationState(string newAnimation, bool forcePlay = false)
    {
        if (!forcePlay && currentAnimaton == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
    // void Attack()
    // {
    //     isAttacking = true;
    //     animator.SetBool("IsAttack", true);

    //     // Mematikan animasi setelah durasi tertentu
    //     Invoke("ResetAttack", attackDuration);
    // }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttack", false);
    }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         foreach (ContactPoint2D contact in collision.contacts)
    //         {

    //             if (Mathf.Abs(contact.normal.x) > 0.5f) // Jika normal X lebih dari 0.5, berarti menyentuh dinding vertikal
    //             {
    //                 isTouchingWall = true;
    //                 wallDirection = contact.normal.x > 0 ? -1 : 1; // 1 jika dinding di kanan, -1 jika di kiri
    //                 return;
    //             }
    //         }
    //     }
    // }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }


    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     isGrounded = true;
    //     animator.SetBool("IsJumping", false);
    // }
}

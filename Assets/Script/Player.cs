using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isTapAttack = false;

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

    [Header("For Attack")]
    public GameObject attackArea;
    private bool isAttackQueued = false;
    [SerializeField] private float attackCooldown = 0.9f; // waktu minimal antar spam

    private float lastAttackTime = -999f;
    int index_anim_attack = 1;
    private bool isAttacking = false;
    public float attackDuration = 0.2f; // Durasi serangan saat ditekan sekali
    public float attackHoldDuration = 0.5f;
    public float timeToAttack = 0.3f;

    [Header("Stats")]
    
    public float maxHealth = 100f;
    public float maxMana = 50;
    public float maxStamina = 75;

    private float currentHealth;
    private float currentMana;
    private float currentStamina;

 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Ambil komponen Animator
        spriteRenderer = GetComponent<SpriteRenderer>();
        // attackArea = transform.GetChild(1).gameObject;
        attackArea.SetActive(false);

        
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

        }

     

        if (Input.GetKeyDown(KeyCode.J) && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(HandleAttack());
        }

        

    }
    IEnumerator HandleAttack()
    {
        isAttackQueued = false;
        isAttacking = true;

        string baseAttackAnim = "";

        if (!isGrounded)
        {
            baseAttackAnim = PLAYER_AIR_ATTACK;
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            baseAttackAnim = PLAYER_RUN_ATTACK;
        }
        else
        {
            baseAttackAnim = PLAYER_ATTACK;
        }

        string nextAttackAnim = baseAttackAnim + "_" + index_anim_attack;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        attackArea.SetActive(true);

        if (!stateInfo.IsName(nextAttackAnim))
        {
            ChangeAnimationState(nextAttackAnim, true);
            yield return new WaitForSeconds(timeToAttack);
            attackArea.SetActive(false); // Matikan area serangan

            // Handle attack queue (combo)
            if (isAttackQueued)
            {
                isAttackQueued = false;
                index_anim_attack = (index_anim_attack % 2) + 1; // 1→2→1→2...
                StartCoroutine(HandleAttack()); // Lanjut ke serangan berikutnya
            }
            else
            {
                isAttacking = false;
                index_anim_attack = 1; // Reset ke serangan pertama
            }
        }
        else{
            isAttackQueued = true;

        }
        

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Prioritaskan animasi jump selama sedang melompat
        if (isJumping || isJumpPressed)
        {
            // Jika masih di udara, tetap animasi lompat
            if (!isGrounded && !isAttacking)
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
        if (!isAttacking && !isAttackQueued)
        {

            // Urutan setelah selesai lompat:
            if ((horizontalInput >= 0.1f || horizontalInput <= -0.1f) && isGrounded && isJumping == false)
            {
                ChangeAnimationState(PLAYER_RUN);
            }
            else if (isGrounded && !isJumping && !isJumpPressed && !isAttacking && !isAttackQueued)
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }


        //player fall animation
        if (!isJumping && !isJumpPressed && !isGrounded && rb.linearVelocity.y < 0.1f)
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


    // void ResetAttack()
    // {
    //     attackArea.SetActive(false);

    //     if (isAttackQueued)
    //     {
    //         isAttackQueued = false;
    //         index_anim_attack++;
    //         if (index_anim_attack > 2) index_anim_attack = 1;

    //         string baseAttackAnim = "";

    //         if (!isGrounded)
    //         {
    //             baseAttackAnim = PLAYER_AIR_ATTACK;
    //         }
    //         else if (Mathf.Abs(horizontalInput) > 0.1f)
    //         {
    //             baseAttackAnim = PLAYER_RUN_ATTACK;
    //         }
    //         else
    //         {
    //             baseAttackAnim = PLAYER_ATTACK;
    //         }

    //         string nextAttackAnim = baseAttackAnim + "_" + index_anim_attack;

    //         ChangeAnimationState(nextAttackAnim, true);
    //         Invoke(nameof(ResetAttack), timeToAttack);
    //         return;
    //     }

    //     isAttacking = false;
    //     index_anim_attack++;
    //     if (index_anim_attack > 2)
    //     {
    //         index_anim_attack = 1;
    //     }
    // }



    // ATTRIBUT PLAYER
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Animator animator;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Vector2 inputVector;
    private bool jumpButton;
    private bool jumpButtonUp;
    private bool jumpButtonDown;

    private bool isGrounded;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    [Space(10)]
    public float moveSpeed = 2f;

    [Space(10)]
    public float jumpForce = 2f;
    public int maxAirJumps = 2;
    public int jumpsLeft = 2;

    public delegate void JumpAction(int numberOfJump);
    public event JumpAction onGroundJump;
    public event JumpAction onAirJump;
    public delegate void changedMaxJumps(int amount);
    public event changedMaxJumps onChangedMaxJumps;
    public delegate void filledJumpsLeft();
    public event filledJumpsLeft onFilledJumpsLeft;


    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    public int count;
    void Update()
    {
        isGrounded = isGroundedCast();
        inputManager();
        animationsManager();
        jump();
    }

    private void FixedUpdate()
    {
        walk();
    }


    void inputManager()
    {
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpButton = Input.GetButton("Jump");
        jumpButtonDown = Input.GetButtonDown("Jump");
        jumpButtonUp = Input.GetButtonUp("Jump");
    }

    void animationsManager()
    {
        if (rb.velocity.x > .1f)
            sprite.flipX = false;
        else if (rb.velocity.x < -.1f)
            sprite.flipX = true;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isGrounded", isGrounded);

        if (rb.velocity.y > .1)
            animator.SetBool("isJumping", true);
        else
            animator.SetBool("isJumping", false);
    }


    void walk()
    {
        if (inputVector.x > 0.1f || inputVector.x < -0.1f)
            rb.AddForce(new Vector2((inputVector.x * moveSpeed), 0f), ForceMode2D.Impulse);
    }


    void jump()
    {
        if (isGrounded)
        {
            if (jumpsLeft != maxAirJumps)
            {
                jumpsLeft = maxAirJumps;
                if (onFilledJumpsLeft != null)
                    onFilledJumpsLeft();
            }
        }
        if (jumpButtonDown) // begin jump
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                rb.gravityScale = 1;
                if (onGroundJump != null)
                    onGroundJump(jumpsLeft);
            }
            else if (jumpsLeft > 0)
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                jumpsLeft--;
                rb.gravityScale = 1;
                if (onAirJump != null)
                    onAirJump(jumpsLeft);
            }
        }
        if (rb.velocity.y > 0 && jumpButtonUp && !isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
            rb.gravityScale = 3;
        }
        if (rb.velocity.y < 0 && !isGrounded && jumpButton)
        {
            rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
            Debug.Log("slow fall");
            rb.gravityScale = 3;
        }
        else if (rb.velocity.y < 0 && !isGrounded)
        {
            rb.AddForce(new Vector2(0f, -0.3f), ForceMode2D.Impulse);
            Debug.Log("normal fall");
            rb.gravityScale = 3;
        }
    }

    private void changeMaxJumps(int amount)
    {
        maxAirJumps += amount;
        if (maxAirJumps < 0)
            maxAirJumps = 0;
        if(onChangedMaxJumps != null)
            onChangedMaxJumps(amount);
    }

    private bool isGroundedCast()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, groundLayer);
    }
}

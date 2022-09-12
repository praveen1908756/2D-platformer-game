using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fox : MonoBehaviour
{
    #region instances
    //priv
    Rigidbody2D rb;
    Animator animator;
    [SerializeField]Transform groundCheckCollider;
    [SerializeField]LayerMask groundLayer;

    const float groundCheckRadius = 0.2f;
    [SerializeField]float speed = 2;
    [SerializeField]float jumpPower = 30;
    float horizontalValue;
    float runSpeedModifier = 2f;

    bool facingRight = true;    //def true - always facing right
    bool isRunning;
    bool jump;
    bool isGrounded = true;
    #endregion

    //whenever script loaded into game
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    //every frame  - inputs, continuous update
    void Update(){
        //setting yVelocity
        animator.SetFloat("yVelocty", rb.velocity.y);

        // Input.GetAxis()     //-1 to 1
        horizontalValue = Input.GetAxisRaw("Horizontal");      // -1, 0 and 1
        
        //running using leftshift
        if(Input.GetKeyDown(KeyCode.LeftShift))   //enable run
            isRunning = true;
        if(Input.GetKeyUp(KeyCode.LeftShift))   //disable run
            isRunning = false;
        //jumping
        if(Input.GetButtonDown("Jump")){
            jump = true;
            animator.SetBool("Jump", true);
        }
        else if(Input.GetButtonUp("Jump"))
            jump = false;

    }

    //every fixed frame, physics interactions, move
    void FixedUpdate(){
        GroundCheck();
        Move(horizontalValue, jump);
    }

    void GroundCheck(){
        isGrounded = false;
        //check if player is on ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if(colliders.Length > 0)
            isGrounded = true;
        
        //grounded -> jump bool disabled (if fox touches ground)
        animator.SetBool("Jump", !isGrounded);
    }

    //moving fox
    void Move(float dir, bool jumpFlag){
        #region Move & Run
        float xVal = dir * speed * Time.fixedDeltaTime;  //time to make speed same for low/high end pcs
        //increase speed if lshift is down
        if(isRunning)
            xVal *= runSpeedModifier;

        Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
        rb.velocity = targetVelocity;

        //store current scale
        Vector3 currentScale = transform.localScale;
        //if right -> left arrow -> face left
        if(facingRight && dir < 0){
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        //if left -> right arrow -> face right
        else if(facingRight == false && dir > 0){
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }
        // 0 idle, 0.6 walking, 1.2 running -> x val
        //set velocity acc to x val
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    
        #region Jump
        //if player grounded and space is down
        if(isGrounded && jumpFlag){
            isGrounded = false;
            jumpFlag = false;
            //add jump force
            rb.AddForce(new Vector2(0f, jumpPower));
        }
        #endregion
    }
}

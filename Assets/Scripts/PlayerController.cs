using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpPower = 2.0f;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //초기화
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        //점프
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }
        
        //애니메이션
        if(rb.linearVelocity.normalized.x == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }
    void FixedUpdate()
    {
        //방향키 누르면 움직임
        float h = Input.GetAxisRaw("Horizontal");
        if(h > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(h < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        //float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h * speed, rb.linearVelocity.y);

        //랜딩 플랫폼
        if(rb.linearVelocity.y < -0.5f){
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            Debug.DrawRay(rb.position, Vector3.down, new Color(1,0,0));
            RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                Debug.Log(rayHit.collider.name);
                if(rayHit.distance < 1.2f)
                {
                    animator.SetBool("isFalling", false);
                }
                
            }
        }
    }

    
}

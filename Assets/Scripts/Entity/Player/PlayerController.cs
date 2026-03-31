using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.Burst.Intrinsics; //삭제해도 됨
using Unity.Mathematics;      //삭제해도 됨
using Unity.VisualScripting;  //삭제해도 됨
using Unity.VisualScripting.InputSystem; //삭제해도 됨
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;     //삭제해도 됨


public class PlayerController : Entity
{
    public float speed = 5f;
    [SerializeField]
    private float jumpPower = 2.0f;

    public float baseDamage = 10f; // 기본 데미지
    public float atkMultiplier = 1f; // 데미지 배율
    public float speedMultiplier = 1f; // 스피드 배율
    public float FinalSpeed => speed * speedMultiplier; // 최종 스피드 
    public float FinalDamage => baseDamage * atkMultiplier; // 최종 데미지
    public bool isDashing = false;


    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Setup();
    }
    void Start()
    {
        //초기화
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Mental = 70f;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isInputLocked) return;
        //점프
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping") && !animator.GetBool("isFalling")) 
        {   // 점프 횟수 제한 제어문(지금은 1회만 가능)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); 
            animator.SetBool("isJumping", true); // 애니메이션 재생을 위한 Bool 변수 값 지정
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Mental -= 5f;
            Debug.Log(Mental);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Mental += 5f;
            Debug.Log(Mental);
        }
        
        //애니메이션
        if(rb.linearVelocity.normalized.x == 0) // Idle과 Run 애니메이션 제어문
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
        
        if(Stamina < maxStamina)
        {
            Stamina += 2f * Time.deltaTime; //기획서 내용에 따라 2로 변경 3/28
        }
        
    }
    void FixedUpdate()
    {
        if(isDashing){return; }
        //방향키 누르면 움직임
        float h = Input.GetAxisRaw("Horizontal");
        if(h > 0) // 움직이는 방향에 따라 localScale 함수를 이용해 방향 전환
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(h < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        //float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h * FinalSpeed, rb.linearVelocity.y);
        //랜딩 플랫폼
        if(rb.linearVelocity.y < -0.5f){ // 바닥에 닿았을 때 애니메이션 전환 로직
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            Debug.DrawRay(rb.position, Vector3.down, new Color(1,0,0));
            RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));
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

    public override float maxHP => 100f;
    public override float maxMP => 100f;
    public override float maxMental => 100f;
    public override float maxStamina => 50f;
    public override void TakeDamage(float damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            HP = 0; //플레이어 사망처리
        }
    }

}

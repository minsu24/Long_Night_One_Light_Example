using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : Entity
{
    [SerializeField]
    private float jumpPower = 2.0f;
    [SerializeField]
    private float knockbackPower = 15.0f;
    private float _stamina;
    public float atkMultiplier = 1f; // 데미지 배율
    public float speedMultiplier = 1f; // 스피드 배율
    public float FinalSpeed => Speed * speedMultiplier; // 최종 스피드 
    public float FinalDamage => Attack_Power * atkMultiplier; // 최종 데미지
    public bool isDashing = false;
    private bool isInvincible = false;
    private float invincibleDuration = 1.0f; // 무적 시간 (초)

    public float h;

    bool isKnockback;

    public DialogueManager dialogueManager; // 대화 중 입력 제어용 DialogueManager

    public float Stamina
    {
        set => _stamina = Mathf.Clamp(value, 0, maxStamina);
        get => _stamina;
    }

    Rigidbody2D rb;
    Collider2D playerCollider; //변수명 변경
    SpriteRenderer spriteRenderer;
    public Animator animator;
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
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Mental = 70f;
        Speed = 10f;
        Attack_Power = 10f;
        Stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        // 대화 중이거나 대화 직후 입력 잠금 중이면 점프/이동 입력 막기
        if ((dialogueManager != null && dialogueManager.IsInputBlocked()) || isKnockback)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        // 다른 시스템에서 입력 잠금이 걸린 경우도 입력 막기
        if (GameManager.instance != null && GameManager.instance.isInputLocked) return;

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
        if (rb.linearVelocity.normalized.x == 0) // Idle과 Run 애니메이션 제어문
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        if (Stamina < maxStamina)
        {
            Stamina += 2f * Time.deltaTime; //기획서 내용에 따라 2로 변경 3/28
        }

    }
    void FixedUpdate()
    {
        // 대화 중이거나 대화 직후 입력 잠금 중이면 좌우 이동 막기
        if ((dialogueManager != null && dialogueManager.IsInputBlocked()))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 다른 시스템에서 입력 잠금이 걸린 경우도 좌우 이동 막기
        if (GameManager.instance != null && GameManager.instance.isInputLocked)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (isDashing || isKnockback) { return; }

        //방향키 누르면 움직임
        h = Input.GetAxisRaw("Horizontal");
        if (h > 0) // 움직이는 방향에 따라 localScale 함수를 이용해 방향 전환
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (h < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        //float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h * FinalSpeed, rb.linearVelocity.y);
        //랜딩 플랫폼
        if (rb.linearVelocity.y < -0.5f)
        { // 바닥에 닿았을 때 애니메이션 전환 로직
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            Debug.DrawRay(rb.position, Vector3.down, new Color(1, 0, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                Debug.Log(rayHit.collider.name);
                if (rayHit.distance < 1.2f)
                {
                    animator.SetBool("isFalling", false);
                }

            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.collider.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                Debug.Log("적이랑 충돌");
                if (!isInvincible)
                {
                    TakeDamage(enemyController.Attack_Power);
                    float direction = transform.position.x > collision.transform.position.x ? 1f : -1f;
                    ApplyKnockback(direction);
                    Debug.Log(enemyController.Attack_Power);
                    //StartCoroutine(InvincibleCoroutine());
                    
                }
            }
            else
            {
                Debug.Log("에너미컨트롤러 없음");
            }

        }
    }

    void ApplyKnockback(float direction)
    {
        StartCoroutine(KnockBackTRoutine()); // 이동 제어권 강탈
        rb.linearVelocity = Vector2.zero;
        Vector2 knockbackForce = new Vector2(direction, 0.4f).normalized * knockbackPower;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(InvincibleCoroutine());
    }

    public void startMentalHeal(float amount, float duration)
    {
        StartCoroutine(MentalHealRoutine(amount, duration));
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;
        yield return new WaitForSeconds(invincibleDuration);

        c.a = 1.0f;
        spriteRenderer.color = c;
        isInvincible = false;
    }

    private IEnumerator KnockBackTRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(0.5f);
        isKnockback = false;
    }

    private IEnumerator MentalHealRoutine(float amount, float duration)
    {
        float timer = 0f;
        float startMental = Mental;
        float targetMental = Mathf.Min(startMental + amount, maxMental); //최대치 고정

        while(timer < duration)
        {
            timer += Time.deltaTime;
            //Lerp를 이용해서 정신력이 서서히 오르도록 만듦
            Mental = Mathf.Lerp(startMental, targetMental, timer / duration);
            //다음 프레임까지 대기
            yield return null;
        }
        Mental = targetMental;
    }

    public override float maxHP => 100f;
    public override float maxMP => 100f;
    public override float maxMental => 100f;
    public float maxStamina => 50f;
    public override void TakeDamage(float damage)
    {
        Debug.Log("데미지");
        if(HP > 0)
        {
            HP -= damage;
        }
        if (HP <= 0)
        {
            HP = 0; //플레이어 사망처리
            rb.simulated = false;
        }
        Debug.Log(HP);
    }

}
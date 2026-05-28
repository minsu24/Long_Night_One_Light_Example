using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : Entity
{
    [SerializeField]
    private float jumpPower = 2.0f;
    [SerializeField]
    private float knockbackPower = 15.0f;
    private float _stamina;
    public float atkMultiplier = 1f; // 데미지 배율
    public float speedMultiplier = 1f; // 스피드 배율
    public float climbingSpeed = 5f; // 줄 오르는 속도
    public float FinalSpeed => Speed * speedMultiplier; // 최종 스피드 
    public float FinalDamage => Attack_Power * atkMultiplier; // 최종 데미지

    public bool isDashing, isClimbing, isGrounded, isMoved, isCharging, sSkilling = false;
    private bool isInvincible = false;
    private float invincibleDuration = 1.5f; // 무적 시간 (초)
    private float ropeX;
    private int playerLayer, EnemyLayer;

    [SerializeField] private int maxJumpCount = 2; // 최대 점프 횟수 (2 = 2단 점프)
    private int jumpCount = 0;                     // 현재 점프 횟수

    public float h, v;

    bool isKnockback;

    public DialogueManager dialogueManager; // 대화 중 입력 제어용 DialogueManager

    public DeadUI deadUI;

    public float Stamina
    {
        set => _stamina = Mathf.Clamp(value, 0, maxStamina);
        get => _stamina;
    }

    public Rigidbody2D rb;
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
        EnemyLayer = LayerMask.NameToLayer("Enemy");
        playerLayer = LayerMask.NameToLayer("Player");
        deadUI = FindAnyObjectByType<DeadUI>();
        Mental = 70f;
        Speed = 10f;
        Attack_Power = 10f;
        Stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        //줄을 오르는 중이라면 별개의 이동 로직 작동
        if (isClimbing)
        {
            if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
            {
                isClimbing = false;
                isGrounded = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 기존 수직 속도 초기화 후 점프
                rb.AddForce(Vector2.one * jumpPower, ForceMode2D.Impulse);
                jumpCount++;
                return;
            }
            rb.gravityScale = 0; //중력을 0으로
            transform.position = new Vector3(ropeX, transform.position.y, 0);  
        }
        else
        {
            rb.gravityScale = 4f;
        }

        // 대화 중이거나 대화 직후 입력 잠금 중이면 점프/이동 입력 막기
        if ((dialogueManager != null && dialogueManager.IsInputBlocked()) || isKnockback || isCharging || sSkilling)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        // 다른 시스템에서 입력 잠금이 걸린 경우도 입력 막기
        if (GameManager.instance != null && GameManager.instance.isInputLocked) return;

        //점프 (2단 점프 지원)
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            isGrounded = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 기존 수직 속도 초기화 후 점프
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            Debug.Log(jumpCount);
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
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
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("BossRoom1");
            MapTransferData.TargetSpawnPointname = "In_BossRoom1_Portal";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("Vilage");
            MapTransferData.TargetSpawnPointname = "In_Vilage_Portal";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Dark_Forest2");
            MapTransferData.TargetSpawnPointname = "In_DarkForest2_Portal";
        }

        //애니메이션
        if (isMoved) // Idle과 Run 애니메이션 제어문
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (!isGrounded && rb.linearVelocity.y > 0.1f)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
        }
        // 2. 추락 중인지 확인 (속도가 음수이고 바닥이 아닐 때)
        else if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        // 3. 착지 상태
        else if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }

        if (Stamina < maxStamina)
        {
            Stamina += 2f * Time.deltaTime; //기획서 내용에 따라 2로 변경 3/28
        }

    }
    void FixedUpdate()
    {
        if (isClimbing)
        {
            v = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, v * climbingSpeed); 
        }

        // 대화 중이거나 대화 직후 입력 잠금 중이면 좌우 이동 막기
        if ((dialogueManager != null && dialogueManager.IsInputBlocked()) || isCharging || sSkilling)
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
        if(rb.linearVelocity.x == 0)
        {
            isMoved = false;
        }
        else
        {
            isMoved = true;
        }
        //랜딩 플랫폼

        Debug.DrawRay(rb.position, Vector3.down, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rb.position, Vector3.down, 1.5f, LayerMask.GetMask("Platform"));
        if (rayHit.collider != null && rayHit.distance < 1.2f)
        {                         
            isGrounded = true;
            jumpCount = 0; // 착지 시 점프 횟수 초기화
        }
        else
        {
            isGrounded = false;
        }
    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("무언가와 닿음: " + collision.name);
        if (collision.CompareTag("Rope"))
        {
            Rope currentrope = collision.GetComponent<Rope>();
            ropeX = currentrope.GetRopeCenterX();
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyHitBox hitbox = collision.GetComponent<EnemyHitBox>();
            if (hitbox != null)
            {
                Debug.Log("히트박스(무기)에 맞음!");
                if (!isInvincible)
                {
                    TakeDamage(hitbox.damage);
                    // 넉백 방향은 히트박스를 생성한 몬스터(owner)의 위치를 기준으로 계산
                    float direction = transform.position.x > hitbox.attackOwner.position.x ? 1f : -1f;
                    ApplyKnockback(direction);
                }
                return; // 맞았으니 아래 코드는 실행 안 하고 종료
            }
            EnemyController enemyController = collision.GetComponent<EnemyController>();
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
        if(collision.CompareTag("BossProjectile"))
        {
            float direction = transform.position.x > collision.transform.position.x ? 1f : -1f;
            ApplyKnockback(direction);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Boss00"))
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

    public void ApplyKnockback(float direction)
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
        Physics2D.IgnoreLayerCollision(EnemyLayer, playerLayer, true);
        Color c = spriteRenderer.color;
        c.a = 0.5f;
        spriteRenderer.color = c;
        yield return new WaitForSeconds(invincibleDuration);
        Physics2D.IgnoreLayerCollision(EnemyLayer, playerLayer, false);
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
        if(isInvincible) return;
        Debug.Log("데미지");
        if(HP > 0)
        {
            HP -= damage;
        }
        if (HP <= 0)
        {
            Debug.Log("사망로직 진입");
            deadUI.OpenDeadUI();
            Debug.Log("UI 오픈");
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            Time.timeScale = 0;
            
        }
        Debug.Log(HP);
    }

    void SSkillingToTrue()
    {
        sSkilling = true;
    }

    void SSkillingToFalse()
    {
        sSkilling = false;
    }

    public void ResetPlayerData()
    {
        StopCoroutine(InvincibleCoroutine());
        Time.timeScale = 1;
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;
        HP = 100;
        MP = 100;
        Stamina = 50;
        isInvincible = false;
        rb.simulated = true;
    }

}
using UnityEngine;

public class SSkillFire : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f; // 발사체 속도 (발사 시)
    private Vector3 startPos, moveDirection;
    

    public float damage;
    public PlayerController playerController;
    public AudioClip explosionClip;

    [SerializeField]
    private GameObject enemy;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    BoxCollider2D boxCollider2D;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position; // 시작 위치 저장
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 발사체 이동 로직
        if(Vector3.Distance(startPos, transform.position) >= 12.5f) // 일정 거리 이동하면 삭제
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 dir) // PlayerAttackSystem에서 방향 받아오는 함수
    {
        moveDirection = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<EnemyController>().TakeDamage(damage); // 적에게 데미지
        if (HitShaker.Instance != null)
        {
            HitShaker.Instance.TriggerShake(moveDirection, 0.4f, 0.12f);
        }
        else
        {
            Debug.LogWarning("씬에 CameraShaker 인스턴스가 존재하지 않습니다! 오브젝트를 확인해 주세요.");
        }
        
        // 3. 투사체 비활성화
        if (boxCollider2D != null) boxCollider2D.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (audioSource != null) audioSource.enabled = false;

        AudioSource.PlayClipAtPoint(explosionClip, transform.position, 1.0f);
        // 4. 안전하게 시간차 파괴
        Destroy(gameObject, 0.5f);
        }

}

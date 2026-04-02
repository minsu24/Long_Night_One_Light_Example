using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EnemyController : Entity
{
    SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    void Awake()
    {
        base.Setup();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //몬스터 이동 및 AI 로직 추가 예정 
    }

    public override float maxHP => 100;
    public override float maxMP => 0;
    public override float maxMental => 0f;
    //public override float maxStamina => 0f;

    public override void TakeDamage(float damage) // 데미지 계산
    {
        if(HP > 0)
        {
            HP -= damage;
        }
        if(HP<=0)
        {
            ReduceMental(); // 죽으면 멘탈 감소
            Destroy(gameObject);
        }
        Debug.Log("적 HP : " + HP);
        StartCoroutine("HitAnimation");
    }

    private IEnumerator HitAnimation() 
    {
        Color color = spriteRenderer.color;

        color.a = 0.2f;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.2f);

        color.a = 1;
        spriteRenderer.color = color;
    }

    private void ReduceMental()
    {
        playerController.Mental -= 2;
    }
}

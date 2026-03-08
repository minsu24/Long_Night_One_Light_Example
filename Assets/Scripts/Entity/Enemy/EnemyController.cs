using System.Collections;
using UnityEngine;

public class EnemyController : Entity
{
    SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        base.Setup();
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float maxHP => 100;
    public override float maxMP => 0;
    public override float maxMental => 0f;

    public override void TakeDamage(float damage) // 데미지 계산
    {
        if(HP > 0)
        {
            HP -= damage;
        }
        if(HP==0)
        {
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

}

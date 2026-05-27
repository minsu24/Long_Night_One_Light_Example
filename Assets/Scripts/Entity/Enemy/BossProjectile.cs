using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed;
    public float moveDistance = 30f;
    public float damage = 10f;
    private Vector2 moveDirection;
    private Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); // 발사체 이동 로직
        if(Vector3.Distance(startPos, transform.position) >= moveDistance) // 일정 거리 이동하면 삭제
        {
                Destroy(gameObject);
        }
    }

    public void Setup(Vector2 dir)
    {
        moveDirection = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

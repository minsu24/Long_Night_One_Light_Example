using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    private Vector3 startPos, moveDirection;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        if(Vector3.Distance(startPos, transform.position) >= 12.5f)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            rb.simulated = false;
            Destroy(gameObject);
            
        }
    }

}

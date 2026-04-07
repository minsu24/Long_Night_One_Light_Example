using UnityEngine;

public class BreakableBlock : MonoBehaviour // 실라의 공격에 적중히 태울 수 있는 오브젝트
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttack"))
        {
            Destroy(gameObject); // 블럭만 사라짐
        }
    }
}

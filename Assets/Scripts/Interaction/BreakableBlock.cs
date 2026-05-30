using UnityEngine;

public class BreakableBlock : MonoBehaviour // 실라의 공격에 적중히 태울 수 있는 오브젝트
{
    [SerializeField] private string wallID;

    void Start()
    {
        // 매니저에게 이 벽이 깨진 적이 있는지 물어봄
        if (DataManager.instance != null && DataManager.instance.IsWallDestroyed(wallID))
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fire") || collision.CompareTag("SSkillFire"))
        {
            if(DataManager.instance != null)
            {
                DataManager.instance.SetWallDestroyed(wallID);
                Destroy(gameObject); // 블럭만 사라짐
            }
            
        }
    }
}

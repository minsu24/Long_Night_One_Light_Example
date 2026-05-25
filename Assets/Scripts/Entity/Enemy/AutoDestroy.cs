using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // 히트박스 자동 삭제를 위한 스크립트
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class ArrowMoveMent : MonoBehaviour
{
    private RectTransform rectTransform;

    [Header("이동 설정")]
    public float moveSpeed = 100f; // 초당 이동 속도
    
    [Header("대각선 방향 (X, Y 비율)")]
    // (1, 1)이면 우측 상단 45도, (1, -1)이면 우측 하단 45도 대각선
    public Vector2 moveDirection = new Vector2(1f, 1f); 

    [Header("왕복 효과 사용 여부")]
    public bool isPingPong = false;
    public float pingPongFrequency = 2f; // 왕복 속도
    public float pingPongDistance = 20f;  // 왕복 거리

    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
        
        // 방향 벡터 정규화 (길이를 1로 만들어 정확한 속도 계산)
        moveDirection.Normalize();
    }

    void Update()
    {
        if (isPingPong)
        {
            // 시간(Time.time)에 따라 -1 ~ 1 사이를 반복하는 Sin 곡선 활용
            float wave = Mathf.Sin(Time.time * pingPongFrequency) * pingPongDistance;
            rectTransform.anchoredPosition = startPosition + (moveDirection * wave);
        }
        else
        {
            // 한 방향으로 멈추지 않고 대각선 이동
            rectTransform.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}

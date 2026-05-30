using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    public float scrollSpeed = 50f;          // 기본 스크롤 속도
    public float fastScrollMultiplier = 3f;  // 배속 스크롤 시 곱해질 속도

    [Header("종료 지점 설정")]
    public float endPositionY = 2000f;       // 크레딧이 멈출 Y 좌표 (텍스트 길이에 맞춰 조절)
    
    public string nextSceneName = "Main_Screen"; // 크레딧 종료 후 이동할 씬 이름

    private RectTransform rectTransform;
    private PlayerController playerController;

    void Start()
    {
        // UI의 위치를 제어하기 위해 RectTransform을 가져옵니다.
        rectTransform = GetComponent<RectTransform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        // 1. 현재 스크롤 속도 결정 (스페이스바를 누르고 있으면 배속)
        float currentSpeed = scrollSpeed;
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed *= fastScrollMultiplier;
        }

        // 2. 텍스트 위로 이동
        rectTransform.anchoredPosition += Vector2.up * currentSpeed * Time.unscaledDeltaTime;

        // 3. 종료 지점 도달 시 처리
        if (rectTransform.anchoredPosition.y >= endPositionY)
        {
            EndCredits();
        }
    }

    void EndCredits()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject dataManager = GameObject.FindGameObjectWithTag("DataManager");
        GameObject mapManager = GameObject.FindGameObjectWithTag("MapManager");
        GameObject gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        // 타이틀 화면으로 돌아가거나 게임을 종료하는 로직
        if(player != null)
        {
            Destroy(player);
        }
        if(gameManager != null)
        {
            Destroy(gameManager);
        }
        if(mapManager != null)
        {
            Destroy(mapManager);
        }
        if(gameCanvas != null)
        {
            Debug.Log("캔버스 삭제");
            Destroy(gameCanvas);
        }

        

        Debug.Log("엔딩 크레딧 종료");
        SceneManager.LoadScene(nextSceneName);
    }
}

using System.Collections;
using UnityEngine;

public class EyeBlinkEffect : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform topEyelid;
    public RectTransform bottomEyelid;


    [Header("Settings")]
    public float firstOpenRatio = 0.25f;
    public float firstOpenDuration = 0.6f;  // 살짝 뜨는 시간
    public float firstCloseDuration = 0.4f; // 다시 질끈 감는 시간

    public float SecondOpenDuration = 0.8f;  // 살짝 뜨는 시간
    public float SecondCloseDuration = 0.6f; // 다시 질끈 감는 시간

    public float holdClosedDuration = 0.5f; // 감은 채로 잠시 어질어질하게 대기하는 시간
    public float finalOpenDuration = 1.8f;   // 완전히 번쩍 뜨는 시간
    public bool isEnd = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void WakeUp()
    {
        Debug.Log("눈 효과 시작");
        StartCoroutine(OpenEyesRoutine());
    }

    IEnumerator OpenEyesRoutine()
    {
        Debug.Log("눈 코루틴 시작");
        // 1. 초기 위치 기준 잡기 (화면을 꽉 닫고 있는 상태)
        Vector2 topClosedPos = topEyelid.anchoredPosition;
        Vector2 bottomClosedPos = bottomEyelid.anchoredPosition;

        // 완전히 열렸을 때의 목표 위치 계산 (자기 높이만큼 바깥으로 탈출)
        Vector2 topFullOpenPos = topClosedPos + new Vector2(0, topEyelid.rect.height);
        Vector2 bottomFullOpenPos = bottomClosedPos - new Vector2(0, bottomEyelid.rect.height);

        // 첫 번째 살짝 떴을 때의 중간 목표 위치 계산
        Vector2 topPartialOpenPos = Vector2.Lerp(topClosedPos, topFullOpenPos, firstOpenRatio);
        Vector2 bottomPartialOpenPos = Vector2.Lerp(bottomClosedPos, bottomFullOpenPos, firstOpenRatio);


        // --- [STEP 1] 스르륵 살짝 눈 뜨기 ---
        float elapsedTime = 0f;
        while (elapsedTime < firstOpenDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / firstOpenDuration);
            
            topEyelid.anchoredPosition = Vector2.Lerp(topClosedPos, topPartialOpenPos, t);
            bottomEyelid.anchoredPosition = Vector2.Lerp(bottomClosedPos, bottomPartialOpenPos, t);
            yield return null;
        }

        // --- [STEP 2] 정신 차리려고 질끈 다시 감기 ---
        elapsedTime = 0f;
        while (elapsedTime < firstCloseDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            // 감을 때는 탁! 하고 빠르게 감기도록 가속도 느낌(t * t)을 줍니다.
            float t = elapsedTime / firstCloseDuration;
            float accelT = t * t; 

            topEyelid.anchoredPosition = Vector2.Lerp(topPartialOpenPos, topClosedPos, accelT);
            bottomEyelid.anchoredPosition = Vector2.Lerp(bottomPartialOpenPos, bottomClosedPos, accelT);
            yield return null;
        }
        
        // 확실하게 닫힌 상태로 고정
        topEyelid.anchoredPosition = topClosedPos;
        bottomEyelid.anchoredPosition = bottomClosedPos;    

        //한 번 더 반복 
        elapsedTime = 0f;
        while (elapsedTime < SecondOpenDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / SecondOpenDuration);
            
            topEyelid.anchoredPosition = Vector2.Lerp(topClosedPos, topPartialOpenPos, t);
            bottomEyelid.anchoredPosition = Vector2.Lerp(bottomClosedPos, bottomPartialOpenPos, t);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < SecondCloseDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            // 감을 때는 탁! 하고 빠르게 감기도록 가속도 느낌(t * t)을 줍니다.
            float t = elapsedTime / SecondCloseDuration;
            float accelT = t * t; 

            topEyelid.anchoredPosition = Vector2.Lerp(topPartialOpenPos, topClosedPos, accelT);
            bottomEyelid.anchoredPosition = Vector2.Lerp(bottomPartialOpenPos, bottomClosedPos, accelT);
            yield return null;
        }
        
        // 확실하게 닫힌 상태로 고정
        topEyelid.anchoredPosition = topClosedPos;
        bottomEyelid.anchoredPosition = bottomClosedPos;


        // --- [STEP 3] 완전히 감은 채로 잠시 암전 (정신 아득한 연출) ---
        yield return new WaitForSecondsRealtime(holdClosedDuration);

        // --- [STEP 4] 드디어 완전히 번쩍 눈 뜨기 ---
        elapsedTime = 0f;
        while (elapsedTime < finalOpenDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            // 완전히 뜰 때는 잠에서 서서히 깨어나는 느낌으로 SmoothStep 처리
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / finalOpenDuration);

            topEyelid.anchoredPosition = Vector2.Lerp(topClosedPos, topFullOpenPos, t);
            bottomEyelid.anchoredPosition = Vector2.Lerp(bottomClosedPos, bottomFullOpenPos, t);
            yield return null;
        }

        // 최종 위치 고정 및 오브젝트 정리
        topEyelid.anchoredPosition = topFullOpenPos;
        bottomEyelid.anchoredPosition = bottomFullOpenPos;
        
        isEnd = true;
        // 연출용 캔버스 비활성화 (드로우콜 최적화)
        gameObject.SetActive(false);
        
    }
}

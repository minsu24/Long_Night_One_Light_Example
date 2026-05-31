using System.Collections;
using UnityEngine;

public class FogEffect : MonoBehaviour
{
    [Tooltip("밖을 가리고 있는 검은색 스프라이트 렌더러")]
    public SpriteRenderer outsideCover; 
    public float fadeDuration = 1.5f; // 풍경이 밝아지는데 걸리는 시간

    private bool isRevealed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 출구 트리거에 닿았고, 아직 연출이 안 나왔다면
        if (collision.CompareTag("Player") && !isRevealed)
        {
            isRevealed = true;
            StartCoroutine(FadeOutCover());
        }
    }

    private IEnumerator FadeOutCover()
    {
        float timer = 0f;
        Color startColor = outsideCover.color;
        // 알파(투명도) 값을 0으로 설정한 목표 색상
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            outsideCover.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null;
        }
        
        // 투명해진 가림막을 완전히 비활성화하여 리소스 절약
        outsideCover.gameObject.SetActive(false);
    }
 
}

using UnityEngine;
using System.Collections;


public class HitShaker : MonoBehaviour
{
    // 전역에서 언제든 이 셰이커에 접근할 수 있도록 싱글톤 인스턴스 선언
    public static HitShaker Instance { get; private set; }

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerShake(Vector2 direction, float magnitude = 0.6f, float duration = 0.2f)
    {
        originalPos = transform.localPosition;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        
        shakeCoroutine = StartCoroutine(ShakeRoutine(direction, magnitude, duration));
    }

    private IEnumerator ShakeRoutine(Vector2 direction, float magnitude, float duration)
    {
        float elapsed = 0.0f;
        Vector3 shakeDir = new Vector3(direction.x, direction.y, 0f).normalized;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float damper = 1.0f - (elapsed / duration);

            Vector3 offset = shakeDir * magnitude * damper;
            offset += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * (magnitude * 0.2f) * damper;

            transform.localPosition = originalPos + offset;
            yield return null;
        }

        transform.localPosition = originalPos;
        shakeCoroutine = null;
    }
}

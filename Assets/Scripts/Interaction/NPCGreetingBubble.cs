using UnityEngine;
using TMPro;
using System.Collections;

public class NPCGreetingBubble : MonoBehaviour
{
    [Header("말풍선 UI")]
    public GameObject bubbleObject;

    public TMP_Text bubbleText;

    [Header("NPC 인사 대사")]
    [TextArea(2, 5)]
    public string[] greetingLines;

    [Header("말풍선 유지 시간")]
    public float bubbleDuration = 2f;

    // 플레이어가 범위 안에 있는지
    private bool playerInRange = false;

    // 이미 인사했는지 확인
    private bool hasGreeted = false;

    private void Start()
    {
        if (bubbleObject != null)
            bubbleObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        // 이미 인사했으면 다시 안 뜸
        if (hasGreeted)
            return;

        ShowGreeting();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        // 범위를 나가면 다시 인사 가능하게 초기화
        hasGreeted = false;
    }

    private void ShowGreeting()
    {
        if (bubbleObject == null || bubbleText == null)
        {
            Debug.Log("NPCGreetingBubble 연결 안 됨");
            return;
        }

        if (greetingLines == null || greetingLines.Length == 0)
            return;

        hasGreeted = true;

        int randomIndex = Random.Range(0, greetingLines.Length);

        bubbleText.text = greetingLines[randomIndex];

        bubbleObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideBubble());
    }

    private IEnumerator HideBubble()
    {
        yield return new WaitForSeconds(bubbleDuration);

        if (bubbleObject != null)
            bubbleObject.SetActive(false);
    }
}
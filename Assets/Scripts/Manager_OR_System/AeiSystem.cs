using UnityEngine;
using TMPro;
using System.Collections;

public class AeiSystem : MonoBehaviour
{
    // 플레이어 Entity
    private Entity playerEntity;

    // 정신력 감소 비교용
    private float previousMental;

    // 마지막으로 에이 말풍선이 나온 정신력 값
    private float lastAeiTalkMental;

    // 첫 정신력 감소 대사가 나왔는지 확인
    private bool hasFirstAeiTalk = false;

    // 이전 정신력 단계 저장
    private MentalState previousMentalState;

    [Header("에이 말풍선 UI")]
    public TMP_Text bubbleText;
    public GameObject bubbleObject;

    [Header("하단 경고 UI")]
    public WarningMessageUI warningUI;

    [Header("정신력 정상 상태 대사")]
    public string[] normalLines =
    {
        "괜찮아? 조금 지친 것 같아.",
        "천천히 가도 돼."
    };

    [Header("정신력 우울 상태 대사")]
    public string[] depressionLines =
    {
        "무리하지 마. 잠깐 숨 좀 고르자.",
        "괜찮아. 아직 버틸 수 있어."
    };

    [Header("정신력 불안 상태 대사")]
    public string[] anxietyLines =
    {
        "호흡이 빨라졌어. 천천히 움직여.",
        "서두르지 마. 주변을 먼저 봐."
    };

    [Header("정신력 붕괴 상태 대사")]
    public string[] collapseLines =
    {
        "위험해. 지금은 멈춰야 해.",
        "실라, 더 버티면 무너질지도 몰라."
    };

    [Header("정신력 단계 진입 경고 대사")]
    public DialogueLine[] depressionWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "실라… 조금 쉬는 게 좋겠어." }
    };

    public DialogueLine[] anxietyWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "지금 상태가 이상해. 주변을 조심해." }
    };

    public DialogueLine[] collapseWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "안 돼… 더 버티면 정말 무너질지도 몰라." }
    };

    void Start()
    {
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();

        playerEntity.OnMentalChanged += CheckMental;

        previousMental = playerEntity.Mental;
        lastAeiTalkMental = playerEntity.Mental;

        previousMentalState = GameManager.instance.CurrentMentalState;

        if (bubbleObject != null)
            bubbleObject.SetActive(false);
    }

    private void CheckMental(float currentMental)
    {
        CheckMentalStateWarning();

        if (currentMental < previousMental)
        {
            if (!hasFirstAeiTalk)
            {
                ShowBubble(GetAeiLineByMentalState());

                hasFirstAeiTalk = true;
                lastAeiTalkMental = currentMental;
            }
            else if (currentMental <= lastAeiTalkMental - 3)
            {
                ShowBubble(GetAeiLineByMentalState());

                lastAeiTalkMental = currentMental;
            }
        }

        previousMental = currentMental;
    }

    private void CheckMentalStateWarning()
    {
        if (GameManager.instance == null)
            return;

        MentalState currentState = GameManager.instance.CurrentMentalState;

        if (currentState == previousMentalState)
            return;

        previousMentalState = currentState;

        if (warningUI == null)
            return;

        switch (currentState)
        {
            case MentalState.Depression:
                ShowWarningDialogue(depressionWarningLines);
                break;

            case MentalState.Anxiety:
                ShowWarningDialogue(anxietyWarningLines);
                break;

            case MentalState.Collapse:
                ShowWarningDialogue(collapseWarningLines);
                break;
        }
    }

    private void ShowWarningDialogue(DialogueLine[] warningLines)
    {
        if (warningLines == null || warningLines.Length == 0)
            return;

        warningUI.ShowWarning(warningLines);
    }

    private string GetAeiLineByMentalState()
    {
        if (GameManager.instance == null)
            return "괜찮아?";

        switch (GameManager.instance.CurrentMentalState)
        {
            case MentalState.Normal:
                return GetRandomLine(normalLines);

            case MentalState.Depression:
                return GetRandomLine(depressionLines);

            case MentalState.Anxiety:
                return GetRandomLine(anxietyLines);

            case MentalState.Collapse:
                return GetRandomLine(collapseLines);

            default:
                return "괜찮아?";
        }
    }

    private string GetRandomLine(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return "괜찮아?";

        int index = Random.Range(0, lines.Length);
        return lines[index];
    }

    private void ShowBubble(string message)
    {
        Debug.Log("에이 말풍선 출력: " + message);

        if (bubbleText == null || bubbleObject == null)
        {
            Debug.Log("말풍선 연결 안 됨");
            return;
        }

        bubbleText.text = message;
        bubbleObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideBubbleAfterTime());
    }

    private IEnumerator HideBubbleAfterTime()
    {
        yield return new WaitForSeconds(2.5f);

        bubbleObject.SetActive(false);
    }
}
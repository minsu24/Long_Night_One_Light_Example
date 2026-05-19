using UnityEngine;
using TMPro;
using System.Collections;

public class AeiSystem : MonoBehaviour
{
    // 플레이어의 정신력 값을 가져오기 위한 Entity 변수
    // Entity 안에 Mental 값과 OnMentalChanged 이벤트가 들어 있음
    private Entity playerEntity;

    // 이전 프레임 또는 이전 변화 시점의 정신력 값
    // 현재 정신력과 비교해서 정신력이 감소했는지 판단하는 데 사용
    private float previousMental;

    // 에이가 마지막으로 말풍선을 띄웠던 정신력 값
    // 정신력이 1씩 줄어들 때마다 계속 말하지 않도록 하기 위해 사용
    private float lastAeiTalkMental;

    // 정신력이 처음 감소했을 때 에이가 이미 말했는지 확인하는 변수
    // false면 첫 감소 시 무조건 한 번 말함
    private bool hasFirstAeiTalk = false;

    // 이전 정신력 단계 저장
    // Normal, Depression, Anxiety, Collapse 중 이전 상태를 기억해서
    // 상태가 바뀐 순간에만 하단 경고 UI가 뜨게 하기 위해 사용
    private MentalState previousMentalState;

    // 말풍선 UI의 원래 크기 저장
    // 불씨 오브젝트가 왼쪽을 볼 때 Scale X가 뒤집혀도,
    // 말풍선 글자가 같이 뒤집히지 않도록 보정하는 데 사용
    private Vector3 originalBubbleScale;

    [Header("에이 말풍선 UI")]
    // 에이 옆에 뜨는 작은 말풍선의 텍스트
    public TMP_Text bubbleText;

    // 에이 옆 말풍선 전체 오브젝트
    // 보통 FireSpirit 아래에 있는 Canvas를 연결함
    public GameObject bubbleObject;

    [Header("하단 경고 UI")]
    // 정신력 단계가 바뀔 때 화면 아래에 띄우는 경고 UI
    // NPC 대화창과 다르게 플레이어 이동을 막지 않음
    public WarningMessageUI warningUI;

    [Header("정신력 정상 상태 대사")]
    // Normal 상태일 때 에이 말풍선에 랜덤으로 출력될 대사
    public string[] normalLines =
    {
        "괜찮아? 조금 지친 것 같아.",
        "천천히 가도 돼."
    };

    [Header("정신력 우울 상태 대사")]
    // Depression 상태일 때 에이 말풍선에 랜덤으로 출력될 대사
    public string[] depressionLines =
    {
        "무리하지 마. 잠깐 숨 좀 고르자.",
        "괜찮아. 아직 버틸 수 있어."
    };

    [Header("정신력 불안 상태 대사")]
    // Anxiety 상태일 때 에이 말풍선에 랜덤으로 출력될 대사
    public string[] anxietyLines =
    {
        "호흡이 빨라졌어. 천천히 움직여.",
        "서두르지 마. 주변을 먼저 봐."
    };

    [Header("정신력 붕괴 상태 대사")]
    // Collapse 상태일 때 에이 말풍선에 랜덤으로 출력될 대사
    public string[] collapseLines =
    {
        "위험해. 지금은 멈춰야 해.",
        "실라, 더 버티면 무너질지도 몰라."
    };

    [Header("정신력 단계 진입 경고 대사")]
    // Normal에서 Depression 단계로 진입했을 때 하단 경고 UI에 출력될 대사
    public DialogueLine[] depressionWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "실라… 조금 쉬는 게 좋겠어." }
    };

    // Depression에서 Anxiety 단계로 진입했을 때 하단 경고 UI에 출력될 대사
    public DialogueLine[] anxietyWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "지금 상태가 이상해. 주변을 조심해." }
    };

    // Anxiety에서 Collapse 단계로 진입했을 때 하단 경고 UI에 출력될 대사
    public DialogueLine[] collapseWarningLines =
    {
        new DialogueLine { speaker = "에이", text = "안 돼… 더 버티면 정말 무너질지도 몰라." }
    };

    void Start()
    {
        // Player 태그가 붙은 오브젝트를 찾아 Entity 컴포넌트를 가져옴
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();

        // Entity의 OnMentalChanged 이벤트에 CheckMental 함수를 연결
        // 즉, 플레이어 정신력이 바뀔 때마다 CheckMental 함수가 자동 실행됨
        playerEntity.OnMentalChanged += CheckMental;

        // 게임 시작 시 현재 정신력 값을 기준값으로 저장
        // 이후 정신력이 감소했는지 비교하는 데 사용
        previousMental = playerEntity.Mental;

        // 마지막 말풍선 출력 정신력도 시작 정신력으로 초기화
        lastAeiTalkMental = playerEntity.Mental;

        // 현재 정신력 단계를 저장
        // 이후 단계가 바뀌었는지 비교하는 데 사용
        previousMentalState = GameManager.instance.CurrentMentalState;

        // 말풍선 오브젝트가 연결되어 있다면
        if (bubbleObject != null)
        {
            // 말풍선의 원래 크기를 저장
            originalBubbleScale = bubbleObject.transform.localScale;

            // 게임 시작 시 말풍선은 보이지 않게 숨김
            bubbleObject.SetActive(false);
        }
    }

    void Update()
    {
        // 말풍선 오브젝트가 없으면 실행하지 않음
        if (bubbleObject == null)
            return;

        // 저장해 둔 원래 스케일 값을 가져옴
        Vector3 scale = originalBubbleScale;

        // 불씨 오브젝트가 왼쪽을 보려고 뒤집히면
        // 자식 오브젝트인 말풍선도 같이 뒤집히는 문제가 생김
        // 그래서 부모가 뒤집힌 경우 말풍선의 X 스케일을 반대로 보정함
        if (transform.lossyScale.x < 0)
            scale.x = -Mathf.Abs(originalBubbleScale.x);
        else
            scale.x = Mathf.Abs(originalBubbleScale.x);

        // 보정된 스케일을 말풍선에 적용
        // 결과적으로 불씨가 왼쪽을 봐도 글자는 정상 방향으로 보임
        bubbleObject.transform.localScale = scale;
    }

    // 정신력이 바뀔 때마다 실행되는 함수
    // Entity.OnMentalChanged 이벤트를 통해 호출됨
    private void CheckMental(float currentMental)
    {
        // 먼저 정신력 단계가 바뀌었는지 확인
        // 단계가 바뀌면 하단 경고 UI를 출력함
        CheckMentalStateWarning();

        // 현재 정신력이 이전 정신력보다 낮다면 정신력이 감소한 것
        if (currentMental < previousMental)
        {
            // 첫 정신력 감소라면 무조건 한 번 에이 말풍선을 출력
            if (!hasFirstAeiTalk)
            {
                ShowBubble(GetAeiLineByMentalState());

                // 첫 대사가 출력되었음을 기록
                hasFirstAeiTalk = true;

                // 마지막 말풍선 출력 정신력을 현재 정신력으로 저장
                lastAeiTalkMental = currentMental;
            }

            // 첫 대사 이후에는 정신력이 3 이상 더 줄어들었을 때만 출력
            // 예: 100에서 한 번 출력됐다면 97 이하가 되었을 때 다시 출력
            else if (currentMental <= lastAeiTalkMental - 3)
            {
                ShowBubble(GetAeiLineByMentalState());

                // 다시 출력한 시점의 정신력을 저장
                lastAeiTalkMental = currentMental;
            }
        }

        // 다음 정신력 변화와 비교하기 위해 현재 정신력을 저장
        previousMental = currentMental;
    }

    // 정신력 단계가 바뀌었는지 확인하고,
    // Depression, Anxiety, Collapse 단계 진입 시 하단 경고 UI를 출력하는 함수
    private void CheckMentalStateWarning()
    {
        // GameManager가 없으면 실행하지 않음
        if (GameManager.instance == null)
            return;

        // 현재 정신력 단계 가져오기
        MentalState currentState = GameManager.instance.CurrentMentalState;

        // 현재 단계와 이전 단계가 같으면 단계 변화가 없으므로 아무것도 하지 않음
        if (currentState == previousMentalState)
            return;

        // 단계가 바뀌었으므로 이전 단계 값을 현재 단계로 갱신
        previousMentalState = currentState;

        // 경고 UI가 연결되어 있지 않으면 실행하지 않음
        if (warningUI == null)
            return;

        // 새로 진입한 정신력 단계에 따라 다른 경고 대사를 출력
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

    // 하단 경고 UI에 경고 대사를 출력하는 함수
    private void ShowWarningDialogue(DialogueLine[] warningLines)
    {
        // 대사가 없으면 실행하지 않음
        if (warningLines == null || warningLines.Length == 0)
            return;

        // WarningMessageUI의 ShowWarning 함수를 호출해서
        // 화면 아래 전용 경고 패널에 문구를 표시함
        warningUI.ShowWarning(warningLines);
    }

    // 현재 정신력 단계에 맞는 에이 말풍선 대사를 가져오는 함수
    private string GetAeiLineByMentalState()
    {
        // GameManager가 없으면 기본 대사 반환
        if (GameManager.instance == null)
            return "괜찮아?";

        // 현재 정신력 단계에 따라 서로 다른 대사 배열에서 랜덤 선택
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

    // 문자열 배열에서 랜덤 대사 하나를 뽑아 반환하는 함수
    private string GetRandomLine(string[] lines)
    {
        // 배열이 비어 있으면 기본 대사 반환
        if (lines == null || lines.Length == 0)
            return "괜찮아?";

        // 0부터 배열 길이 전까지 랜덤 인덱스 선택
        int index = Random.Range(0, lines.Length);

        // 선택된 대사 반환
        return lines[index];
    }

    // 에이 옆 작은 말풍선을 출력하는 함수
    private void ShowBubble(string message)
    {
        Debug.Log("에이 말풍선 출력: " + message);

        // 텍스트나 말풍선 오브젝트가 연결되어 있지 않으면 실행하지 않음
        if (bubbleText == null || bubbleObject == null)
        {
            Debug.Log("말풍선 연결 안 됨");
            return;
        }

        // 말풍선에 표시할 문장 설정
        bubbleText.text = message;

        // 말풍선 오브젝트 활성화
        bubbleObject.SetActive(true);

        // 이전에 실행 중이던 숨김 코루틴이 있다면 중지
        // 대사가 새로 출력될 때 시간이 꼬이지 않게 하기 위함
        StopAllCoroutines();

        // 일정 시간 뒤 말풍선을 숨기는 코루틴 시작
        StartCoroutine(HideBubbleAfterTime());
    }

    // 일정 시간 후 에이 말풍선을 숨기는 코루틴
    private IEnumerator HideBubbleAfterTime()
    {
        // 2.5초 동안 대기
        yield return new WaitForSeconds(2.5f);

        // 말풍선 숨김
        bubbleObject.SetActive(false);
    }
}
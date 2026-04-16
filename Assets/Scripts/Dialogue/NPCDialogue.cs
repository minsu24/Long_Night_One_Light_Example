using UnityEngine;

// 대사 한 줄 구조 (화자 + 내용)
[System.Serializable]
public class DialogueLine
{
    public string speaker; // 말하는 사람 (NPC / Player)

    [TextArea(2, 5)]
    public string text; // 대사 내용
}

public class NPCDialogue : MonoBehaviour
{
    // 대화를 실행하는 매니저
    public DialogueManager dialogueManager;

    // [E] Talk UI 오브젝트
    public GameObject talkPrompt;

    // NPC 이름 (대화창에 표시됨)
    public string npcName;

    // 기본 대사 목록 (현재 사용하는 대화)
    [Header("기본 대화")]
    public DialogueLine[] dialogueLines;

    // ─── 정신력 기반 대화 시스템 ─────────────────────────────────────────────
    [Header("정신력 기반 대화 (멘탈 시스템)")]

    // 동일한 상황을 가리키는 공유 식별자
    public int situationId;

    // Normal_Table: 정상(70~100) 구간 대사
    [Tooltip("정신력 정상(70~100) 구간에 출력되는 객관적 대사")]
    public DialogueLine[] normalLines;

    // Anxiety_Table: 불안(30~49) 구간 대사
    [Tooltip("정신력 불안(30~49) 구간에 출력되는 대사")]
    public DialogueLine[] anxietyLines;

    // Depression_Table: 우울(50~69) 구간 대사
    [Tooltip("정신력 우울(50~69) 구간에 출력되는 대사")]
    public DialogueLine[] depressionLines;

    // Collapse_Table: 붕괴(0~29) 구간 대사
    [Tooltip("정신력 붕괴(0~29) 구간에 출력되는 대사")]
    public DialogueLine[] collapseLines;
    // ──────────────────────────────────────────────────────────────────────────

    // 선택지 사용 여부
    public bool hasChoice = false;

    // 몇 번째 대사 뒤에 선택지를 띄울지
    public int choiceTriggerLine = 4;

    // 선택지 1 문구
    public string choice1Label;

    // 선택지 2 문구
    public string choice2Label;

    // 선택지 1을 골랐을 때 나오는 대사 목록
    public DialogueLine[] choice1Lines;

    // 선택지 2를 골랐을 때 나오는 대사 목록
    public DialogueLine[] choice2Lines;

    // 플레이어가 범위 안에 있는지 확인
    private bool playerInRange = false;

    void Start()
    {
        if (talkPrompt != null)
            talkPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
            {
                DialogueLine[] selectedLines = SelectLinesByMentalState();

                if (selectedLines == null || selectedLines.Length == 0)
                    selectedLines = dialogueLines;

                dialogueManager.StartDialogue(selectedLines);

                if (talkPrompt != null)
                    talkPrompt.SetActive(false);
            }
        }
    }

    // 현재 정신력 상태에 따라 출력할 대사 테이블 선택
    private DialogueLine[] SelectLinesByMentalState()
    {
        if (GameManager.instance == null)
            return dialogueLines;

        switch (GameManager.instance.CurrentMentalState)
        {
            case MentalState.Collapse:
                if (collapseLines != null && collapseLines.Length > 0) return collapseLines;
                if (anxietyLines != null && anxietyLines.Length > 0) return anxietyLines;
                if (depressionLines != null && depressionLines.Length > 0) return depressionLines;
                if (normalLines != null && normalLines.Length > 0) return normalLines;
                return dialogueLines;

            case MentalState.Anxiety:
                if (anxietyLines != null && anxietyLines.Length > 0) return anxietyLines;
                if (normalLines != null && normalLines.Length > 0) return normalLines;
                return dialogueLines;

            case MentalState.Depression:
                if (depressionLines != null && depressionLines.Length > 0) return depressionLines;
                if (normalLines != null && normalLines.Length > 0) return normalLines;
                return dialogueLines;

            default: // Normal
                if (normalLines != null && normalLines.Length > 0) return normalLines;
                return dialogueLines;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (talkPrompt != null && dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
                talkPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (talkPrompt != null)
                talkPrompt.SetActive(false);
        }
    }

    public void ShowPromptAgain()
    {
        if (playerInRange && talkPrompt != null && dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
        {
            talkPrompt.SetActive(true);
        }
    }
}
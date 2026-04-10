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

    // NPC 대사 목록
    public DialogueLine[] dialogueLines;

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
        // 시작 시 안내 UI 비활성화
        if (talkPrompt != null)
            talkPrompt.SetActive(false);
    }

    void Update()
    {
        // 플레이어가 범위 안에 있고 E 키를 눌렀을 때
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 대화 중이 아니고, 입력 잠금 상태가 아닐 때만 실행
            if (dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
            {
                // 대화 시작
                dialogueManager.StartDialogue(dialogueLines);

                // 대화 시작 시 안내 UI 비활성화
                if (talkPrompt != null)
                    talkPrompt.SetActive(false);
            }
        }
    }

    // 플레이어가 범위 안으로 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player 태그인지 확인
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // 대화 중이 아니고 입력 잠금 상태가 아닐 때만 안내 UI 표시
            if (talkPrompt != null && dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
                talkPrompt.SetActive(true);
        }
    }

    // 플레이어가 범위를 벗어났을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // 안내 UI 비활성화
            if (talkPrompt != null)
                talkPrompt.SetActive(false);
        }
    }

    // 대화가 끝난 뒤, 플레이어가 아직 범위 안에 있으면 다시 안내 UI 표시
    public void ShowPromptAgain()
    {
        if (playerInRange && talkPrompt != null && dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
        {
            talkPrompt.SetActive(true);
        }
    }
}
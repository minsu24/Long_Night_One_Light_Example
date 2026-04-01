using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // 대화를 실행하는 매니저
    public DialogueManager dialogueManager;

    // [E] Talk UI 오브젝트
    public GameObject talkPrompt;

    // NPC 이름 (대화창에 표시됨)
    public string npcName;

    // NPC 대사 목록
    [TextArea(2, 5)]
    public string[] dialogueLines;

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
                dialogueManager.StartDialogue(npcName, dialogueLines);

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
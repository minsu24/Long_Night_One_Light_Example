using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager; // ⭐ 대화를 실행하는 매니저
    public string npcName;                  // ⭐ NPC 이름
    public string[] dialogueLines;          // ⭐ NPC 대사들

    public GameObject talkPrompt;           // ⭐ [E] 대화하기 표시 UI

    bool playerInRange = false;             // ⭐ 플레이어가 근처에 있는지 확인

    void Update()
    {
        // ⭐ 플레이어가 근처에 있고 E를 누르면 대화 시작
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            dialogueManager.StartDialogue(npcName, dialogueLines);

            // ⭐ 대화 시작하면 안내 문구 숨기기
            if (talkPrompt != null)
            {
                talkPrompt.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // ⭐ 플레이어가 가까이 오면 [E] 대화하기 표시
            if (talkPrompt != null)
            {
                talkPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // ⭐ 플레이어가 멀어지면 [E] 대화하기 숨기기
            if (talkPrompt != null)
            {
                talkPrompt.SetActive(false);
            }
        }
    }
}
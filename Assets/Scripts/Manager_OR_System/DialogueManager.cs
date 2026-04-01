using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;       // 대화창 전체
    public TextMeshProUGUI dialogueText;   // 대사 텍스트
    public TextMeshProUGUI nameText;       // NPC 이름 텍스트

    public GameObject choicePanel;         // 선택지 패널 전체
    public TextMeshProUGUI choice1Text;    // 선택지 1 텍스트
    public TextMeshProUGUI choice2Text;    // 선택지 2 텍스트

    public string[] lines;                 // 현재 대화 문장들
    int currentLine = 0;                   // 현재 몇 번째 줄인지
    public bool isDialogueActive = false;  // 대화 중인지 확인

    public float inputBlockTime = 0.15f;   // 대화 종료 후 입력 잠금 시간
    float inputBlockTimer = 0f;

    public float typingSpeed = 0.05f;      // 타자 출력 속도
    bool isTyping = false;                 // 현재 타이핑 중인지
    Coroutine typingCoroutine;             // 실행 중인 타이핑 코루틴 저장

    bool isChoiceActive = false;           // 지금 선택지 고르는 중인지 확인

    public AudioSource audioSource;        // 효과음을 재생할 AudioSource
    public AudioClip typingSound;          // 타자 효과음 파일

    NPCDialogue currentNPC;                // 현재 대화 중인 NPC 저장

    void Update()
    {
        // 대화 종료 직후 입력 잠금 시간 감소
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
        }

        // 선택지 입력 처리
        if (isChoiceActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("선택지 1 선택");
                HideChoices();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("선택지 2 선택");
                HideChoices();
            }

            return;
        }

        // 대화 중 E 입력 처리
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            // 아직 타이핑 중이면 전체 문장 즉시 출력
            if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                // 다음 줄로 이동
                currentLine++;

                if (currentLine < lines.Length)
                {
                    StartTyping(lines[currentLine]);

                    // 4번째 줄 이후 선택지 표시
                    if (currentLine == 3)
                    {
                        ShowChoices("Yes", "No");
                    }
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    // 대화 시작
    public void StartDialogue(string npcName, string[] newLines)
    {
        lines = newLines;
        currentLine = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);

        // NPC 이름 표시
        if (nameText != null)
        {
            nameText.text = npcName;
        }

        // 현재 대화 중인 NPC 찾기
        currentNPC = FindCurrentNPC();

        // 첫 줄부터 바로 타이핑 시작
        StartTyping(lines[currentLine]);
    }

    // 현재 플레이어와 접촉 중인 NPC 찾기
    NPCDialogue FindCurrentNPC()
    {
        NPCDialogue[] npcs = FindObjectsOfType<NPCDialogue>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return null;

        Collider2D playerCol = player.GetComponent<Collider2D>();

        if (playerCol == null)
            return null;

        foreach (NPCDialogue npc in npcs)
        {
            if (npc != null)
            {
                Collider2D npcCol = npc.GetComponent<Collider2D>();

                if (npcCol != null && npcCol.bounds.Intersects(playerCol.bounds))
                {
                    return npc;
                }
            }
        }

        return null;
    }

    // 선택지 표시 함수
    public void ShowChoices(string choice1, string choice2)
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }

        if (choice1Text != null)
        {
            choice1Text.text = "[1] " + choice1;
        }

        if (choice2Text != null)
        {
            choice2Text.text = "[2] " + choice2;
        }

        isChoiceActive = true;
    }

    // 선택지 숨기기 함수
    void HideChoices()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        isChoiceActive = false;
    }

    // 대화 종료
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        currentLine = 0;
        lines = null;
        dialogueText.text = "";

        HideChoices();

        inputBlockTimer = inputBlockTime;

        // 대화 끝난 뒤 플레이어가 아직 범위 안에 있으면 다시 [E] Talk 표시
        if (currentNPC != null)
        {
            currentNPC.ShowPromptAgain();
        }
    }

    // 플레이어 입력 잠금 여부 확인
    public bool IsInputBlocked()
    {
        return isDialogueActive || inputBlockTimer > 0f;
    }

    // 한 줄 타이핑 시작
    void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    // 한 글자씩 출력
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;

            // 공백이 아닐 때만 타자 효과음 재생
            if (c != ' ' && audioSource != null && typingSound != null)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // 타이핑 중 E 누르면 전체 문장 즉시 표시
    void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = lines[currentLine];
        isTyping = false;
    }
}
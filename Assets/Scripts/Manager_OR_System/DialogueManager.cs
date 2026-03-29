using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;       // ⭐ 대화창 전체
    public TextMeshProUGUI dialogueText;   // ⭐ 대사 텍스트
    public TextMeshProUGUI nameText;       // ⭐ NPC 이름 텍스트

    // ⭐⭐ [추가] 선택지 UI 관련 변수
    public GameObject choicePanel;         // 선택지 패널 전체
    public TextMeshProUGUI choice1Text;    // 선택지 1 텍스트
    public TextMeshProUGUI choice2Text;    // 선택지 2 텍스트

    public string[] lines;                 // ⭐ 현재 대화 문장들
    int currentLine = 0;                   // ⭐ 현재 몇 번째 줄인지
    public bool isDialogueActive = false;  // ⭐ 대화 중인지 확인

    public float inputBlockTime = 0.15f;   // ⭐ 대화 종료 후 입력 잠금 시간
    float inputBlockTimer = 0f;

    public float typingSpeed = 0.05f;      // ⭐ 타자 출력 속도
    bool isTyping = false;                 // ⭐ 현재 타이핑 중인지
    Coroutine typingCoroutine;             // ⭐ 실행 중인 타이핑 코루틴 저장

    // ⭐⭐ [추가] 선택지 활성화 여부
    bool isChoiceActive = false;           // 지금 선택지 고르는 중인지 확인

    // ⭐⭐ [추가] 타자 효과음 관련 변수
    public AudioSource audioSource;        // 효과음을 재생할 AudioSource
    public AudioClip typingSound;          // 타자 효과음 파일

    void Update()
    {
        // ⭐ 대화 종료 직후 입력 잠금 시간 감소
        if (inputBlockTimer > 0f)
        {
            inputBlockTimer -= Time.deltaTime;
        }

        // ⭐⭐ [추가] 선택지 입력 처리
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

            return; // ⭐ 선택지 고르는 중에는 일반 대화 입력 막기
        }

        // ⭐ 대화 중 Space 입력 처리
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            // ⭐ 아직 타이핑 중이면 전체 문장 즉시 출력
            if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                // ⭐ 다음 줄로 이동
                currentLine++;

                if (currentLine < lines.Length)
                {
                    StartTyping(lines[currentLine]);

                    // ⭐ Element 3 (Would you help me?) 다음에 선택지 등장
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

    // ⭐ 대화 시작
    public void StartDialogue(string npcName, string[] newLines)
    {
        lines = newLines;
        currentLine = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);

        // ⭐ NPC 이름 표시
        if (nameText != null)
        {
            nameText.text = npcName;
        }

        // ⭐ 첫 줄부터 바로 타이핑 시작
        StartTyping(lines[currentLine]);
    }

    // ⭐⭐ [추가] 선택지 표시 함수
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

    // ⭐⭐ [추가] 선택지 숨기기 함수
    void HideChoices()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        isChoiceActive = false;
    }

    // ⭐ 대화 종료
    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        currentLine = 0;          // ⭐ 다음 대화를 위해 줄 번호 초기화
        lines = null;             // ⭐ 대사 배열 초기화
        dialogueText.text = "";   // ⭐ 대사창 비우기

        // ⭐⭐ [추가] 대화 종료 시 선택지도 같이 숨기기
        HideChoices();

        inputBlockTimer = inputBlockTime;
    }

    // ⭐ 플레이어 입력 잠금 여부 확인
    public bool IsInputBlocked()
    {
        return isDialogueActive || inputBlockTimer > 0f;
    }

    // ⭐ 한 줄 타이핑 시작
    void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    // ⭐ 한 글자씩 출력
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;

            // ⭐⭐ [추가] 공백이 아닐 때만 타자 효과음 재생
            if (c != ' ' && audioSource != null && typingSound != null)
            {
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // ⭐ 타이핑 중 Space 누르면 전체 문장 즉시 표시
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
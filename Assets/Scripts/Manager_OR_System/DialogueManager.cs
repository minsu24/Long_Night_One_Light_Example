using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    void Awake()
    {
        instance = this;
    }

    public GameObject dialoguePanel;       // 대화창 전체
    public TextMeshProUGUI dialogueText;   // 대사 텍스트
    public TextMeshProUGUI nameText;       // NPC 이름 텍스트

    public GameObject choicePanel;         // 선택지 패널 전체
    public TextMeshProUGUI choice1Text;    // 선택지 1 텍스트
    public TextMeshProUGUI choice2Text;    // 선택지 2 텍스트

    public DialogueLine[] lines;           // 현재 대화 문장들
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
    NPCDialogue activeNPC;                 // 선택지용 NPC 저장

    bool justStartedDialogue = false;      // 대화 시작 직후 E 키 유지 입력 무시용

    private bool isMadnessMode = false;

    Coroutine warningCoroutine;            // 실행 중인 경고문구 코루틴 저장

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

                if (activeNPC != null && activeNPC.choice1Lines != null && activeNPC.choice1Lines.Length > 0)
                {
                    HideChoices();

                    lines = activeNPC.choice1Lines;
                    currentLine = 0;
                    StartTyping(lines[currentLine]);
                }
                else
                {
                    HideChoices();
                    EndDialogue();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("선택지 2 선택");

                if (activeNPC != null && activeNPC.choice2Lines != null && activeNPC.choice2Lines.Length > 0)
                {
                    HideChoices();

                    lines = activeNPC.choice2Lines;
                    currentLine = 0;
                    StartTyping(lines[currentLine]);
                }
                else
                {
                    HideChoices();
                    EndDialogue();
                }
            }

            return;
        }

        // 대화 시작에 사용된 E 키가 아직 눌린 상태면 입력 무시
        if (justStartedDialogue)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                justStartedDialogue = false;
            }
            else
            {
                return;
            }
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
                // 현재 줄에서 선택지를 띄워야 하면 먼저 선택지 표시
                if (activeNPC != null && activeNPC.hasChoice && currentLine == activeNPC.choiceTriggerLine)
                {
                    ShowChoices(activeNPC.choice1Label, activeNPC.choice2Label);
                    return;
                }

                // 다음 줄로 이동
                currentLine++;

                if (lines != null && currentLine < lines.Length)
                {
                    StartTyping(lines[currentLine]);
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    // 대화 시작
    public void StartDialogue(DialogueLine[] newLines)
    {
        // 이전 경고문구 코루틴이 남아 있으면 정리
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        // 이전 타이핑 코루틴이 남아 있으면 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        lines = newLines;
        currentLine = 0;
        isDialogueActive = true;
        justStartedDialogue = true;
        isTyping = false;
        isChoiceActive = false;
        dialogueText.text = "";

        dialoguePanel.SetActive(true);

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // 현재 대화 중인 NPC 찾기
        currentNPC = FindCurrentNPC();
        activeNPC = currentNPC;

        // 첫 줄부터 바로 타이핑 시작
        if (lines != null && lines.Length > 0)
        {
            StartTyping(lines[currentLine]);
        }
    }

    // 게임 진행을 막지 않는 경고문구 표시
    // 정신력 하락 경고처럼 플레이어가 움직이면서 봐야 하는 문구에 사용
    public void ShowWarningMessage(DialogueLine[] warningLines, float duration = 3f)
    {
        if (warningLines == null || warningLines.Length == 0)
            return;

        // 일반 NPC 대화 중이면 경고문구가 대화를 덮어쓰지 않도록 중단
        if (isDialogueActive)
            return;

        // 이전 경고문구 코루틴이 남아 있으면 정리
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        // 이전 타이핑 코루틴이 남아 있으면 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // 입력 차단용 변수는 건드리지 않음
        // 즉, 경고문구가 떠도 이동/점프 가능
        isTyping = false;
        isChoiceActive = false;
        justStartedDialogue = false;

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        dialoguePanel.SetActive(true);

        if (nameText != null)
        {
            nameText.text = warningLines[0].speaker;
        }

        if (dialogueText != null)
        {
            dialogueText.text = warningLines[0].text;
        }

        warningCoroutine = StartCoroutine(HideWarningAfterTime(duration));
    }

    // 일정 시간 후 경고문구 숨김
    IEnumerator HideWarningAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 일반 NPC 대화가 시작되지 않았을 때만 경고문구 숨김
        if (!isDialogueActive)
        {
            dialoguePanel.SetActive(false);

            if (dialogueText != null)
            {
                dialogueText.text = "";
            }

            if (nameText != null)
            {
                nameText.text = "";
            }
        }

        warningCoroutine = null;
    }

    // 현재 플레이어와 접촉 중인 NPC 찾기
    NPCDialogue FindCurrentNPC()
    {
        NPCDialogue[] npcs = FindObjectsByType<NPCDialogue>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
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
        // 타이핑 코루틴이 남아 있으면 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        isChoiceActive = false;
        justStartedDialogue = false;

        dialoguePanel.SetActive(false);

        currentLine = 0;
        lines = null;
        dialogueText.text = "";

        if (nameText != null)
        {
            nameText.text = "";
        }

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
    void StartTyping(DialogueLine line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";
        isTyping = false;

        // 현재 줄의 화자 이름 표시
        if (nameText != null)
        {
            nameText.text = line.speaker;
        }

        typingCoroutine = StartCoroutine(TypeLine(line.text));
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
        typingCoroutine = null;
    }

    // 타이핑 중 E 누르면 전체 문장 즉시 표시
    void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (nameText != null && lines != null && currentLine < lines.Length)
        {
            nameText.text = lines[currentLine].speaker;
        }

        if (lines != null && currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine].text;
        }

        isTyping = false;
    }

    public void SetMadnessMode(bool isMadness)
    {
        isMadnessMode = isMadness;
    }
}
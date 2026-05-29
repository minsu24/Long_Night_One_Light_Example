using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;

    [TextArea(2, 5)]
    public string text;
}

public class NPCDialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject talkPrompt;
    public string npcName;

    [Header("플레이어 감지")]
    public Transform player;
    public float talkRange = 2f;

    [Header("기본 대화")]
    public DialogueLine[] dialogueLines;

    [Header("정신력 기반 대화 (멘탈 시스템)")]
    public int situationId;
    public DialogueLine[] normalLines;
    public DialogueLine[] anxietyLines;
    public DialogueLine[] depressionLines;
    public DialogueLine[] collapseLines;

    public bool hasChoice = false;
    public int choiceTriggerLine = 4;
    public string choice1Label;
    public string choice2Label;
    public DialogueLine[] choice1Lines;
    public DialogueLine[] choice2Lines;

    private bool playerInRange = false;

    void Start()
    {
        if (talkPrompt != null)
            talkPrompt.SetActive(false);

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Update()
    {
        CheckPlayerDistance();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E 눌림 / playerInRange = " + playerInRange);
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("대화 시도");

            if (dialogueManager == null)
            {
                Debug.LogWarning("DialogueManager 체크 / dialogueManager가 null임");
                return;
            }

            Debug.Log(
                "DialogueManager 체크 / " +
                "isDialogueActive = " + dialogueManager.isDialogueActive +
                " / IsInputBlocked = " + dialogueManager.IsInputBlocked()
            );

            if (!dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
            {
                DialogueLine[] selectedLines = SelectLinesByMentalState();

                if (selectedLines == null || selectedLines.Length == 0)
                    selectedLines = dialogueLines;

                if (selectedLines == null || selectedLines.Length == 0)
                {
                    Debug.LogWarning("출력할 대사가 없음");
                    return;
                }

                dialogueManager.StartDialogue(selectedLines);

                if (talkPrompt != null)
                    talkPrompt.SetActive(false);
            }
            else
            {
                Debug.LogWarning(
                    "대화 시작 실패 / " +
                    "isDialogueActive = " + dialogueManager.isDialogueActive +
                    " / IsInputBlocked = " + dialogueManager.IsInputBlocked()
                );
            }
        }
    }

    private void CheckPlayerDistance()
    {
        if (player == null)
        {
            playerInRange = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= talkRange;

        if (talkPrompt != null && dialogueManager != null)
        {
            bool canShowPrompt = playerInRange && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked();
            talkPrompt.SetActive(canShowPrompt);
        }
    }

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

            default:
                if (normalLines != null && normalLines.Length > 0) return normalLines;
                return dialogueLines;
        }
    }

    public void ShowPromptAgain()
    {
        if (playerInRange && talkPrompt != null && dialogueManager != null && !dialogueManager.isDialogueActive && !dialogueManager.IsInputBlocked())
            talkPrompt.SetActive(true);
    }
}
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ForceEndDialogue();
    }

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    public GameObject choicePanel;
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;

    public DialogueLine[] lines;
    int currentLine = 0;
    public bool isDialogueActive = false;

    public float inputBlockTime = 0.15f;
    float inputBlockTimer = 0f;

    public float typingSpeed = 0.05f;
    bool isTyping = false;
    Coroutine typingCoroutine;

    bool isChoiceActive = false;

    public AudioSource audioSource;
    public AudioClip typingSound;

    NPCDialogue currentNPC;
    NPCDialogue activeNPC;

    bool justStartedDialogue = false;
    private bool isMadnessMode = false;
    Coroutine warningCoroutine;

    void Update()
    {
        if (inputBlockTimer > 0f)
            inputBlockTimer -= Time.unscaledDeltaTime;

        if (isChoiceActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
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

        if (justStartedDialogue)
        {
            if (!Input.GetKey(KeyCode.E))
                justStartedDialogue = false;
            else
                return;
        }

        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                CompleteLine();
            }
            else
            {
                if (activeNPC != null && activeNPC.hasChoice && currentLine == activeNPC.choiceTriggerLine)
                {
                    ShowChoices(activeNPC.choice1Label, activeNPC.choice2Label);
                    return;
                }

                currentLine++;

                if (lines != null && currentLine < lines.Length)
                    StartTyping(lines[currentLine]);
                else
                    EndDialogue();
            }
        }
    }

    public void StartDialogue(DialogueLine[] newLines)
    {
        Time.timeScale = 0f;

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

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

        if (dialogueText != null)
            dialogueText.text = "";

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        currentNPC = FindCurrentNPC();
        activeNPC = currentNPC;

        if (lines != null && lines.Length > 0)
            StartTyping(lines[currentLine]);
    }

    public void ForceEndDialogue()
    {
        Time.timeScale = 1f;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        isChoiceActive = false;
        justStartedDialogue = false;
        inputBlockTimer = 0f;

        currentLine = 0;
        lines = null;
        currentNPC = null;
        activeNPC = null;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        if (nameText != null)
            nameText.text = "";
    }

    public void ShowWarningMessage(DialogueLine[] warningLines, float duration = 3f)
    {
        if (warningLines == null || warningLines.Length == 0)
            return;

        if (isDialogueActive)
            return;

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        isChoiceActive = false;
        justStartedDialogue = false;

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = warningLines[0].speaker;

        if (dialogueText != null)
            dialogueText.text = warningLines[0].text;

        warningCoroutine = StartCoroutine(HideWarningAfterTime(duration));
    }

    IEnumerator HideWarningAfterTime(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (!isDialogueActive)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (dialogueText != null)
                dialogueText.text = "";

            if (nameText != null)
                nameText.text = "";
        }

        warningCoroutine = null;
    }

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
                    return npc;
            }
        }

        return null;
    }

    public void ShowChoices(string choice1, string choice2)
    {
        if (choicePanel != null)
            choicePanel.SetActive(true);

        if (choice1Text != null)
            choice1Text.text = "[1] " + choice1;

        if (choice2Text != null)
            choice2Text.text = "[2] " + choice2;

        isChoiceActive = true;
    }

    void HideChoices()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);

        isChoiceActive = false;
    }

    void EndDialogue()
    {
        Time.timeScale = 1f;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        isChoiceActive = false;
        justStartedDialogue = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        currentLine = 0;
        lines = null;

        if (dialogueText != null)
            dialogueText.text = "";

        if (nameText != null)
            nameText.text = "";

        HideChoices();

        inputBlockTimer = inputBlockTime;

        if (currentNPC != null)
            currentNPC.ShowPromptAgain();
    }

    public bool IsInputBlocked()
    {
        return isDialogueActive || inputBlockTimer > 0f;
    }

    void StartTyping(DialogueLine line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
            dialogueText.text = "";

        isTyping = false;

        if (nameText != null)
            nameText.text = line.speaker;

        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char c in line)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            if (c != ' ' && audioSource != null && typingSound != null)
                audioSource.PlayOneShot(typingSound);

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    void CompleteLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (nameText != null && lines != null && currentLine < lines.Length)
            nameText.text = lines[currentLine].speaker;

        if (dialogueText != null && lines != null && currentLine < lines.Length)
            dialogueText.text = lines[currentLine].text;

        isTyping = false;
    }

    public void SetMadnessMode(bool isMadness)
    {
        isMadnessMode = isMadness;
    }
}
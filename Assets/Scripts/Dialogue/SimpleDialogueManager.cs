using UnityEngine;
using TMPro;

public class SimpleDialogueManager : MonoBehaviour
{
    public static SimpleDialogueManager instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private DialogueLine[] lines;
    private int currentLine;
    private bool isActive;
    private float blockTimer;

    public bool IsActive => isActive;
    public bool IsBlocked => blockTimer > 0f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RefreshUI();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (blockTimer > 0f)
            blockTimer -= Time.deltaTime;

        if (!isActive)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            NextLine();
    }

    public void StartDialogue(DialogueLine[] newLines)
    {
        RefreshUI();

        if (isActive || IsBlocked)
            return;

        if (newLines == null || newLines.Length == 0)
            return;

        lines = newLines;
        currentLine = 0;
        isActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowLine();
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine < lines.Length)
            ShowLine();
        else
            EndDialogue();
    }

    void ShowLine()
    {
        if (nameText != null)
            nameText.text = lines[currentLine].speaker;

        if (dialogueText != null)
            dialogueText.text = lines[currentLine].text;
    }

    void EndDialogue()
    {
        isActive = false;
        blockTimer = 0.25f;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (nameText != null)
            nameText.text = "";

        if (dialogueText != null)
            dialogueText.text = "";
    }

    void RefreshUI()
    {
        if (dialoguePanel == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "DialoguePanel" && obj.scene.IsValid())
                {
                    dialoguePanel = obj;
                    break;
                }
            }
        }

        if (dialoguePanel == null)
            return;

        if (dialogueText == null)
        {
            Transform textObj = dialoguePanel.transform.Find("DialogueText");

            if (textObj != null)
                dialogueText = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (nameText == null)
        {
            Transform nameObj = dialoguePanel.transform.Find("NameText");

            if (nameObj != null)
                nameText = nameObj.GetComponent<TextMeshProUGUI>();
        }
    }
}
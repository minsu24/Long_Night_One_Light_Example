using UnityEngine;

public class SimpleNPCDialogue : MonoBehaviour
{
    public Transform player;
    public float talkRange = 2f;

    private bool talked = false;

    private DialogueLine[] dialogueLines =
    {
        new DialogueLine { speaker = "마을 주민", text = "거기... 잠깐만요!" },
        new DialogueLine { speaker = "마을 주민", text = "그 불빛은..." },
        new DialogueLine { speaker = "마을 주민", text = "이런 시대에 아직도 저렇게 밝은 빛을 지닌 분이 계셨군요." },
        new DialogueLine { speaker = "마을 주민", text = "실례지만 부탁 하나만 들어주실 수 있을까요?" },
        new DialogueLine { speaker = "마을 주민", text = "숲 깊은 곳에서 어둠이 점점 짙어지고 있습니다." },
        new DialogueLine { speaker = "마을 주민", text = "최근에는 그들을 이끄는 거대한 존재까지 나타났습니다." },
        new DialogueLine { speaker = "마을 주민", text = "녀석이 나타난 뒤로 숲은 점점 죽어가고 있어요." },
        new DialogueLine { speaker = "마을 주민", text = "부디 숲의 중심으로 가서 어둠의 근원을 끊어 주세요." },

        new DialogueLine { speaker = "에이", text = "실라..." },
        new DialogueLine { speaker = "에이", text = "저 어둠의 냄새, 나도 느껴져." },
        new DialogueLine { speaker = "에이", text = "분명 평범한 몬스터는 아니야." },
        new DialogueLine { speaker = "에이", text = "불씨를 되살리려면 결국 저런 어둠과 마주해야 해." },
        new DialogueLine { speaker = "에이", text = "무섭더라도 앞으로 나아가자." },

        new DialogueLine { speaker = "안내", text = "숲 깊은 곳으로 향해 어둠에 잠식된 괴물들을 정화하고, 숲의 중심에 자리한 어둠의 군주를 처치하세요." }
    };

    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }

    void Update()
    {
        if (talked)
            return;

        if (player == null)
            return;

        if (SimpleDialogueManager.instance == null)
            return;

        if (SimpleDialogueManager.instance.IsActive || SimpleDialogueManager.instance.IsBlocked)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= talkRange && Input.GetKeyDown(KeyCode.E))
        {
            talked = true;
            SimpleDialogueManager.instance.StartDialogue(dialogueLines);
        }
    }
}
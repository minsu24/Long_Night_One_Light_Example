using UnityEngine;

public class SimpleNPCDialogue : MonoBehaviour
{
    public Transform player;
    public float talkRange = 2f;

    // 정상 상태 대사
    private DialogueLine[] normalDialogue =
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

    // 정신력 불안(Anxiety)
    private DialogueLine[] anxietyDialogue =
    {
        new DialogueLine { speaker = "마을 주민", text = "잠깐만요... 또 숲으로 가시려는 건가요?" },
        new DialogueLine { speaker = "마을 주민", text = "요즘 마을 사람들 사이에서 이상한 말이 돌고 있습니다." },
        new DialogueLine { speaker = "마을 주민", text = "당신이 나타난 뒤로 숲의 어둠이 더 짙어진 것 같다고요." },
        new DialogueLine { speaker = "마을 주민", text = "물론 저도 그런 말을 그대로 믿고 싶지는 않습니다." },
        new DialogueLine { speaker = "마을 주민", text = "하지만 마을의 불빛은 점점 약해지고, 밤마다 괴물들의 울음소리는 가까워지고 있어요." },
        new DialogueLine { speaker = "마을 주민", text = "당신의 불씨가 우리를 구원할 빛인지, 아니면 어둠을 자극하는 불씨인지..." },
        new DialogueLine { speaker = "마을 주민", text = "이제는 저도 확신할 수 없습니다." },
        new DialogueLine { speaker = "마을 주민", text = "부디 증명해 주세요." },
        new DialogueLine { speaker = "마을 주민", text = "당신이 이 마을을 위험하게 만든 사람이 아니라, 정말 구하러 온 사람이라는 것을요." }
    };

    // 정신력 붕괴(Collapse)
    private DialogueLine[] collapseDialogue =
    {
        new DialogueLine { speaker = "마을 주민", text = "이제야 알겠습니다." },
        new DialogueLine { speaker = "마을 주민", text = "처음부터 당신이 문제였던 겁니다." },
        new DialogueLine { speaker = "마을 주민", text = "당신이 이 마을에 나타난 뒤로 모든 것이 망가지기 시작했어요." },
        new DialogueLine { speaker = "마을 주민", text = "숲은 더 깊은 어둠에 잠겼고, 괴물들은 전보다 훨씬 난폭해졌습니다." },
        new DialogueLine { speaker = "마을 주민", text = "우리는 당신의 불빛이 희망이라고 믿었습니다." },
        new DialogueLine { speaker = "마을 주민", text = "하지만 아니었어요." },
        new DialogueLine { speaker = "마을 주민", text = "그 빛은 우리를 구하러 온 게 아니라, 더 큰 어둠을 불러온 신호였던 겁니다." },
        new DialogueLine { speaker = "마을 주민", text = "제발... 더는 우리를 돕겠다고 말하지 마세요." },
        new DialogueLine { speaker = "마을 주민", text = "당신이 움직일수록, 우리가 잃는 것만 늘어나고 있으니까요." },
        new DialogueLine { speaker = "마을 주민", text = "정말 미안하지만..." },
        new DialogueLine { speaker = "마을 주민", text = "이 마을을 위한다면, 차라리 아무것도 하지 말아 주세요." }
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
        if (player == null)
            return;

        if (SimpleDialogueManager.instance == null)
            return;

        if (SimpleDialogueManager.instance.IsActive || SimpleDialogueManager.instance.IsBlocked)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= talkRange && Input.GetKeyDown(KeyCode.E))
        {
            MentalState state = GameManager.instance.CurrentMentalState;

            if (state == MentalState.Collapse)
            {
                SimpleDialogueManager.instance.StartDialogue(collapseDialogue);
            }
            else if (state == MentalState.Anxiety)
            {
                SimpleDialogueManager.instance.StartDialogue(anxietyDialogue);
            }
            else
            {
                SimpleDialogueManager.instance.StartDialogue(normalDialogue);
            }
        }
    }
}
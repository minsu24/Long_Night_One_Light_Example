using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    private bool waitingForIntroDialogueEnd = false;
    private bool waitingForMoveCheck = false;

    private bool waitingForMoveSuccessDialogueEnd = false;
    private bool waitingForJumpStart = false;
    private bool waitingForJumpLanding = false;

    private bool waitingForDoubleJumpDialogueEnd = false;
    private bool waitingForDoubleJumpStart = false;
    private bool waitingForDoubleJumpLanding = false;

    private bool waitingForAttackDialogueEnd = false;
    private bool waitingForAttackInput = false;

    private bool waitingForChargeAttackDialogueEnd = false;
    private bool waitingForChargeAttackInput = false;

    private bool waitingForSkillDialogueEnd = false;
    private bool waitingForSkillInput = false;

    private bool waitingForDashDialogueEnd = false;
    private bool waitingForDashInput = false;

    private bool waitingForSpeedUpDialogueEnd = false;
    private bool waitingForSpeedUpInput = false;

    private bool waitingForInventoryDialogueEnd = false;
    private bool waitingForInventoryInput = false;

    private bool waitingForMapDialogueEnd = false;
    private bool waitingForMapInput = false;

    private Vector3 startPlayerPosition;
    private float jumpStartY;
    private float doubleJumpStartY;

    [Header("플레이어 오브젝트")]
    public Transform player;

    [Header("이동 성공으로 인정할 거리")]
    public float moveCheckDistance = 5f;

    [Header("점프로 인정할 높이")]
    public float jumpHeightCheck = 0.2f;

    [Header("2단 점프로 인정할 높이")]
    public float doubleJumpHeightCheck = 0.4f;

    [Header("공격 후 대화가 뜨기까지 기다릴 시간")]
    public float attackDelay = 1.5f;

    [Header("스킬 후 대화가 뜨기까지 기다릴 시간")]
    public float skillDelay = 1.5f;

    [Header("대쉬 후 대화가 뜨기까지 기다릴 시간")]
    public float dashDelay = 1.5f;

    [Header("이속증가 후 대화가 뜨기까지 기다릴 시간")]
    public float speedUpDelay = 1.5f;

    [Header("인벤토리/맵 후 대화가 뜨기까지 기다릴 시간")]
    public float uiDelay = 0.5f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ShowIntroDialogue();
    }

    void Update()
    {
        if (waitingForIntroDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForIntroDialogueEnd = false;

                if (player != null)
                {
                    startPlayerPosition = player.position;
                    waitingForMoveCheck = true;
                }
            }

            return;
        }

        if (waitingForMoveCheck)
        {
            if (player == null)
                return;

            float movedDistance = Vector3.Distance(startPlayerPosition, player.position);

            if (movedDistance >= moveCheckDistance)
            {
                waitingForMoveCheck = false;
                ShowMoveSuccessDialogue();
            }

            return;
        }

        if (waitingForMoveSuccessDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForMoveSuccessDialogueEnd = false;

                if (player != null)
                {
                    jumpStartY = player.position.y;
                    waitingForJumpStart = true;
                }
            }

            return;
        }

        if (waitingForJumpStart)
        {
            if (player == null)
                return;

            if (player.position.y > jumpStartY + jumpHeightCheck)
            {
                waitingForJumpStart = false;
                waitingForJumpLanding = true;
            }

            return;
        }

        if (waitingForJumpLanding)
        {
            if (player == null)
                return;

            if (player.position.y <= jumpStartY + 0.05f)
            {
                waitingForJumpLanding = false;
                ShowJumpSuccessDialogue();
            }

            return;
        }

        if (waitingForDoubleJumpDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForDoubleJumpDialogueEnd = false;

                if (player != null)
                {
                    doubleJumpStartY = player.position.y;
                    waitingForDoubleJumpStart = true;
                }
            }

            return;
        }

        if (waitingForDoubleJumpStart)
        {
            if (player == null)
                return;

            if (player.position.y > doubleJumpStartY + doubleJumpHeightCheck)
            {
                waitingForDoubleJumpStart = false;
                waitingForDoubleJumpLanding = true;
            }

            return;
        }

        if (waitingForDoubleJumpLanding)
        {
            if (player == null)
                return;

            if (player.position.y <= doubleJumpStartY + 0.05f)
            {
                waitingForDoubleJumpLanding = false;
                ShowDoubleJumpSuccessDialogue();
            }

            return;
        }

        if (waitingForAttackDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForAttackDialogueEnd = false;
                waitingForAttackInput = true;
            }

            return;
        }

        if (waitingForAttackInput)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                waitingForAttackInput = false;
                StartCoroutine(ShowAttackSuccessAfterDelay());
            }

            return;
        }

        if (waitingForChargeAttackDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForChargeAttackDialogueEnd = false;
                waitingForChargeAttackInput = true;
            }

            return;
        }

        if (waitingForSkillDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForSkillDialogueEnd = false;
                waitingForSkillInput = true;
            }

            return;
        }

        if (waitingForSkillInput)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                waitingForSkillInput = false;
                StartCoroutine(ShowSkillSuccessAfterDelay());
            }

            return;
        }

        if (waitingForDashDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForDashDialogueEnd = false;
                waitingForDashInput = true;
            }

            return;
        }

        if (waitingForDashInput)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                waitingForDashInput = false;
                StartCoroutine(ShowDashSuccessAfterDelay());
            }

            return;
        }

        if (waitingForSpeedUpDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForSpeedUpDialogueEnd = false;
                waitingForSpeedUpInput = true;
            }

            return;
        }

        if (waitingForSpeedUpInput)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                waitingForSpeedUpInput = false;
                StartCoroutine(ShowSpeedUpSuccessAfterDelay());
            }

            return;
        }

        if (waitingForInventoryDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForInventoryDialogueEnd = false;
                waitingForInventoryInput = true;
            }

            return;
        }

        if (waitingForInventoryInput)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                waitingForInventoryInput = false;
                StartCoroutine(ShowInventorySuccessAfterDelay());
            }

            return;
        }

        if (waitingForMapDialogueEnd)
        {
            if (DialogueManager.instance != null && !DialogueManager.instance.isDialogueActive)
            {
                waitingForMapDialogueEnd = false;
                waitingForMapInput = true;
            }

            return;
        }

        if (waitingForMapInput)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                waitingForMapInput = false;
                StartCoroutine(ShowMapSuccessAfterDelay());
            }

            return;
        }
    }

    IEnumerator ShowAttackSuccessAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        ShowAttackSuccessDialogue();
    }

    IEnumerator ShowChargeAttackSuccessAfterDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        ShowChargeAttackSuccessDialogue();
    }

    IEnumerator ShowSkillSuccessAfterDelay()
    {
        yield return new WaitForSeconds(skillDelay);
        ShowSkillSuccessDialogue();
    }

    IEnumerator ShowDashSuccessAfterDelay()
    {
        yield return new WaitForSeconds(dashDelay);
        ShowDashSuccessDialogue();
    }

    IEnumerator ShowSpeedUpSuccessAfterDelay()
    {
        yield return new WaitForSeconds(speedUpDelay);
        ShowSpeedUpSuccessDialogue();
    }

    IEnumerator ShowInventorySuccessAfterDelay()
    {
        yield return new WaitForSeconds(uiDelay);
        ShowInventorySuccessDialogue();
    }

    IEnumerator ShowMapSuccessAfterDelay()
    {
        yield return new WaitForSeconds(uiDelay);
        ShowMapSuccessDialogue();
    }

    public void OnChargeAttackUsed()
    {
        if (!waitingForChargeAttackInput)
            return;

        waitingForChargeAttackInput = false;
        StartCoroutine(ShowChargeAttackSuccessAfterDelay());
    }

    void ShowIntroDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "실라!" },
            new DialogueLine { speaker = "에이", text = "드디어 일어났구나!" },
            new DialogueLine { speaker = "에이", text = "정말 걱정했잖아..." },
            new DialogueLine { speaker = "에이", text = "몸은 괜찮아?" },
            new DialogueLine { speaker = "에이", text = "오래 잠들어 있었으니까... 먼저 조금 움직여 보자!" },
            new DialogueLine { speaker = "안내", text = "← → 방향키로 이동할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForIntroDialogueEnd = true;
    }

    void ShowMoveSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "오!" },
            new DialogueLine { speaker = "에이", text = "좋아 좋아!" },
            new DialogueLine { speaker = "에이", text = "아직 몸은 기억하고 있는 것 같네!" },
            new DialogueLine { speaker = "에이", text = "이번엔 한번 뛰어올라 봐!" },
            new DialogueLine { speaker = "안내", text = "Space 키로 점프할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForMoveSuccessDialogueEnd = true;
    }

    void ShowJumpSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "그거야!" },
            new DialogueLine { speaker = "에이", text = "역시 실라라니까!" },
            new DialogueLine { speaker = "에이", text = "그리고 아직 숨겨둔 힘도 남아있어." },
            new DialogueLine { speaker = "에이", text = "공중에서도 한 번 더 도약할 수 있거든!" },
            new DialogueLine { speaker = "안내", text = "점프 중 Space를 다시 눌러 2단 점프를 사용할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForDoubleJumpDialogueEnd = true;
    }

    void ShowDoubleJumpSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "와아!" },
            new DialogueLine { speaker = "에이", text = "맞아 맞아!" },
            new DialogueLine { speaker = "에이", text = "그 힘이야!" },
            new DialogueLine { speaker = "에이", text = "이 정도면 여행을 시작할 수 있겠어!" },
            new DialogueLine { speaker = "에이", text = "하지만 조심해야 해..." },
            new DialogueLine { speaker = "에이", text = "어둠이 남긴 것들이 아직 이곳에 있을지도 몰라." },
            new DialogueLine { speaker = "에이", text = "혹시 앞을 막는 게 나타나면, 힘을 뻗어 밀어내야 해!" },
            new DialogueLine { speaker = "안내", text = "A 키로 기본 공격을 사용할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForAttackDialogueEnd = true;
    }

    void ShowAttackSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "좋아!" },
            new DialogueLine { speaker = "에이", text = "방금 그게 기본 공격이야!" },
            new DialogueLine { speaker = "에이", text = "아직 약하지만... 실라의 빛은 분명 남아 있어!" },
            new DialogueLine { speaker = "에이", text = "그리고 A 키를 길게 누르면 힘을 더 모을 수 있어!" },
            new DialogueLine { speaker = "안내", text = "A 키를 꾹 눌러 차징 공격을 사용해 보세요." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForChargeAttackDialogueEnd = true;
    }

    void ShowChargeAttackSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "와, 방금 봤어?!" },
            new DialogueLine { speaker = "에이", text = "그게 차징 공격이야!" },
            new DialogueLine { speaker = "에이", text = "힘을 모은 만큼 더 강하게 밀어낼 수 있어!" },
            new DialogueLine { speaker = "에이", text = "좋아, 이제 조금씩 감각이 돌아오는 것 같아!" },
            new DialogueLine { speaker = "에이", text = "그럼 이번엔 불씨의 힘을 직접 끌어내 보자!" },
            new DialogueLine { speaker = "안내", text = "S 키로 스킬을 사용할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForSkillDialogueEnd = true;
    }

    void ShowSkillSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "좋아!" },
            new DialogueLine { speaker = "에이", text = "방금 그게 스킬이야!" },
            new DialogueLine { speaker = "에이", text = "불씨의 힘을 쓰는 기술이라서, 아무 때나 막 쓰면 위험할 수도 있어." },
            new DialogueLine { speaker = "에이", text = "하지만 지금처럼 꼭 필요할 땐 실라를 지켜줄 거야!" },
            new DialogueLine { speaker = "에이", text = "위험한 순간에는 빠르게 몸을 빼는 것도 중요해!" },
            new DialogueLine { speaker = "안내", text = "Left Shift 키로 대쉬할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForDashDialogueEnd = true;
    }

    void ShowDashSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "좋아, 빨랐어!" },
            new DialogueLine { speaker = "에이", text = "방금처럼 대쉬하면 위험한 공격을 피할 수 있어." },
            new DialogueLine { speaker = "에이", text = "하지만 무작정 쓰면 오히려 더 위험할 수도 있으니까 조심해야 해!" },
            new DialogueLine { speaker = "에이", text = "그리고 잠깐 동안 몸을 더 빠르게 움직이는 힘도 남아 있어!" },
            new DialogueLine { speaker = "안내", text = "C 키로 이속증가를 사용할 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForSpeedUpDialogueEnd = true;
    }

    void ShowSpeedUpSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "좋아!" },
            new DialogueLine { speaker = "에이", text = "방금처럼 D 키를 누르면 잠시 동안 더 빠르게 움직일 수 있어!" },
            new DialogueLine { speaker = "에이", text = "멀리 이동하거나, 위험한 곳을 빠르게 벗어날 때 써보자!" },
            new DialogueLine { speaker = "에이", text = "이제 기본 움직임과 전투 감각은 거의 돌아온 것 같아." },
            new DialogueLine { speaker = "에이", text = "마지막으로, 여행에 필요한 화면들도 확인해보자!" },
            new DialogueLine { speaker = "안내", text = "Q 키로 인벤토리를 열 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForInventoryDialogueEnd = true;
    }

    void ShowInventorySuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "좋아, 그게 인벤토리야!" },
            new DialogueLine { speaker = "에이", text = "여행 중에 얻은 물건이나 필요한 것들은 여기서 확인할 수 있어." },
            new DialogueLine { speaker = "에이", text = "어둠 속에서는 작은 물건 하나도 큰 도움이 될 수 있으니까, 가끔 확인해줘!" },
            new DialogueLine { speaker = "안내", text = "M 키로 맵을 열 수 있습니다." }
        };

        DialogueManager.instance.StartDialogue(lines);
        waitingForMapDialogueEnd = true;
    }

    void ShowMapSuccessDialogue()
    {
        DialogueLine[] lines = new DialogueLine[]
        {
            new DialogueLine { speaker = "에이", text = "응, 그게 맵이야!" },
            new DialogueLine { speaker = "에이", text = "우리가 어디에 있는지, 어디로 가야 할지 확인할 수 있어." },
            new DialogueLine { speaker = "에이", text = "좋아... 이제 정말 출발할 수 있겠다." },
            new DialogueLine { speaker = "에이", text = "무섭지 않냐고?" },
            new DialogueLine { speaker = "에이", text = "나는 조금 무섭긴 한데..." },
            new DialogueLine { speaker = "에이", text = "그래도 괜찮아. 실라 곁에는 내가 있으니까!" }
        };

        DialogueManager.instance.StartDialogue(lines);
    }
}
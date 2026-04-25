using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
// 정신력 4단계 열거형
// 플레이어의 현재 정신력 수치를 4개의 구간으로 분류한다.
//
//  Normal     (70~100) : 정상 상태. 스탯 보정 없음.
//  Depression (50~ 69) : 우울 상태. 공격력 +15% (억눌린 분노 반응)
//  Anxiety    (30~ 49) : 불안 상태. 이동속도 +10% (도주 본능 반응)
//  Collapse   ( 0~ 29) : 붕괴 상태. 공격력 +30% (광기 반응)
// ─────────────────────────────────────────────────────────────────────────────
public enum MentalState { Normal, Depression, Anxiety, Collapse }

/// <summary>
/// 플레이어의 정신력(Mental) 수치를 감시하고,
/// 구간에 따라 스탯 배율을 적용하며 GameManager에 상태를 전파하는 시스템.
///
/// [레이어 구분]
///  - Monitoring : Entity.OnMentalChanged 이벤트를 구독해 수치 변화를 감지
///  - Logic      : 수치 → MentalState 판단, 배율 계산
///  - Output     : GameManager.SetMentalState()로 다른 시스템(Dialogue, Spawner 등)에 전파
/// </summary>
public class MentalSystem : MonoBehaviour
{
    private PlayerController player;

    // 이전 상태를 기억해 동일 상태 반복 전파를 방지
    private MentalState currentState = MentalState.Normal;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        // Entity의 OnMentalChanged 이벤트 구독
        // → 정신력 수치가 바뀔 때마다 ChangeMental()이 자동 호출됨
        var entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        entity.OnMentalChanged += ChangeMental;
    }

    /// <summary>
    /// 정신력 수치가 변경될 때 Entity로부터 호출되는 메서드.
    /// 수치를 비율로 환산해 MentalState를 결정하고,
    /// 스탯 배율 적용 및 상태 전파를 수행한다.
    /// </summary>
    /// <param name="currentMental">변경 후 현재 정신력 수치</param>
    public void ChangeMental(float currentMental)
    {
        // 정신력 수치를 0~1 비율로 환산 (maxMental 기준)
        float ratio = currentMental / player.maxMental;

        // 비율에 따라 4단계 중 하나로 분류
        MentalState newState;
        if (ratio > 0.7f)       newState = MentalState.Normal;
        else if (ratio > 0.5f)  newState = MentalState.Depression;
        else if (ratio > 0.3f)  newState = MentalState.Anxiety;
        else                    newState = MentalState.Collapse;

        // 매 수치 변화마다 배율 적용 (상태가 같아도 외부에서 배율을 덮어쓸 수 있으므로 항상 갱신)
        ApplyMentalEffect(newState);

        // 상태가 실제로 바뀐 경우에만 GameManager에 알림 (불필요한 이벤트 발화 방지)
        if (newState != currentState)
        {
            currentState = newState;
            GameManager.instance.SetMentalState(currentState);
        }
    }

    /// <summary>
    /// MentalState에 따라 PlayerController의 스탯 배율을 적용한다.
    /// 상태 전환 시 이전 배율이 잔류하지 않도록 매번 1.0으로 초기화한 뒤 효과를 적용한다.
    /// </summary>
    private void ApplyMentalEffect(MentalState state)
    {
        // ── 초기화: 이전 상태의 배율이 누적되지 않도록 기본값으로 리셋 ──
        player.speedMultiplier = 1f;
        player.atkMultiplier   = 1f;

        // ── 해당 상태의 효과만 적용 ──
        switch (state)
        {
            case MentalState.Normal:
                // 초기화만으로 충분 (보정 없음)
                break;

            case MentalState.Depression:
                // 우울 반응: 억눌린 감정이 공격성으로 표출 → 공격력 소폭 증가
                player.atkMultiplier = 1.15f;
                break;

            case MentalState.Anxiety:
                // 불안 반응: 도주 본능 활성화 → 이동속도 소폭 증가
                player.speedMultiplier = 1.1f;
                break;

            case MentalState.Collapse:
                // 붕괴 반응: 이성 상실, 광기 상태 → 공격력 대폭 증가
                player.atkMultiplier = 1.3f;
                break;
        }
    }
}

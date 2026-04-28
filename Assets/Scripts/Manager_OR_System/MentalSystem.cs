using UnityEngine;

// 정신력 4단계: 논문 설계 기준
// Normal(70~100), Depression(50~69), Anxiety(30~49), Collapse(0~29)
public enum MentalState { Normal, Depression, Anxiety, Collapse }

public class MentalSystem : MonoBehaviour
{
    private PlayerController player;
    private MentalState currentState = MentalState.Normal; // 현재 정신력 상태 저장

    void Awake()
    {
        // 플레이어 오브젝트에서 PlayerController 컴포넌트를 가져옴
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        // 플레이어의 정신력 변화 이벤트(OnMentalChanged)에 ChangeMental 함수를 연결
        // 정신력 수치가 바뀔 때마다 자동으로 ChangeMental이 호출됨
        var entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        entity.OnMentalChanged += ChangeMental;
    }

    public void ChangeMental(float currentMental)
    {
        // 현재 정신력을 최대 정신력으로 나눠 0~1 사이의 비율로 변환
        float ratio = currentMental / player.maxMental;

        // 비율에 따라 4단계 중 하나로 분류
        MentalState newState;
        if (ratio >= 0.7f)       newState = MentalState.Normal;     // 70 이상
        else if (ratio >= 0.5f)  newState = MentalState.Depression; // 50~69
        else if (ratio >= 0.3f)  newState = MentalState.Anxiety;    // 30~49
        else                     newState = MentalState.Collapse;    // 0~29

        // 결정된 상태에 맞는 스탯 배율을 플레이어에게 적용
        ApplyMentalEffect(newState);

        // 이전 상태와 달라졌을 때만 GameManager에 알림
        // (같은 상태가 반복 전파되지 않도록 방지)
        if (newState != currentState)
        {
            currentState = newState;
            GameManager.instance.SetMentalState(currentState); // 다른 시스템(대화, 스폰 등)에 상태 전달
        }
    }

    private void ApplyMentalEffect(MentalState state)
    {
        // 상태가 바뀔 때 이전 배율이 남아있지 않도록 먼저 기본값으로 초기화
        player.speedMultiplier = 1f;
        player.atkMultiplier   = 1f;

        switch (state)
        {
            case MentalState.Normal:
                // 정상 상태: 초기화만으로 충분, 추가 효과 없음
                break;

            case MentalState.Depression:
                player.atkMultiplier = 1.15f; // 우울: 공격력 15% 증가
                break;

            case MentalState.Anxiety:
                player.speedMultiplier = 1.1f; // 불안: 이동속도 10% 증가
                break;

            case MentalState.Collapse:
                player.atkMultiplier = 1.3f; // 붕괴: 공격력 30% 증가
                break;
        }
    }
}

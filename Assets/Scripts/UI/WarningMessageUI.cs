using UnityEngine;
using TMPro;
using System.Collections;

public class WarningMessageUI : MonoBehaviour
{
    // 경고문구 패널 전체
    public GameObject warningPanel;

    // 화자 이름 텍스트
    public TMP_Text warningNameText;

    // 경고 내용 텍스트
    public TMP_Text warningText;

    // 실행 중인 경고문구 코루틴 저장
    private Coroutine warningCoroutine;

    void Start()
    {
        // 게임 시작 시 경고 패널 숨김
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    // 경고문구 출력 함수
    public void ShowWarning(DialogueLine[] warningLines, float duration = 3f)
    {
        if (warningLines == null || warningLines.Length == 0)
            return;

        if (warningPanel == null || warningNameText == null || warningText == null)
        {
            Debug.Log("WarningMessageUI 연결 안 됨");
            return;
        }

        // 이전 경고가 떠 있으면 정리
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }

        // 첫 번째 경고 대사만 표시
        warningNameText.text = warningLines[0].speaker;
        warningText.text = warningLines[0].text;

        warningPanel.SetActive(true);

        warningCoroutine = StartCoroutine(HideWarningAfterTime(duration));
    }

    // 일정 시간 후 경고문구 숨김
    private IEnumerator HideWarningAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (warningPanel != null)
            warningPanel.SetActive(false);

        warningCoroutine = null;
    }
}
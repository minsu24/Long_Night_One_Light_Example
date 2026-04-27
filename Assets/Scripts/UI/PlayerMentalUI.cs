using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMentalUI : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField]
    private TextMeshProUGUI textMentalState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        GameManager.instance.OnMentalStateChanged += ChangeText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeText(MentalState mentalState)
    {
        switch (mentalState)
        {
            case MentalState.Collapse:
                textMentalState.text = "Collapse";
                return;

            case MentalState.Anxiety:
                textMentalState.text = "Anxiety";
                return;

            case MentalState.Depression:
                textMentalState.text = "Depression";
                return;

            default: // Normal
                textMentalState.text = "Normal";
                return;
                
        }
    }

    // 부모가 뒤집혀도 텍스트는 정방향을 유지하게 하는 코드
    void LateUpdate()
    {
        Vector3 parentScale = transform.parent.localScale;
        // 부모의 x가 음수라면 내 x를 -1로 곱해 상쇄시킴
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (parentScale.x > 0 ? 1 : -1),
        transform.localScale.y, transform.localScale.z);
    }
}

using UnityEngine;
using UnityEngine.UI;


public class Boss00HP : MonoBehaviour
{
    [SerializeField]
    private MON_BOSS_00 bossController;
    private GameObject Boss00;
    [SerializeField]
    private Slider sliderHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boss00 = GameObject.FindGameObjectWithTag("Boss00");
        bossController = Boss00.GetComponent<MON_BOSS_00>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sliderHP != null) sliderHP.value = Utils.Percent(bossController.HP, bossController.maxHP); // 슬라이드를 이용한 HP 표시
    }
}

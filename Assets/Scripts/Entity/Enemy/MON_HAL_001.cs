using UnityEngine;

public class MON_HAL_001 : EnemyController
{
    public float mentalReduce;
    bool firstHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void MonsterAbility()
    {
        MentalOra();
    }

    private void MentalOra()
    {
        playerController.Mental -= mentalReduce * Time.deltaTime;
    }
}

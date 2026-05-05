using UnityEngine;

public class MON_HAL_001 : EnemyController
{
    public float mentalReduce;
    bool firstHit = false;

    protected override void MonsterAbility()
    {
        MentalOra();
    }

    private void MentalOra()
    {
        playerController.Mental -= mentalReduce * Time.deltaTime;
    }
}

using System.Collections;
using System.Linq;
using UnityEngine;

public class MON_SAD_001 : EnemyController
{

    protected override void MonsterAbility()
    {
        MentalOra();
    }

    private void MentalOra()
    {
        playerController.Mental -= 1 * Time.deltaTime;
    }
}

using System.Collections;
using System.Linq;
using UnityEngine;

public class MON_SAD_001 : EnemyController
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    protected override void MonsterAbility()
    {
        MentalOra();
    }

    private void MentalOra()
    {
        playerController.Mental -= 1 * Time.deltaTime;
    }
}

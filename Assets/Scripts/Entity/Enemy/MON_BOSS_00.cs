using Unity.VisualScripting;
using UnityEngine;

public class MON_BOSS_00 : EnemyController
{
    public GameObject projectilePrefab;
    public float fireRange = 7f;
    public float attackRate = 4f;

    private float lastFireTime;

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        projectile.GetComponent<BossProjectile>().Setup(direction);
    }
    
    protected override void MonsterAbility()
    {
        if(Time.time >= lastFireTime + attackRate)
        {
            FireProjectile();
            lastFireTime = Time.time;
        }
        if(HP / maxHP > 0.5f)
        {

        }
        else
        {
            
        }
    }
}

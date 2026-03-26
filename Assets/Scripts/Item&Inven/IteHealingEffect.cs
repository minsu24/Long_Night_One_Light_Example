using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Health")]
public class IteHealingEffect : ItemEffect
{
    public int healingPoint = 0;
    public override bool ExecuteRole()
    {
        Debug.Log("Player Hp Add : " + healingPoint);
        return true;
    }
}

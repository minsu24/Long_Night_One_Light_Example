using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Health")]
public class IteHealingEffect : ItemEffect
{
    public float healingPoint = 0f;
    public override bool ExecuteRole()
    {
        Debug.Log("Player Hp Add : " + healingPoint);
        return true;
    }
}

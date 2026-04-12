using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Mental")]
public class ItemMentalHealEft : ItemEffect
{
    public float mental_heal_amount = 0f;
    public float heal_duration = 2.0f;

    PlayerController playerController;

    public override bool ExecuteRole()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerController.startMentalHeal(mental_heal_amount, heal_duration);
            return true;
        }
        else
        {
            return false;
        }
    }
}

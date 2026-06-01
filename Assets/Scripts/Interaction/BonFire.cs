using UnityEngine;

public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float mentalRecoverAmount = 10f;
    [SerializeField]
    private float hpRecoverAmount = 10f;
    PlayerController playerController;
    private void Awake() 
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void Interact()
    {
        playerController.Mental = Mathf.Min(playerController.Mental + mentalRecoverAmount, playerController.maxMental);
        playerController.HP = Mathf.Min(playerController.HP + hpRecoverAmount, playerController.maxHP);
    }

}

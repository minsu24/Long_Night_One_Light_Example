using Mono.Cecil.Cil;
using UnityEngine;

public enum MentalState{ Normal, Sad, Angry, Despair, Madness }

public class MentalSystem : MonoBehaviour
{
    private PlayerController player;
    

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {        
        var entity = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
        entity.OnMentalChanged += ChangeMental;
    }
    public void ChangeMental(float currentMental)
    {
        MentalState state;
        if(currentMental/player.maxMental <= 0.1f){state = MentalState.Madness;}
        else if(currentMental/player.maxMental <= 0.3f) {state = MentalState.Despair;}
        else if(currentMental/player.maxMental <= 0.5f) {state = MentalState.Angry;}
        else if(currentMental/player.maxMental <= 0.7f) {state = MentalState.Sad;}
        else {state = MentalState.Normal;}

        ApplyMentalEffect(state);
    }

    private void ApplyMentalEffect(MentalState state)
    {
        switch (state)
        {
            case MentalState.Normal:
                player.speedMultiplier = 1f;
                player.atkMultiplier = 1f;
                break;
            case MentalState.Sad:
                player.atkMultiplier = 1.15f;
                break;
            case MentalState.Angry:
                player.speedMultiplier = 1.1f;
                Debug.Log(player.FinalSpeed);
                break;
            case MentalState.Despair: 
                break;
            case MentalState.Madness:
                player.atkMultiplier = 1.3f;
                break;
        }
    }
}

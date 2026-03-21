using Mono.Cecil.Cil;
using UnityEngine;

public class MentalSystem : MonoBehaviour
{
    string state;
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
        if(currentMental <= 10)
        {
            
        }
        else if(currentMental <= 30)
        {
            
        }
        else if(currentMental <= 50)
        {
            
        }
        else if(currentMental <= 70)
        {
            
        }
        else
        {
            
        }
    }
}

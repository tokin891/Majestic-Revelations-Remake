using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{
    public Enemy Index;
    public float damage;
    private PlayerMovement Player;

    private bool startAttack = false;
    private bool isInTrigger = false;

    private void Update()
    {
        if(isInTrigger)
        {
            if (Index.IsAttack && !startAttack)
            {
                startAttack = true;
                Player.TakeDamage(new Damage
                { 
                    damage = this.damage
                });
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            isInTrigger = true;
            Player = other.GetComponent<PlayerMovement>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            isInTrigger = false;
            Player = null;
            startAttack = false;
        }
    }
}

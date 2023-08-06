using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public bool IsAttack { set; get; }
    public EnemyController EnemyController;

    private void OnTriggerEnter(Collider other)
    {
        if (IsAttack)
            if (other.TryGetComponent(out PlayerMovement player))
                player.TakeDamage(new Damage { damage = EnemyController.AttackDamage });
    }
}

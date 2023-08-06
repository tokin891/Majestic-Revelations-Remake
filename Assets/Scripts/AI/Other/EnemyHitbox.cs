using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public bool IsHead { set; get; }
    public EnemyController enemyController;

    public void TakeDamage(Damage dmg)
    {
        if (IsHead)
            dmg.damage = 100;

        enemyController.TakeDamage(dmg);
    }
}

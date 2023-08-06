using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitboxController : MonoBehaviour
{
    [SerializeField] Collider headCollider;
    [SerializeField] Collider weaponColider;
    [SerializeField] Collider[] otherCollider;
    [SerializeField] EnemyController enemyController;

    private void Awake()
    {
        EnemyHitbox headHitbox = headCollider.gameObject.AddComponent<EnemyHitbox>();
        headHitbox.IsHead = true;
        headHitbox.enemyController = enemyController;
        EnemyWeapon enemyWeapon = weaponColider.gameObject.AddComponent<EnemyWeapon>();
        enemyWeapon.EnemyController = enemyController;
        enemyController.EnemyWeapon = enemyWeapon;

        for (int i = 0; i < otherCollider.Length; i++)
        {
            EnemyHitbox otherHitbox = otherCollider[i].gameObject.AddComponent<EnemyHitbox>();
            otherHitbox.IsHead = false;
            otherHitbox.enemyController = enemyController;
        }
    }
}

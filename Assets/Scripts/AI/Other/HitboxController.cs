using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] Collider headCollider;
    [SerializeField] Collider weaponColider;
    [SerializeField] Collider[] otherCollider;
    [SerializeField] EnemyController enemyController;

    private void Awake()
    {
        Hitbox headHitbox = headCollider.gameObject.AddComponent<Hitbox>();
        headHitbox.IsHead = true;
        headHitbox.enemyController = enemyController;

        for (int i = 0; i < otherCollider.Length; i++)
        {
            Hitbox otherHitbox = otherCollider[i].gameObject.AddComponent<Hitbox>();
            otherHitbox.IsHead = false;
            otherHitbox.enemyController = enemyController;
        }
    }
}

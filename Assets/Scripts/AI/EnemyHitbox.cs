using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] float damageMultiplayer;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void TakeDamage(Damage damage)
    {
        GetComponentInParent<Enemy>().TakeDamage(damage.damage * damageMultiplayer);
    }
}

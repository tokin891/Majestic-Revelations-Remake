using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public void StopAttack()
    {
        GetComponentInParent<Enemy>().StopAttack();
    }
}

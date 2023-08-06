using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IEnemyAIState
{
    private Transform playerTarget;
    private EnemyProps enemyProps;
    private EnemyWeapon enemyWeapon;
    private const float speedOfChase = 2f;

    public ChaseState(Transform playerTarget, EnemyProps enemyProps, EnemyWeapon enemyWeapon)
    {
        this.playerTarget = playerTarget;
        this.enemyProps = enemyProps;
        this.enemyWeapon = enemyWeapon;
    }

    public void EnterState(EnemyAI ai)
    {
        enemyProps.Animator.SetBool("Run", true);
        enemyProps.Agent.speed = speedOfChase;
    }

    public void ExitState()
    {
        enemyProps.Animator.SetBool("Run", false);
        enemyProps.Animator.SetBool("Attack", false);

        enemyWeapon.IsAttack = false;
    }

    public void UpdateState()
    {
        enemyProps.Agent.SetDestination(playerTarget.position);

        enemyProps.Animator.SetBool("Attack", ReadyToAttack());
        enemyWeapon.IsAttack = ReadyToAttack();
    }

    private bool ReadyToAttack()
    {
        return Vector3.Distance(enemyProps.Agent.transform.position, playerTarget.position) < enemyProps.Agent.stoppingDistance + 2.2f
               && Mathf.Abs(Vector3.Angle(enemyProps.Agent.transform.position, (playerTarget.position - enemyProps.Agent.transform.position))) > 10f;
    }
}

public class PatrolState : IEnemyAIState
{
    private Transform[] waypoints;
    private EnemyProps enemyProps;

    private int destPoint = 0;
    private Vector3 targetPoint;

    private const float speedOfPatrol = 1f;

    public PatrolState(Transform[] waypoints, EnemyProps enemyProps)
    {
        this.enemyProps = enemyProps;
        this.waypoints = waypoints;
    }

    public void EnterState(EnemyAI ai)
    {
        destPoint = 0;
        targetPoint = waypoints[destPoint].position;
        enemyProps.Agent.SetDestination(targetPoint);
        enemyProps.Animator.SetBool("Walk", true);
        enemyProps.Agent.speed = speedOfPatrol;
    }

    public void ExitState()
    {
        enemyProps.Animator.SetBool("Walk", false);
    }

    public void UpdateState()
    {
        MoveToPoints();
    }

    private void MoveToPoints()
    {
        if (Vector3.Distance(enemyProps.Transform.position, targetPoint) < 3f)
        {
            destPoint++;
            if (destPoint == waypoints.Length)
            {
                destPoint = 0;
            }

            targetPoint = waypoints[destPoint].position;
            enemyProps.Agent.SetDestination(targetPoint);
        }
    }
}

public class JumpState : IEnemyAIState
{
    private EnemyProps enemyProps;
    private Vector3 endPos;
    private float normalizedTime;
    private OffMeshLinkData data;


    public JumpState (EnemyProps enemyProps)
    {
        this.enemyProps = enemyProps;
    }

    public void EnterState(EnemyAI ai)
    {
        enemyProps.Animator.SetBool("Jump", true);
        data = enemyProps.Agent.currentOffMeshLinkData;

        if(enemyProps.Agent.transform.position.y > data.endPos.y)
        {
            endPos = data.endPos + Vector3.down * enemyProps.Agent.baseOffset;
        }else
            endPos = data.endPos + Vector3.up * enemyProps.Agent.baseOffset;
        normalizedTime = 0.0f;
    }

    public void ExitState()
    {
        enemyProps.Animator.SetBool("Jump", false);
        enemyProps.Agent.CompleteOffMeshLink();
    }

    public void UpdateState()
    {
        normalizedTime += Time.deltaTime / .5f;
        if (normalizedTime < 1.0f)
        {
            float yOffset = 1f * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            enemyProps.Agent.transform.position = Vector3.Lerp(enemyProps.Agent.transform.position, endPos, normalizedTime) + yOffset * Vector3.up;
        }
    }
}

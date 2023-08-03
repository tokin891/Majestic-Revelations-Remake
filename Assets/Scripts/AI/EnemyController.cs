using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private bool slightSeePlayer;
    private bool isJumping = false;

    [SerializeField]
    float healthOnStart = 100f;
    [SerializeField] 
    Transform[] waypoints;
    [SerializeField] 
    Animator anm;
    [SerializeField]
    float timePlayerGone = 3f;
    [SerializeField]
    private float radiusSeePlayer = 1f;
    [SerializeField]
    GameObject slightEyes;
    [SerializeField] 
    PlayerMovement playerTarget;

    private EnemyProps enemyProps;

    public float Health { private set; get; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyProps = new EnemyProps(transform, agent, anm);
    }

    private void Start()
    {
        enemyAI = new EnemyAI();

        enemyAI.SetState(new PatrolState(waypoints, enemyProps));
        Health = healthOnStart;
    }

    private void Update()
    {
        enemyAI.Update();

        if (IsPlayerAround() && !isJumping)
        {
            OnPlayerDetected();
        }         
    }

    public void OnPlayerDetected()
    {
        isJumping = false;

        enemyAI.SetState(new ChaseState(playerTarget.transform, enemyProps));
    }

    public void OnPlayerLost()
    {
        enemyAI.SetState(new PatrolState(waypoints, enemyProps));
    }

    private bool IsPlayerAround()
    {     
        if (playerTarget == null)
            return false ;

        slightEyes.transform.LookAt(playerTarget.transform);
        if (Physics.Raycast(slightEyes.transform.position, slightEyes.transform.forward, out var hit))
        {
            slightSeePlayer = hit.transform.tag.Equals("Player") ? true : false;
        }

        float distance = Vector3.Distance(transform.position, playerTarget.transform.position);

        if (slightSeePlayer)
        {
            if (distance > radiusSeePlayer)
                return false;
        }
        else
            return false;

        if (IsOnAngleViewForward() && playerTarget.isCrouching) 
            return false;
        if (IsOnAngleViewUp())
            return false;

        return true;
    }

    private bool IsOnAngleViewForward()
    {
        return Mathf.Abs(Vector3.Angle(transform.forward, (playerTarget.transform.position - transform.position))) > 90f;
    }

    private bool IsOnAngleViewUp()
    {
        return Mathf.Abs(Vector3.Angle(transform.up, (playerTarget.transform.position - transform.position)) - 90f) > 5f;
    }

    public void TakeDamage(Damage dmg)
    {
        if (Health <= 0)
            return;

        Health -= dmg.damage;
        if (Health > 0)
            return;

        agent.enabled = false;
        anm.SetTrigger("Dead");
    }

    private void OnTriggerEnter(Collider other)
    {
       if (agent.isOnOffMeshLink && !isJumping)
       {
            enemyAI.SetState(new JumpState(enemyProps));
            isJumping = true;
       }
    }

    private void OnTriggerExit(Collider other)
    {
        if(isJumping)
        {
            isJumping = false;
        }
    }
}
public class EnemyProps
{
    public Transform Transform { private set; get; }
    public NavMeshAgent Agent { private set; get; }
    public Animator Animator { private set; get; }

    public EnemyProps(Transform transform, NavMeshAgent agent, Animator animator)
    {
        Transform = transform;
        Agent = agent;
        Animator = animator;
    }
}
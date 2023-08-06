using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private bool slightSeePlayer;
    private bool isJumping = false;

    [SerializeField]
    private float attackDamage = 15f;
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

    public EnemyWeapon EnemyWeapon { get; set; }
    public float Health { private set; get; }
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

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

        enemyAI.SetState(new ChaseState(playerTarget.transform, enemyProps, EnemyWeapon));
    }

    public void OnPlayerLost()
    {
        enemyAI.SetState(new PatrolState(waypoints, enemyProps));
    }

    private bool IsPlayerAround()
    {     
        if (playerTarget == null)
            return false ;
        Debug.Log("Search Player");

        slightEyes.transform.LookAt(playerTarget.transform);
        if (Physics.Raycast(slightEyes.transform.position, slightEyes.transform.forward, out var hit))
        {
            slightSeePlayer = hit.transform.tag.Equals("Player") ? true : false;
        }

        float distance = Vector3.Distance(transform.position, playerTarget.transform.position);

        if (slightSeePlayer)
        {
            if (distance > radiusSeePlayer)
            {
                Debug.Log("To long distance");
                return false;
            }
        }
        else
        {
            Debug.Log("Dont see player");
            return false;
        }

        if (IsOffAngleViewForward() && playerTarget.isCrouching) 
            return false;
        if (IsOffAngleViewUp())
            return false;

        return true;
    }

    private bool IsOffAngleViewForward()
    {
        return Mathf.Abs(Vector3.Angle(transform.forward, (playerTarget.transform.position - transform.position))) > 90f;
    }

    private bool IsOffAngleViewUp()
    {
        Debug.Log(Mathf.Abs(Vector3.Angle(transform.up, (playerTarget.transform.position - transform.position)) - 90f) > 7f);
        return Mathf.Abs(Vector3.Angle(transform.up, (playerTarget.transform.position - transform.position)) - 90f) > 7f;
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
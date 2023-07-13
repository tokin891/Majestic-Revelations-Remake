using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public bool IsAttack { private set; get; }
    public float Health { private set; get; }
    public bool IsJump { private set; get; }

    private int destPoint = 0;
    private Vector3 targetPoint;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private bool slightSeePlayer;
    private bool startCountPlayerGone = false;
    private Animation anm = new Animation();
    private PlayerMovement playerMovment;

    [Header("Main")]
    [SerializeField, Range(0f,100f)]
    float startHealth;
    [Space]
    [Header("Options")]
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField] 
    float speedPatrol;
    [SerializeField] 
    float speedChase;
    [SerializeField]
    float timePlayerGone = 3f;
    [SerializeField]
    private State state = new State();
    [SerializeField]
    private Type type = new Type();
    [SerializeField, Tooltip("White color circle")]
    private float radiusHearPlayer = 1f;
    [SerializeField,Tooltip("Red color circle")]
    private float radiusSeePlayer = 1f;
    [SerializeField]
    GameObject slightEyes;
    [Header("Jump")]
    [SerializeField] Vector3 groundCheck;
    [SerializeField] float groundRadius;
    [Space]
    [Header("Attack Options")]
    [SerializeField] float damageGivePlayer;
    [SerializeField] GameObject meleeWeapon;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        agent.autoBraking = false;
        Health = startHealth;

        targetPoint = waypoints[destPoint].position;
        agent.SetDestination(targetPoint);
        anm = Animation.Idle;

        if (meleeWeapon.GetComponent<EnemyMeleeWeapon>() == null)
        {
            EnemyMeleeWeapon emw = meleeWeapon.AddComponent<EnemyMeleeWeapon>();
            emw.Index = this;
            emw.damage = this.damageGivePlayer;
        }
    }
    IEnumerator Start()
    {
        agent.autoTraverseOffMeshLink = false;
        while(true)
        {
            if(agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(Jump(2f, 0.5f));

                agent.CompleteOffMeshLink();

            }
            yield return null;
        }
    }
    IEnumerator Jump(float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;

        float time = 0.0f;

        while (time < 1.0f)
        {
            float upDist = height * (time - time * time);
            agent.transform.position = Vector3.Lerp(startPos, endPos, time) + upDist * Vector3.up;

            time += Time.deltaTime / duration;
            yield return null;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();

                if(IsPlayerAround())
                {
                    SwitchState(State.Chase);
                }
                SwitchAniamtion(Animation.Walk);
                break;
            case State.Idle:
                Idle();
                SwitchAniamtion(Animation.Idle);
                break;
            case State.Chase:
                Chase();
                IsPlayerGone();
                SwitchAniamtion(Animation.Run);
                break;
        }

        IsJump = Physics.CheckSphere(groundCheck + transform.position, groundRadius);
        Transform playerT = FindObjectOfType<PlayerMovement>().transform;
        slightEyes.transform.LookAt(playerT);
        RaycastHit hit;
        if (Physics.Raycast(slightEyes.transform.position, slightEyes.transform.forward, out hit))
        {
            slightSeePlayer = hit.transform.tag.Equals("Player") ? true : false;
        }
    }

    private void Patrol()
    {
        if(Vector3.Distance(transform.position, targetPoint) < 3f)
        {
            destPoint++;
            if(destPoint == waypoints.Length)
            {
                destPoint = 0;
            }

            targetPoint = waypoints[destPoint].position;
            agent.SetDestination(targetPoint);
        }
    }
    private void Idle()
    {
        agent.SetDestination(transform.position);
        agent.speed = 0f;
    }
    private void Chase()
    {
        if (slightSeePlayer)
            agent.SetDestination(playerMovment.GetDestination(agent).position);
        agent.speed = speedChase;

        if (Vector3.Distance(transform.position, player.position) < agent.stoppingDistance + 2.2f
            && Mathf.Abs(Vector3.Angle(transform.forward, (player.position - transform.position))) > 10f)
        {
            Attack();
        }
    }
    private void Attack()
    {
        IsAttack = true;
        switch (type)
        {
            case Type.Melee:
                animator.SetTrigger("AttackMelee");
                break;
        }
    }

    public void StopAttack()
    {
        IsAttack = false;
        animator.ResetTrigger("AttackMelee");
    }

    private bool IsPlayerAround()
    {
        Transform playerT = FindObjectOfType<PlayerMovement>().transform;
        float distance = Vector3.Distance(transform.position, playerT.position);

        if (slightSeePlayer)
        {
            if (distance < radiusHearPlayer && distance > radiusSeePlayer)
                if (playerT.GetComponent<PlayerMovement>().isCrouching)
                    return false;
        }
        else
            return false;

        if (Mathf.Abs(Vector3.Angle(transform.forward, (playerT.position - transform.position))) > 90f
            && playerT.GetComponent<PlayerMovement>().isCrouching)
            return false;
        if (Mathf.Abs(Vector3.Angle(transform.up, (playerT.position - transform.position)) - 90f) > 5f)
            return false;

        player = playerT;
        playerT.GetComponent<PlayerMovement>().AddAgent(agent);
        playerMovment = player.GetComponent<PlayerMovement>();
        return true;
    }
    private void IsPlayerGone()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if((distance > radiusHearPlayer || !slightSeePlayer))
        {
            if(!startCountPlayerGone)
            {
                startCountPlayerGone = true;
                Invoke(nameof(CheckIsPlayerGone), timePlayerGone);
                Debug.Log("Start Counting");
            }
        }else
        {
            startCountPlayerGone = false;
        }
    }
    private void CheckIsPlayerGone()
    {
        Debug.Log("Counting end");
        float distance = Vector3.Distance(transform.position, player.position);
        if ((distance > radiusHearPlayer || !slightSeePlayer))
        {
            SwitchState(State.Patrol);
            playerMovment.RemoveAgent(agent);
            playerMovment = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(groundCheck + transform.position, groundRadius);

        Gizmos.color = Color.white;
        Vector3 center = transform.position; // Ustawianie œrodka ko³a na lokalne (0,0,0) obiektu

        const int numSegments = 360;
        const float segmentAngle = 360f / numSegments;
        float currentAngle = 0f;

        Quaternion rotation = Quaternion.Euler(90f, 0f, 0f); // Obrót o 90 stopni w osi x

        for (int i = 0; i < numSegments; i++)
        {
            float x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radiusHearPlayer;
            float y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radiusHearPlayer;

            Vector3 startPoint = new Vector3(x, y, 0f);
            startPoint = rotation * startPoint; // Zastosowanie obrotu

            currentAngle += segmentAngle;

            x = Mathf.Cos(currentAngle * Mathf.Deg2Rad) * radiusHearPlayer;
            y = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * radiusHearPlayer;

            Vector3 endPoint = new Vector3(x, y, 0f);
            endPoint = rotation * endPoint; // Zastosowanie obrotu

            Gizmos.DrawLine(center + startPoint, center + endPoint); // Dodanie œrodka obiektu do punktów ko³a
        }

        Gizmos.color = Color.red;
        Vector3 center2 = transform.position; // Ustawianie œrodka ko³a na lokalne (0,0,0) obiektu

        const int numSegments2 = 360;
        const float segmentAngle2 = 360f / numSegments;
        float currentAngle2 = 0f;

        Quaternion rotation2 = Quaternion.Euler(90f, 0f, 0f); // Obrót o 90 stopni w osi x

        for (int i2 = 0; i2 < numSegments2; i2++)
        {
            float x2 = Mathf.Cos(currentAngle2 * Mathf.Deg2Rad) * radiusSeePlayer;
            float y2 = Mathf.Sin(currentAngle2 * Mathf.Deg2Rad) * radiusSeePlayer;

            Vector3 startPoint2 = new Vector3(x2, y2, 0f);
            startPoint2 = rotation2 * startPoint2; // Zastosowanie obrotu

            currentAngle2 += segmentAngle2;

            x2 = Mathf.Cos(currentAngle2 * Mathf.Deg2Rad) * radiusSeePlayer;
            y2 = Mathf.Sin(currentAngle2 * Mathf.Deg2Rad) * radiusSeePlayer;

            Vector3 endPoint2 = new Vector3(x2, y2, 0f);
            endPoint2 = rotation2 * endPoint2; // Zastosowanie obrotu

            Gizmos.DrawLine(center2 + startPoint2, center2 + endPoint2); // Dodanie œrodka obiektu do punktów ko³a
        }
    }
    public void SwitchState(State state)
    {
        this.state = state;
        switch(state)
        {
            case State.Patrol:
                agent.destination = waypoints[destPoint].position;
                break;
        }
    }
    public void SwitchAniamtion(Animation anm)
    {
        if (this.anm == anm)
            return;
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);

        switch (anm)
        {
            case Animation.Idle:
                animator.SetBool("Idle", true);
                break;
            case Animation.Walk:
                animator.SetBool("Walk", true);
                break;
            case Animation.Run:
                animator.SetBool("Run", true);
                break;
        }
        this.anm = anm;
    }

    public void TakeDamage(float damg)
    {
        Health -= damg;
        if(Health <= 0)
        {
            Debug.Log(gameObject.name + " is dead");
            animator.SetBool("Dead",true);
            FindObjectOfType<ExperienceMonitoring>().AddEXP(50);
            agent.enabled = false;
            this.enabled = false;
            GetComponent<Collider>().isTrigger = true;
        }    
    }

    public enum State
    {
        Patrol,
        Idle,
        Chase
    }
    public enum Type
    {
        Melee
    }
    public enum Animation
    {
        Idle,
        Walk,
        Run,
        Jump
    }
}

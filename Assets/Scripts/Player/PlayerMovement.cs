using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public bool IsAbleToMove { set; get; }
    public bool IsGrounded { private set; get; }
    public bool isCrouching { private set; get; }
    public float Health { private set; get; }
    public bool IsAlive { private set; get; }
    public List<NavMeshAgent> AllConnectedAgent { private set; get; }

    [Header("Proporties")]
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float speedNormal = 8f;
    [SerializeField] float speedFast = 12f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float jumpHeight;
    [SerializeField] bool isImmortal;

    [Header("Object")]
    public Camera playerCamera;
    [SerializeField] Transform groundCheck;
    [SerializeField] UnityEngine.UI.Slider sliderHealth;
    [SerializeField] GameObject deadScreen;
    [SerializeField] Transform[] destination;
    [SerializeField] TMPro.TMP_Text textHealth;

    [Header("Func")]
    [SerializeField] bool hideCursor;

    [Header("Inputs")]
    [SerializeField] KeyCode jumpCode;
    [SerializeField] KeyCode sprintCode;
    [SerializeField] KeyCode crouchingCode;

    [Header("Events"), Space]
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onAddHealth;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRot = 0f;
    private float startHeight;

    private float speed { set; get; }

    private void Awake()
    {
        AllConnectedAgent = new List<NavMeshAgent>();
        Time.timeScale = 1f;
        IsAbleToMove = true;
        controller = GetComponent<CharacterController>();
        startHeight = controller.height;
        Health = 100;
        IsAlive = true;
    }
    private void Update()
    {
        if (!IsAlive)
        {
            Time.timeScale = 0f;
            deadScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        if (!IsAlive)
            return;

        if(IsAbleToMove && Cursor.lockState == CursorLockMode.Locked)
        {
            Move();
            Look();
        }

        Gravity();
        sliderHealth.value = Health;
        textHealth.text = Health.ToString() + "%";
    }
    private void Move()
    {
        float iX = Input.GetAxis("Horizontal");
        float iZ = Input.GetAxis("Vertical");
        isCrouching = Input.GetKey(crouchingCode);

        Vector3 move = transform.right * iX + transform.forward * iZ;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // JUMP
        if (Input.GetKeyDown(jumpCode) && IsGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Speed & Crouching
        if(isCrouching)
        {
            speed = speedNormal / 2f;
            controller.height = startHeight / 2f;
        }
        else
        {
            if (IsGrounded)
            {
                speed = Input.GetKey(sprintCode).Equals(true) ? speedFast : speedNormal;
            }
            else
                speed = speedNormal;
            controller.height = startHeight;
        }
    }
    private void Look()
    {
        float mX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRot -= mY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mX);
    }
    private void Gravity()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);

        if (IsGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    public void TakeDamage(Damage damage)
    {
        if (isImmortal)
            return;

        Health -= damage.damage;
        if (Health <= 0)
        {
            Health = 0f;
            sliderHealth.value = 0f;
            IsAlive = false;
        }
        onTakeDamage?.Invoke();
    }
    public void AddHealth(float health)
    {
        if (Health + health <= 100)
            Health += health;
        if (health + health > 100)
            Health = 100;

        onAddHealth?.Invoke();
    }

    public void AddAgent(NavMeshAgent agent)
    {
        for (int i = 0; i < AllConnectedAgent.Count; i++)
        {
            if (AllConnectedAgent[i] == agent)
                return;
        }
        AllConnectedAgent.Add(agent);
    }
    public void RemoveAgent(NavMeshAgent agent)
    {
        if (agent == null)
            return;
        for (int i = 0; i < AllConnectedAgent.Count; i++)
        {
            if(AllConnectedAgent[i] == agent)
                AllConnectedAgent.Remove(agent);
        }
    }
    public Transform GetDestination(NavMeshAgent agent)
    {
        for (int i = 0; i < AllConnectedAgent.Count; i++)
        {
            if(AllConnectedAgent[i] == agent)
            {
                // Find Agent
                int indexAgent = i;
                return destination[indexAgent];
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }
}
public struct Damage
{
    public float damage;
}


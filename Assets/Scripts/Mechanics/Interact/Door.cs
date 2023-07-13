using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractSystem
{
    private bool isOpen { get; set; }
    private Animator anmController;

    [Space]
    [Header("Event")]
    [SerializeField] UnityEngine.Events.UnityEvent onInteract;

    private void Awake()
    {
        anmController = GetComponent<Animator>();
    }

    public override void OnInteract()
    {
        InteractWithDoor();
        onInteract?.Invoke();
    }

    private void InteractWithDoor()
    {
        if(isOpen)
        {
            // Close Door
            anmController.SetBool("isOpen", false);
        }else
        {
            // Open Door
            anmController.SetBool("isOpen", true);
        }

        isOpen = !isOpen;
    }
}

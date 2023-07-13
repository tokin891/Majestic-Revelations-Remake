using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC : InteractSystem
{
    private bool inInteract = false;

    [Header("Options")]
    [SerializeField] Transform playerLookAt;
    [SerializeField] DialogueScript dialogue;
    [SerializeField] UnityEvent onEndDialogue;
    [SerializeField] GameObject objectGive;

    private void Update()
    {
        if(objectGive == null)
            GetComponentInChildren<Animator>().SetBool("give", false);
    }
    public override void OnInteract()
    {
        inInteract = true;

        // Stop Player
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        player.IsAbleToMove = false;

        player.playerCamera.transform.LookAt(playerLookAt);
        var lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = rotation;

        DialogueSystem.Instance.StartDialogue(dialogue, this);
        GetComponentInChildren<Animator>().SetBool("talk", true);
    }

    public void StopInteract()
    {
        inInteract = false;

        // Turn on Player
        FindObjectOfType<PlayerMovement>().IsAbleToMove = true;
        GetComponentInChildren<Animator>().SetBool("talk", false);

        onEndDialogue?.Invoke();
    }

    public void GiveItem()
    {
        if (objectGive == null)
            return;

        objectGive.SetActive(true);
        GetComponentInChildren<Animator>().SetBool("give", true);
    }
    public void OpenNewDialogue(DialogueScript script)
    {
        dialogue = script;
    }
}

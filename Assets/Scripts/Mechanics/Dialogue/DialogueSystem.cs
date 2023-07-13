using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance;

    [SerializeField] Dialogue dialoguePrefab;
    [SerializeField] Transform positionAndParentDialogue;

    private bool continueDialogues = false;
    private Dialogue dialogueCurrent;
    private DialogueScript dialogueScirptCurrent;
    private NPC currentNPC;
    private int indexPage = 0;

    CanvasGroup cg;

    private void Awake()
    {
        Instance = this;
        cg = GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        if(dialogueCurrent != null)
        {
            if(continueDialogues)
            {
                if(indexPage >= dialogueScirptCurrent.texts.Length -1)
                {
                    // Stop
                    StopDialogue();
                    currentNPC.StopInteract();
                    currentNPC = null;
                    dialogueScirptCurrent = null;
                    Destroy(dialogueCurrent.gameObject);
                    dialogueCurrent = null;
                }else
                {
                    indexPage++;
                    if (dialogueCurrent != null)
                        Destroy(dialogueCurrent.gameObject);
                    continueDialogues = true;
                    if(indexPage == dialogueScirptCurrent.texts.Length -1)
                        CreateDialogue(dialogueScirptCurrent.texts[indexPage].textPlayer, dialogueScirptCurrent.texts[indexPage].textNPC, true,
                            dialogueScirptCurrent.allOption, dialogueScirptCurrent.Id);
                    if (indexPage != dialogueScirptCurrent.texts.Length -1)
                        CreateDialogue(dialogueScirptCurrent.texts[indexPage].textPlayer, dialogueScirptCurrent.texts[indexPage].textNPC,false,null, dialogueScirptCurrent.Id);
                }

                continueDialogues = false;
            }
        }
    }
    public void StartDialogue(DialogueScript script, NPC npc)
    {
        dialogueScirptCurrent = script;
        currentNPC = npc;
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        indexPage = 0;
        if (indexPage == dialogueScirptCurrent.texts.Length - 1)
            CreateDialogue(script.texts[indexPage].textPlayer, script.texts[indexPage].textNPC, true,script.allOption, script.Id);
        if (indexPage != dialogueScirptCurrent.texts.Length - 1)
            CreateDialogue(script.texts[indexPage].textPlayer, script.texts[indexPage].textNPC,false,null, script.Id);

        GameManager.Instance.SetCursor(true);
        Debug.Log("Create Dialogue");
    }
    public void StopDialogue()
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        GameManager.Instance.SetCursor(false);
    }
    public void CreateDialogue(string textPlayer, string textNPC, bool lastPage = false, DialogueScript.OptionsDialogue[] textButtons = null, int idDialogue = 0)
    {
        dialogueCurrent = Instantiate(dialoguePrefab,positionAndParentDialogue);
        dialogueCurrent.transform.position = positionAndParentDialogue.position;

        dialogueCurrent.textNPC.text = textNPC;
        dialogueCurrent.textPlayer.text = textPlayer;

        if(lastPage && textButtons != null)
        {
            for (int i = 0; i < textButtons.Length; i++)
            {
                int io = textButtons[i].idOption;
                dialogueCurrent.allOption[i].gameObject.SetActive(true);
                dialogueCurrent.allOption[i].GetComponentInChildren<TMPro.TMP_Text>().text = textButtons[i].textOption;
                dialogueCurrent.allOption[i].onClick.AddListener(delegate { 
                    ClickOptions(idDialogue, io); 
                    ContinueText(); 
                });      
            }
        }
    }
    public void ClickOptions(int idDialogue, int idOption)
    {
        Debug.Log("Use " + idOption);

        if(idDialogue == 1)
        {
            if (idOption == 1)
            {
                currentNPC.GiveItem();
                currentNPC.OpenNewDialogue(SearchDialogueByID(2));
            }
        }
        if(idDialogue == 4)
        {
            switch(idOption)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            currentNPC.OpenNewDialogue(SearchDialogueByID(2));
        }
    }
    public void ContinueText() => continueDialogues = true;

    public DialogueScript SearchDialogueByID(int id)
    {
        DialogueScript[] allDialogue = GameManager.Instance.AllDialogue;
        for (int i = 0; i < allDialogue.Length; i++)
        {
            if (allDialogue[i].Id == id)
                return allDialogue[i];
        }

        return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueScript[] AllDialogue;

    [SerializeField] GameObject panelSkills;
    [SerializeField] CanvasGroup canvasInventory;

    private void Awake()
    {
        Instance = this;
        SetCursor(false);

        AllDialogue = Resources.LoadAll<DialogueScript>("Dialogue");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if(panelSkills.activeSelf)
            {
                panelSkills.SetActive(false);
                SetCursor(false);
            }
            else
            {
                panelSkills.SetActive(true);
                SetCursor(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(canvasInventory.alpha == 0f)
            {
                Inventory.Instance.OpenInventory(null);
            }
            else
            {
                Inventory.Instance.CloseInventory();
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetCursor(bool cursor)
    {
        Cursor.lockState = cursor.Equals(false) ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

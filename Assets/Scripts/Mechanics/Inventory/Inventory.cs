using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public Item CurrentUseItem { set; get; }

    private CanvasGroup canvasInventory;
    private ItemInSlot item;
    private Transform parentItem;

    [Header("Details")]
    [SerializeField] Transform gridItem;

    [Header("Object")]
    [SerializeField] Image imageItem;
    [SerializeField] TMPro.TMP_Text textItem;

    private void Awake()
    {
        Instance = this;
        canvasInventory = GetComponent<CanvasGroup>();
    }
    private void Update()
    {
        if(CurrentUseItem == null)
        {
            textItem.text = "";
            imageItem.color = new Color(0, 0, 0, 0);
        }else
        {
            imageItem.color = Color.white;
            textItem.text = "Current Item: " + CurrentUseItem.nameItem;
            imageItem.sprite = CurrentUseItem.iconItem;
        }
    }
    public void OpenInventory(ItemInSlot item)
    {
        canvasInventory.alpha = 1f;
        canvasInventory.blocksRaycasts = true;
        canvasInventory.interactable = true;

        if (item != null)
        {
            this.item = item;
            parentItem = item.transform.parent;

            item.gameObject.transform.SetParent(gridItem.transform);
            item.gameObject.transform.position = gridItem.transform.position;
            item.ResetStartPosAndRot();
            item.transform.localScale = new Vector3(1, 1, 1);
            item.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        GameManager.Instance.SetCursor(true);
    }
    public void CloseInventory()
    {
        canvasInventory.alpha = 0f;
        canvasInventory.blocksRaycasts = false;
        canvasInventory.interactable = false;

        if(item != null)
        {
            if(item.CurrentSlot != null)
            {
                if(item.CurrentSlot.Length > 0)
                {
                    if (item.CurrentSlot[0].IsInventorySlot == false)
                    {
                        item.gameObject.transform.SetParent(parentItem.transform);
                        item.gameObject.transform.position = Vector2.zero;
                    }
                    else
                    {
                        item.DestroyAfetrPick();
                    }
                }       else
                {
                    item.gameObject.transform.SetParent(parentItem.transform);
                    item.gameObject.transform.position = Vector2.zero;
                }
            }    else
            {
                item.gameObject.transform.SetParent(parentItem.transform);
                item.gameObject.transform.position = Vector2.zero;
            }
        }
        item = null;
        parentItem = null;
        GameManager.Instance.SetCursor(false);
    }
}
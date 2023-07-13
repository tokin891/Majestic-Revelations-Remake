using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCheck : MonoBehaviour
{
    private ItemCurrentDetail[] allCurrentItems;

    private void Awake()
    {
        allCurrentItems = GetComponentsInChildren<ItemCurrentDetail>();
    }

    private void Update()
    {
        Item currentUse = Inventory.Instance.CurrentUseItem;

        if(currentUse != null)
        {
            for (int i = 0; i < allCurrentItems.Length; i++)
            {
                if (allCurrentItems[i].Index == currentUse)
                    allCurrentItems[i].objectItem.SetActive(true);
                if (allCurrentItems[i].Index != currentUse)
                    allCurrentItems[i].objectItem.SetActive(false);
            }
        }else
        {
            for (int i = 0; i < allCurrentItems.Length; i++)
            {
                if (allCurrentItems[i].objectItem.activeSelf == true)
                    allCurrentItems[i].objectItem.SetActive(false);
            }
        }
    }
}

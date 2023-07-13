using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public bool IsFree;
    public bool IsInventorySlot = false;
    public ItemInSlot CurrentItem { set; get; }

    private Color awakeColor;

    private void Awake()
    {
        awakeColor = GetComponent<Image>().color;
    }

    public void SetColor(bool off)
    {
        if (off)
        {
            GetComponent<Image>().color = awakeColor;
        }
        else
        {
            if (IsFree)
            {
                GetComponent<Image>().color = Color.white;
            }
            else
                GetComponent<Image>().color = Color.red;
        }
    }
    public void Clear()
    {
        CurrentItem = null;
        IsFree = true;
    }
}

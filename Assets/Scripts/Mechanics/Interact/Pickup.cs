using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : InteractSystem
{
    [Header("Details")]
    [SerializeField] ItemInSlot itemOrGrid;

    public override void OnInteract()
    {
        // Pickup
        Inventory.Instance.OpenInventory(itemOrGrid);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Item : ScriptableObject
{
    public string nameItem;
    public string descItem;

    [Space]
    public Sprite iconItem;
    public int heightImage = 100;
    public int widthImage = 100;
}
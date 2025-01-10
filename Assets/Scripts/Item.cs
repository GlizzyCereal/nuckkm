using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite itemIcon;
    public bool isStackable;
    public int maxStack = 1;
    [TextArea]
    public string description;
}
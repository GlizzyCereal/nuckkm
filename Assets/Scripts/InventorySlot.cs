using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Item item;
    public int amount;
    
    public void AddItem(Item newItem, int count = 1)
    {
        item = newItem;
        amount = count;
        icon.sprite = newItem.itemIcon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
        icon.sprite = null;
        icon.enabled = false;
    }
}
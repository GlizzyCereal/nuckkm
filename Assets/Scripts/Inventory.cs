using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int quantity;
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string name, int quantity)
    {
        InventoryItem existingItem = items.Find(i => i.itemName == name);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            InventoryItem newItem = new InventoryItem
            {
                itemName = name,
                quantity = quantity
            };
            items.Add(newItem);
        }
    }

    public void RemoveItem(string name, int quantity)
    {
        InventoryItem item = items.Find(i => i.itemName == name);
        if (item == null) return;

        item.quantity -= quantity;
        if (item.quantity <= 0)
        {
            items.Remove(item);
        }
    }

    public int GetItemQuantity(string name)
    {
        InventoryItem item = items.Find(i => i.itemName == name);
        return item != null ? item.quantity : 0;
    }
}
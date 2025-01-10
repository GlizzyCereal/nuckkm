using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventorySlot[] slots;
    
    void Start()
    {
        slots = GetComponentsInChildren<InventorySlot>();
    }

    public bool AddItem(Item item)
    {
        // Find first empty slot
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item == null)
            {
                slots[i].AddItem(item);
                return true;
            }
        }
        return false; // Inventory is full
    }
}
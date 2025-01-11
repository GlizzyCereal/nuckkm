using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    
    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item list")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemButton;

    void Awake()
    {
        Singleton = this;
        giveItemButton.onClick.AddListener( delegate { SpawnInventoryItem(); } );
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if (_item == null)
        {
            int random = Random.Range(0, items.Length);
            _item = items[random];
        } 

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].MyItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    void Update()
    {
        if (carriedItem == null)
            return;
        
        carriedItem.transform.position = Input.mousePosition;
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            if(item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.MyItem.itemTag)
                return;
            item.activeSlot.SetItem(carriedItem);
        }

        if(item.activeSlot.myTag != SlotTag.None)
            EquipEquipment(item.activeSlot.myTag, null);
        
        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                Debug.Log("Equipped head item");
                break;
            case SlotTag.Chest:
                Debug.Log("Equipped chest item");
                break;
            case SlotTag.Legs:
                Debug.Log("Equipped legs item");
                break;
            case SlotTag.Feet:
                Debug.Log("Equipped feet item");
                break;
        }
    }
}

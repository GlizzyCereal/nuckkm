using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem MyItem { get; set; }
    public SlotTag myTag;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.carriedItem == null)
                return;
            
            if (myTag != SlotTag.None && Inventory.carriedItem.MyItem.itemTag != myTag)
                return;
            
            SetItem(Inventory.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {
        Inventory.carriedItem = null;

        item.activeSlot.MyItem = null;

        MyItem = item;
        MyItem.activeSlot = this;
        MyItem.transform.SetParent(transform);
        MyItem.canvasGroup.blocksRaycasts = true;

        if(myTag != SlotTag.None)
            Inventory.Singleton.EquipEquipment(myTag, MyItem);
    }
}

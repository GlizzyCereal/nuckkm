using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image imageIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item MyItem { get; set; }
    public InventorySlot activeSlot { get; set; }
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        imageIcon = GetComponent<Image>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.MyItem = this;
        MyItem = item;
        imageIcon.sprite = item.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
        }
    }
}

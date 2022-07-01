using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public int size = 1;
    public Item.itemType slotType;

    public List<Item> items = new List<Item>();
    private Item itemToShow;
    public bool autoHighlight = true;

    public void AddToSlot(Item item)
    {
        items.Add(item);
        itemToShow = item;
    }

    public void RemoveFromSlot(Item item)
    {
        items.Remove(item);
    }

    public void SelectSlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ToggleItemVisible(items[i] == itemToShow);
        }
    }

    public void DeSelectSlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].visible)
            {
                items[i].ToggleItemVisible(false);
            }
        }
    }
}

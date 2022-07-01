using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventorySlot : MonoBehaviour
{
    public int size = 1;
    public Item.itemType slotType;

    public List<Item> defaultItems = new List<Item>();
    public List<Item> items = new List<Item>();
    public Item highlightedItem;
    public bool autoHighlight = true;
    public bool highlighted = false;

    public bool occupied = false;

    public Action<InventorySlot> OnSlotEmpty;

    public void AddToSlot(Item item, int viewID)
    {
        Debug.Log("Adding " + item.name + " to slot " + name);
        if (items.Count == size)
        {
            //Full slot
            //Replace item in slot with new item
            RemoveFromSlot(highlightedItem);
        }

        occupied = true;
        items.Add(item);
        highlightedItem = item;
        Debug.Log("Setting highlighted item in slot " + slotType + " to item : " + item.name);
        item.GiveToPlayer(viewID);
    }

    public void DropFromSlot()
    {
        //TODO: byt till slot med vapen i om slot är tom

        if (highlightedItem != null)
        {
            RemoveFromSlot(highlightedItem);
        }
    }

    public void RemoveFromSlot(Item item)
    {
        Debug.Log("Removing " + item.name + " from slot " + name);
        if (item.visible == false)
        {
            item.ToggleItemVisible(true);
        }

        if(item == highlightedItem && items.Count == 1)
        {
            //Last item in slot
            //Select other slot
            OnSlotEmpty?.Invoke(this);
            occupied = false;
        }
        else
        {
            highlightedItem = items[0];
        }

        item.DropFromPlayer();
        items.Remove(item);
    }

    public void SelectSlot()
    {
        Debug.Log("Selecting slot " + name);
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ToggleItemVisible(items[i] == highlightedItem);
        }
    }

    public void DeSelectSlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ToggleItemVisible(false);
        }
    }
}

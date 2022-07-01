using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    public PhotonView pv;
    public Transform playerHand;

    public GameObject buyMenu;
    public static bool buyMenuOpen = false;

    public InventorySlot[] slots;
    private InventorySlot selectedSlot;


    private void Start()
    {
        if (pv.IsMine)
        {
            Debug.Log("Adding default items to inventory");
            foreach(var slot in slots)
            {
                Debug.Log("Slot : " + slot.name);
                slot.OnSlotEmpty += OnSlotBecameEmpty;
                //Add default item to slot
                if(slot.defaultItems.Count == 0)
                {
                    continue;
                }

                for (int i = 0; i < slot.defaultItems.Count; i++)
                {
                    Debug.Log($"Slot {slot.name} adding default");
                    var defaultItemToAdd = slot.defaultItems[i];
                    GetItem(defaultItemToAdd);
                }
            }

            SelectSlot(1);
        }
    }

    private void OnDisable()
    {
        if (pv.IsMine)
        {
            foreach (var slot in slots)
            {
                slot.OnSlotEmpty -= OnSlotBecameEmpty;
                for (int i = 0; i < slots[i].items.Count; i++)
                {
                    DropItem(slots[i]);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buyMenu.SetActive(!buyMenu.activeSelf);
            buyMenuOpen = buyMenu.activeSelf;
            Cursor.visible = buyMenuOpen;
            Cursor.lockState = buyMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            //Drop item
            if (selectedSlot != null) 
            {
                DropItem(selectedSlot);
            }
        }

        string inputString = Input.inputString;

        if(int.TryParse(inputString,out var result))
        {
            SelectSlot(result - 1);
        }
    }

    private void OnSlotBecameEmpty(InventorySlot slot)
    {
        if (slot == selectedSlot)
        {
            //Select other slot
            if (slots[0].occupied)
            {
                SelectSlot(0);
            }
            else if (slots[1].occupied)
            {
                SelectSlot(1);
            }
            else
            {
                //Default to knife
                SelectSlot(2);
            }
        }
    }
   
    public void CloseBuyMenu()
    {
        buyMenu.SetActive(false);
        buyMenuOpen = buyMenu.activeSelf;

        Cursor.visible = buyMenuOpen;
        Cursor.lockState = buyMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void GetItem(Item item)
    {
        if(item == null)
        {
            return;
        }

        Debug.Log("Give item " + item.name + " to " + name);

        var newItem = PhotonNetwork.Instantiate(item.name, playerHand.position, playerHand.rotation);
        var itemComponent = newItem.GetComponent<Item>();
        itemComponent.GiveToPlayer(pv.ViewID);

        PickUpItem(itemComponent);
    }

    public void PickUpItem(Item item)
    {
        Debug.Log("PickUpItem()");
        switch (item.type)
        {
            case Item.itemType.Primary:
                slots[0].AddToSlot(item, pv.ViewID);
                if (slots[0].autoHighlight)
                {
                    SelectSlot(0);
                }
                Debug.Log("PickUpItem(primary)");
                break;
            case Item.itemType.Secondary:
                slots[1].AddToSlot(item, pv.ViewID);
                if (slots[1].autoHighlight)
                {
                    SelectSlot(1);
                }

                Debug.Log("PickUpItem(secondary)");
                break;
            case Item.itemType.Knife:
                slots[2].AddToSlot(item, pv.ViewID);
                if (slots[2].autoHighlight)
                {
                    SelectSlot(2);
                }
                Debug.Log("PickUpItem(knife)");
                break;
            case Item.itemType.Utility:
                slots[3].AddToSlot(item, pv.ViewID);
                if (slots[3].autoHighlight)
                {
                    SelectSlot(3);
                }
                Debug.Log("PickUpItem(utility)");
                break;
            case Item.itemType.Special:
                slots[4].AddToSlot(item, pv.ViewID);
                if (slots[4].autoHighlight)
                {
                    SelectSlot(4);
                }
                Debug.Log("PickUpItem(special)");
                break;
        }
    }

    public void DropItem(InventorySlot slot)
    {
        if(slot == selectedSlot)
        {
            slot.DropFromSlot();
        }
    }

    private void SelectSlot(int inputIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i == inputIndex)
            {
                Debug.Log("Select slot " + slots[i]);
                slots[i].highlighted = true;
                slots[i].SelectSlot();
                selectedSlot = slots[i];
            }
            else
            {
                Debug.Log("DeSelect slot " + slots[i]);
                slots[i].highlighted = false;
                slots[i].DeSelectSlot();
            }
        }
    }
}

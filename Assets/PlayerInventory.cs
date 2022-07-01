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
    private Item selectedItem;


    private void Start()
    {
        if (pv.IsMine)
        {
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
        }

        string inputString = Input.inputString;

        if(int.TryParse(inputString,out var result))
        {
            SelectItem(result - 1);
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

        var newItem = PhotonNetwork.Instantiate(item.name, playerHand.position, playerHand.rotation);
        var itemComponent = newItem.GetComponent<Item>();
        itemComponent.GiveToPlayer(pv.ViewID);
    }

    public void PickUpItem(Item item)
    {
        item.GiveToPlayer(pv.ViewID);
    }

    public void DropItem(Item item)
    {
        if(item == selectedItem)
        {
            //Only drop if item is currently selected
            if (item.canDrop)
            {
                var slotOfItem = GetSlotOfItem(item);
                if(slotOfItem == null)
                {
                    Debug.LogError("Tried dropping an item with no found slot!!!");
                    return;
                }
                slotOfItem.RemoveFromSlot(item);
                item.DropFromPlayer();
            }
        }
    }

    private void OnItemPickedUp(Item item)
    {
        
    }

    private void SelectItem(int inputIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i == inputIndex)
            {
                slots[i].SelectSlot();
            }
            else
            {
                slots[i].DeSelectSlot();
            }
        }
    }

    private InventorySlot GetSlotOfItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].items.Contains(item))
            {
                return slots[i];
            }
        }

        return null;
    }
}

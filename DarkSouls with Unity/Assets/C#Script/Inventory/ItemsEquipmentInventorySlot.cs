using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class ItemsEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        ConsumableItem item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(ConsumableItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            //Remove current item
            //Add current item to inventory
            //Equip this new item
            //Remove this item from inventory
            if (uiManager.downItemsSlot01Selected)
            {
                uiManager.player.playerInventoryManager.consumableItemInventory.Add(uiManager.player.playerInventoryManager.ItemsInDownSlots[0]);
                uiManager.player.playerInventoryManager.ItemsInDownSlots[0] = item;
                uiManager.player.playerInventoryManager.consumableItemInventory.Remove(item);
            }
            else if (uiManager.downItemsSlot02Selected)
            {
                uiManager.player.playerInventoryManager.consumableItemInventory.Add(uiManager.player.playerInventoryManager.ItemsInDownSlots[1]);
                uiManager.player.playerInventoryManager.ItemsInDownSlots[1] = item;
                uiManager.player.playerInventoryManager.consumableItemInventory.Remove(item);
            }
            else if (uiManager.downItemsSlot03Selected)
            {
                uiManager.player.playerInventoryManager.consumableItemInventory.Add(uiManager.player.playerInventoryManager.ItemsInDownSlots[2]);
                uiManager.player.playerInventoryManager.ItemsInDownSlots[2] = item;
                uiManager.player.playerInventoryManager.consumableItemInventory.Remove(item);
            }
            else if (uiManager.downItemsSlot04Selected)
            {
                uiManager.player.playerInventoryManager.consumableItemInventory.Add(uiManager.player.playerInventoryManager.ItemsInDownSlots[3]);
                uiManager.player.playerInventoryManager.ItemsInDownSlots[3] = item;
                uiManager.player.playerInventoryManager.consumableItemInventory.Remove(item);
            }
            else { return; }

            uiManager.UpdateUI();

            uiManager.player.playerInventoryManager.currentConsumable = uiManager.player.playerInventoryManager.ItemsInDownSlots[uiManager.player.playerInventoryManager.currentDownItemIndex];

            uiManager.quickSlotsUI.UpdateCurrentConsumableIcon(uiManager.player.playerInventoryManager.currentConsumable);

            uiManager.equipmentWindowUI.LoadItemsOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateConsumableItemStats(item);
        }
    }
}
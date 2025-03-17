using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        WeaponItem item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(WeaponItem newItem)
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
            if (uiManager.rightHandSlot01Selected)
            {
                uiManager.player.playerInventoryManager.weaponsInventory.Add(uiManager.player.playerInventoryManager.weaponsInRightHandSlots[0]);
                uiManager.player.playerInventoryManager.weaponsInRightHandSlots[0] = item;
                uiManager.player.playerInventoryManager.weaponsInventory.Remove(item);
            }
            else if(uiManager.rightHandSlot02Selected)
            {
                uiManager.player.playerInventoryManager.weaponsInventory.Add(uiManager.player.playerInventoryManager.weaponsInRightHandSlots[1]);
                uiManager.player.playerInventoryManager.weaponsInRightHandSlots[1] = item;
                uiManager.player.playerInventoryManager.weaponsInventory.Remove(item);
            }
            else if(uiManager.leftHandSlot01Selected)
            {
                uiManager.player.playerInventoryManager.weaponsInventory.Add(uiManager.player.playerInventoryManager.weaponsInLeftHandSlots[0]);
                uiManager.player.playerInventoryManager.weaponsInLeftHandSlots[0] = item;
                uiManager.player.playerInventoryManager.weaponsInventory.Remove(item);
            }
            else if(uiManager.leftHandSlot02Selected)
            {
                uiManager.player.playerInventoryManager.weaponsInventory.Add(uiManager.player.playerInventoryManager.weaponsInLeftHandSlots[1]);
                uiManager.player.playerInventoryManager.weaponsInLeftHandSlots[1] = item;
                uiManager.player.playerInventoryManager.weaponsInventory.Remove(item);
            }
            else { return; }

            uiManager.UpdateUI();

            uiManager.player.playerInventoryManager.rightWeapon = uiManager.player.playerInventoryManager.weaponsInRightHandSlots[uiManager.player.playerInventoryManager.currentRightWeaponIndex];
            uiManager.player.playerInventoryManager.leftWeapon = uiManager.player.playerInventoryManager.weaponsInLeftHandSlots[uiManager.player.playerInventoryManager.currentLeftWeaponIndex];

            uiManager.player.playerWeaponSlotManager.LoadWeaponOnSlot(uiManager.player.playerInventoryManager.rightWeapon, false);
            uiManager.player.playerWeaponSlotManager.LoadWeaponOnSlot(uiManager.player.playerInventoryManager.leftWeapon, true);

            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateWeaponItemStats(item);
        }
    }
}
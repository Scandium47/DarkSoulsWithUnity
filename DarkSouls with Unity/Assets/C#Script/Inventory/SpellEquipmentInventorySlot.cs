using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class SpellEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        SpellItem item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(SpellItem newItem)
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
            if (uiManager.upSpellSlot01Selected)
            {
                uiManager.player.playerInventoryManager.spellInventory.Add(uiManager.player.playerInventoryManager.SpellInUpSlots[0]);
                uiManager.player.playerInventoryManager.SpellInUpSlots[0] = item;
                uiManager.player.playerInventoryManager.spellInventory.Remove(item);
            }
            else if (uiManager.upSpellSlot02Selected)
            {
                uiManager.player.playerInventoryManager.spellInventory.Add(uiManager.player.playerInventoryManager.SpellInUpSlots[1]);
                uiManager.player.playerInventoryManager.SpellInUpSlots[1] = item;
                uiManager.player.playerInventoryManager.spellInventory.Remove(item);
            }
            else if (uiManager.upSpellSlot03Selected)
            {
                uiManager.player.playerInventoryManager.spellInventory.Add(uiManager.player.playerInventoryManager.SpellInUpSlots[2]);
                uiManager.player.playerInventoryManager.SpellInUpSlots[2] = item;
                uiManager.player.playerInventoryManager.spellInventory.Remove(item);
            }
            else if (uiManager.upSpellSlot04Selected)
            {
                uiManager.player.playerInventoryManager.spellInventory.Add(uiManager.player.playerInventoryManager.SpellInUpSlots[3]);
                uiManager.player.playerInventoryManager.SpellInUpSlots[3] = item;
                uiManager.player.playerInventoryManager.spellInventory.Remove(item);
            }
            else { return; }

            uiManager.UpdateUI();

            uiManager.player.playerInventoryManager.currentSpell = uiManager.player.playerInventoryManager.SpellInUpSlots[uiManager.player.playerInventoryManager.currentUpSpellIndex];

            uiManager.quickSlotsUI.UpdateCurrentSpellIcon(uiManager.player.playerInventoryManager.currentSpell);

            uiManager.equipmentWindowUI.LoadSpellOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateSpellItemStats(item);
        }
    }
}
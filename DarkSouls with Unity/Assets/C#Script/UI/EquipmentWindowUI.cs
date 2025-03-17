using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EquipmentWindowUI : MonoBehaviour
    {
        public WeaponEquipmentSlotUI[] weaponEquipmentSlotUI;
        public HeadEquipmentSlotUI headEquipmentSlotUI;
        public BodyEquipmentSlotUI bodyEquipmentSlotUI;
        public LegEquipmentSlotUI legEquipmentSlotUI;
        public HandEquipmentSlotUI handEquipmentSlotUI;
        public SpellEquipmentSlotUI[] spellEquipmentSlotUI;
        public ItemsEquipmentSlotUI[] itemsEquipmentSlotUI;

        public void LoadWeaponOnEquipmentScreen(PlayerInventoryManager playerInventory)
        {
            for(int i =0; i< weaponEquipmentSlotUI.Length; i++)
            {
                if (weaponEquipmentSlotUI[i].rightHandSlot01)
                {
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                }
                else if (weaponEquipmentSlotUI[i].rightHandSlot02)
                {
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if (weaponEquipmentSlotUI[i].leftHandSlot01)
                {
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else if(weaponEquipmentSlotUI[i].leftHandSlot02)
                {
                    weaponEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
                else
                {
                    return;
                }
            }
        }

        public void LoadArmorOnEquipmentScreen(PlayerInventoryManager playerInventory)
        {
            if(playerInventory.currentHelmetEquipment != null)
            {
                headEquipmentSlotUI.AddItem(playerInventory.currentHelmetEquipment);
            }
            else
            {
                headEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentBodyEquipment != null)
            {
                bodyEquipmentSlotUI.AddItem(playerInventory.currentBodyEquipment);
            }
            else
            {
                bodyEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentLegEquipment != null)
            {
                legEquipmentSlotUI.AddItem(playerInventory.currentLegEquipment);
            }
            else
            {
                legEquipmentSlotUI.ClearItem();
            }

            if (playerInventory.currentHandEquipment != null)
            {
                handEquipmentSlotUI.AddItem(playerInventory.currentHandEquipment);
            }
            else
            {
                handEquipmentSlotUI.ClearItem();
            }
        }

        public void LoadSpellOnEquipmentScreen(PlayerInventoryManager playerInventory)
        {
            for (int i = 0; i < spellEquipmentSlotUI.Length; i++)
            {
                if (spellEquipmentSlotUI[i].upSpellSlot01)
                {
                    spellEquipmentSlotUI[i].AddItem(playerInventory.SpellInUpSlots[0]);
                }
                else if (spellEquipmentSlotUI[i].upSpellSlot02)
                {
                    spellEquipmentSlotUI[i].AddItem(playerInventory.SpellInUpSlots[1]);
                }
                else if (spellEquipmentSlotUI[i].upSpellSlot03)
                {
                    spellEquipmentSlotUI[i].AddItem(playerInventory.SpellInUpSlots[2]);
                }
                else if (spellEquipmentSlotUI[i].upSpellSlot04)
                {
                    spellEquipmentSlotUI[i].AddItem(playerInventory.SpellInUpSlots[3]);
                }
                else
                {
                    return;
                }
            }
        }

        public void LoadItemsOnEquipmentScreen(PlayerInventoryManager playerInventory)
        {
            for (int i = 0; i < itemsEquipmentSlotUI.Length; i++)
            {
                if (itemsEquipmentSlotUI[i].downItemsSlot01)
                {
                    itemsEquipmentSlotUI[i].AddItem(playerInventory.ItemsInDownSlots[0]);
                }
                else if (itemsEquipmentSlotUI[i].downItemsSlot02)
                {
                    itemsEquipmentSlotUI[i].AddItem(playerInventory.ItemsInDownSlots[1]);
                }
                else if (itemsEquipmentSlotUI[i].downItemsSlot03)
                {
                    itemsEquipmentSlotUI[i].AddItem(playerInventory.ItemsInDownSlots[2]);
                }
                else if (itemsEquipmentSlotUI[i].downItemsSlot04)
                {
                    itemsEquipmentSlotUI[i].AddItem(playerInventory.ItemsInDownSlots[3]);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
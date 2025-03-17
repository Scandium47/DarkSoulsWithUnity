using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class HandEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        HandEquipment item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(HandEquipment newItem)
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
            if (uiManager.handEquipmentSlotSelected)
            {
                //将当前手甲装备（如果有的话）加到手甲槽
                if (uiManager.player.playerInventoryManager.currentHandEquipment != null)
                {
                    uiManager.player.playerInventoryManager.handEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentHandEquipment);
                }
                //移除当前手甲 & 替换成新手甲
                uiManager.player.playerInventoryManager.currentHandEquipment = item;
                //从手甲槽内移除新手甲
                uiManager.player.playerInventoryManager.handEquipmentInventory.Remove(item);
                //全部装备
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //更新新手甲到UI/装备显示页面
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
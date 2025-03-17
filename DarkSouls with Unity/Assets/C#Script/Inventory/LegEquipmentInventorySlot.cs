using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class LegEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        LegEquipment item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(LegEquipment newItem)
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
            if (uiManager.legEquipmentSlotSelected)
            {
                //将当前腿甲装备（如果有的话）加到腿甲槽
                if (uiManager.player.playerInventoryManager.currentLegEquipment != null)
                {
                    uiManager.player.playerInventoryManager.legEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentLegEquipment);
                }
                //移除当前腿甲 & 替换成新腿甲
                uiManager.player.playerInventoryManager.currentLegEquipment = item;
                //从腿甲槽内移除新腿甲
                uiManager.player.playerInventoryManager.legEquipmentInventory.Remove(item);
                //全部装备
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //更新新腿甲到UI/装备显示页面
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
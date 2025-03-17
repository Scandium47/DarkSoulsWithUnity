using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class HeadEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        HelmetEquipment item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(HelmetEquipment newItem)
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
            if (uiManager.headEquipmentSlotSelected)
            {
                //将当前头盔装备（如果有的话）加到头盔槽
                if(uiManager.player.playerInventoryManager.currentHelmetEquipment != null)
                {
                    uiManager.player.playerInventoryManager.headEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentHelmetEquipment);
                }
                //移除当前头盔 & 替换成新头盔
                uiManager.player.playerInventoryManager.currentHelmetEquipment = item;
                //从头盔槽内移除新头盔
                uiManager.player.playerInventoryManager.headEquipmentInventory.Remove(item);
                //全部装备
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //更新新头盔到UI/装备显示页面
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
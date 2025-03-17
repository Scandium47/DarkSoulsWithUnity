using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class BodyEquipmentInventorySlot : MonoBehaviour
    {
        UIManager uiManager;

        public Image icon;
        BodyEquipment item;

        private void Awake()
        {
            uiManager = GetComponentInParent<UIManager>();
        }
        public void AddItem(BodyEquipment newItem)
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
            if (uiManager.bodyEquipmentSlotSelected)
            {
                //将当前胸甲装备（如果有的话）加到胸甲槽
                if (uiManager.player.playerInventoryManager.currentBodyEquipment != null)
                {
                    uiManager.player.playerInventoryManager.bodyEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentBodyEquipment);
                }
                //移除当前胸甲 & 替换成新胸甲
                uiManager.player.playerInventoryManager.currentBodyEquipment = item;
                //从胸甲槽内移除新胸甲
                uiManager.player.playerInventoryManager.bodyEquipmentInventory.Remove(item);
                //全部装备
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //更新新胸甲到UI/装备显示页面
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
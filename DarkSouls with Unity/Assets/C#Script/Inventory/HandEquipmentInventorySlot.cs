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
                //����ǰ�ּ�װ��������еĻ����ӵ��ּײ�
                if (uiManager.player.playerInventoryManager.currentHandEquipment != null)
                {
                    uiManager.player.playerInventoryManager.handEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentHandEquipment);
                }
                //�Ƴ���ǰ�ּ� & �滻�����ּ�
                uiManager.player.playerInventoryManager.currentHandEquipment = item;
                //���ּײ����Ƴ����ּ�
                uiManager.player.playerInventoryManager.handEquipmentInventory.Remove(item);
                //ȫ��װ��
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //�������ּ׵�UI/װ����ʾҳ��
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
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
                //����ǰ�ؼ�װ��������еĻ����ӵ��ؼײ�
                if (uiManager.player.playerInventoryManager.currentBodyEquipment != null)
                {
                    uiManager.player.playerInventoryManager.bodyEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentBodyEquipment);
                }
                //�Ƴ���ǰ�ؼ� & �滻�����ؼ�
                uiManager.player.playerInventoryManager.currentBodyEquipment = item;
                //���ؼײ����Ƴ����ؼ�
                uiManager.player.playerInventoryManager.bodyEquipmentInventory.Remove(item);
                //ȫ��װ��
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //�������ؼ׵�UI/װ����ʾҳ��
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
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
                //����ǰ�ȼ�װ��������еĻ����ӵ��ȼײ�
                if (uiManager.player.playerInventoryManager.currentLegEquipment != null)
                {
                    uiManager.player.playerInventoryManager.legEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentLegEquipment);
                }
                //�Ƴ���ǰ�ȼ� & �滻�����ȼ�
                uiManager.player.playerInventoryManager.currentLegEquipment = item;
                //���ȼײ����Ƴ����ȼ�
                uiManager.player.playerInventoryManager.legEquipmentInventory.Remove(item);
                //ȫ��װ��
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //�������ȼ׵�UI/װ����ʾҳ��
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
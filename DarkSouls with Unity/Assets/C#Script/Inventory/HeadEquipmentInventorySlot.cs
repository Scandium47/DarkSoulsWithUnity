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
                //����ǰͷ��װ��������еĻ����ӵ�ͷ����
                if(uiManager.player.playerInventoryManager.currentHelmetEquipment != null)
                {
                    uiManager.player.playerInventoryManager.headEquipmentInventory.Add(uiManager.player.playerInventoryManager.currentHelmetEquipment);
                }
                //�Ƴ���ǰͷ�� & �滻����ͷ��
                uiManager.player.playerInventoryManager.currentHelmetEquipment = item;
                //��ͷ�������Ƴ���ͷ��
                uiManager.player.playerInventoryManager.headEquipmentInventory.Remove(item);
                //ȫ��װ��
                uiManager.player.playerEquipmentManager.EquipAllArmor();
            }
            else { return; }

            //������ͷ����UI/װ����ʾҳ��
            uiManager.equipmentWindowUI.LoadArmorOnEquipmentScreen(uiManager.player.playerInventoryManager);
            uiManager.ResetAllSelectedSlots();
        }

        public void SelectThisSlot()
        {
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
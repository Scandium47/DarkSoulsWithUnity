using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class HandEquipmentSlotUI : MonoBehaviour
    {
        UIManager uiManager;
        public Image icon;
        HandEquipment item;

        private void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        public void AddItem(HandEquipment handEquipment)
        {
            if (handEquipment != null)
            {
                item = handEquipment;
                icon.sprite = item.itemIcon;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
            else
            {
                ClearItem();
            }
        }

        public void ClearItem()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
        }

        public void SelectThisSlot()
        {
            uiManager.handEquipmentSlotSelected = true;
            //打开装备面板说明
            uiManager.itemStatsWindowUI.UpdateArmorItemStats(item);
        }
    }
}
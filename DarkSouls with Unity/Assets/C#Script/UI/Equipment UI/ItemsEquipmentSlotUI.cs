using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class ItemsEquipmentSlotUI : MonoBehaviour
    {
        UIManager uiManager;
        public Image icon;
        ConsumableItem item;

        public bool downItemsSlot01;
        public bool downItemsSlot02;
        public bool downItemsSlot03;
        public bool downItemsSlot04;

        private void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        public void AddItem(ConsumableItem newItem)
        {
            if (newItem != null)
            {
                item = newItem;
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
            uiManager.ResetAllSelectedSlots();

            if (downItemsSlot01)
            {
                uiManager.downItemsSlot01Selected = true;
            }
            else if (downItemsSlot02)
            {
                uiManager.downItemsSlot02Selected = true;
            }
            else if (downItemsSlot03)
            {
                uiManager.downItemsSlot03Selected = true;
            }
            else
            {
                uiManager.downItemsSlot04Selected = true;
            }
            uiManager.itemStatsWindowUI.UpdateConsumableItemStats(item);
        }
    }
}
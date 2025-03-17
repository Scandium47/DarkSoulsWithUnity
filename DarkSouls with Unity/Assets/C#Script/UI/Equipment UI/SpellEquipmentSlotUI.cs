using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class SpellEquipmentSlotUI : MonoBehaviour
    {
        UIManager uiManager;
        public Image icon;
        SpellItem spell;

        public bool upSpellSlot01;
        public bool upSpellSlot02;
        public bool upSpellSlot03;
        public bool upSpellSlot04;

        private void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        public void AddItem(SpellItem newSpell)
        {
            if (newSpell != null)
            {
                spell = newSpell;
                icon.sprite = spell.itemIcon;
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
            spell = null;
            icon.sprite = null;
            icon.enabled = false;
        }

        public void SelectThisSlot()
        {
            uiManager.ResetAllSelectedSlots();

            if (upSpellSlot01)
            {
                uiManager.upSpellSlot01Selected = true;
            }
            else if (upSpellSlot02)
            {
                uiManager.upSpellSlot02Selected = true;
            }
            else if (upSpellSlot03)
            {
                uiManager.upSpellSlot03Selected = true;
            }
            else
            {
                uiManager.upSpellSlot04Selected = true;
            }
            uiManager.itemStatsWindowUI.UpdateSpellItemStats(spell);
        }
    }
}
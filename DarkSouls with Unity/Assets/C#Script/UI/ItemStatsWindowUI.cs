using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SG
{
    public class ItemStatsWindowUI : MonoBehaviour
    {
        public TextMeshProUGUI itemNameText;
        public Image itemIconImage;

        [Header("Equipment Stats Windows")]
        public GameObject weaponStats;
        public GameObject armorStats;
        public GameObject spellStats;
        public GameObject consumableItemsStats;

        [Header("Weapon Stats")]
        public TextMeshProUGUI physicalDamageText;
        public TextMeshProUGUI magicDamageText;
        public TextMeshProUGUI fireDamageText;
        public TextMeshProUGUI lightningDamageText;
        public TextMeshProUGUI darknessDamageText;

        public TextMeshProUGUI physicalAbsorptionText;
        public TextMeshProUGUI magicAbsorptionText;
        public TextMeshProUGUI fireAbsorptionText;
        public TextMeshProUGUI lightningAbsorptionText;
        public TextMeshProUGUI darknessAbsorptionText;

        [Header("Armor Stats")]
        public TextMeshProUGUI armorPhysicalAbsorptionText;
        public TextMeshProUGUI armorMagicAbsorptionText;
        public TextMeshProUGUI armorFireAbsorptionText;
        public TextMeshProUGUI armorLightningAbsorptionText;
        public TextMeshProUGUI armorDarknessAbsorptionText;

        public TextMeshProUGUI armorPoisonResistanceText;

        [Header("Spell Stats")]
        public TextMeshProUGUI spellDamageText;
        public TextMeshProUGUI spellDescriptionText;
        //Ч����FP����

        [Header("Consumable Items Stats")]
        public TextMeshProUGUI itemsDamageText;
        public TextMeshProUGUI itemsDescriptionText;


        //����������Ʒ״̬
        public void UpdateWeaponItemStats(WeaponItem weapon)
        {
            CloseAllStatWindlws();
            if (weapon != null)
            {
                if (weapon.itemName != null)
                {
                    itemNameText.text = weapon.itemName;
                }
                else
                {
                    itemNameText.text = string.Empty;   //""
                }

                if (weapon.itemIcon != null)
                {
                    itemIconImage.gameObject.SetActive(true);
                    itemIconImage.enabled = true;
                    itemIconImage.sprite = weapon.itemIcon;
                }
                else
                {
                    itemIconImage.gameObject.SetActive(false);
                    itemIconImage.enabled = false;
                    itemIconImage.sprite = null;
                }
                //��������״̬����ʾ������״̬UI
                physicalDamageText.text = weapon.physicalDamage.ToString();
                magicDamageText.text = weapon.magicDamage.ToString();
                fireDamageText.text = weapon.fireDamage.ToString();
                fireDamageText.text = weapon.fireDamage.ToString();
                fireDamageText.text = weapon.fireDamage.ToString();
                physicalAbsorptionText.text = weapon.physicalBlockingDamageAbsorption.ToString();
                magicAbsorptionText.text = weapon.magicBlockingDamageAbsorption.ToString();
                fireAbsorptionText.text = weapon.fireBlockingDamageAbsorption.ToString();
                lightningAbsorptionText.text = weapon.lightningBlockingDamageAbsorption.ToString();
                darknessAbsorptionText.text = weapon.darknessBlockingDamageAbsorption.ToString();
                //...

                weaponStats.SetActive(true);
            }
            else
            {
                itemNameText.text = string.Empty;
                itemIconImage.gameObject.SetActive(false);
                itemIconImage.sprite = null;
                weaponStats.SetActive(false);
            }

        }

        //����װ����Ʒ״̬
        public void UpdateArmorItemStats(EquipmentItem armor)
        {
            CloseAllStatWindlws();
            if (armor != null)
            {
                if (armor.itemName != null)
                {
                    itemNameText.text = armor.itemName;
                }
                else
                {
                    itemNameText.text = string.Empty;   //""
                }

                if (armor.itemIcon != null)
                {
                    itemIconImage.gameObject.SetActive(true);
                    itemIconImage.enabled = true;
                    itemIconImage.sprite = armor.itemIcon;
                }
                else
                {
                    itemIconImage.gameObject.SetActive(false);
                    itemIconImage.enabled = false;
                    itemIconImage.sprite = null;
                }
                //��������״̬����ʾ������״̬UI
                armorPhysicalAbsorptionText.text = armor.physicalDefense.ToString();
                armorMagicAbsorptionText.text = armor.MagicDefense.ToString();
                armorFireAbsorptionText.text = armor.FireDefense.ToString();
                armorLightningAbsorptionText.text = armor.LightningDefense.ToString();
                armorDarknessAbsorptionText.text = armor.DarknessDefense.ToString();

                armorPoisonResistanceText.text = armor.poisonResistance.ToString();
                //...

                armorStats.SetActive(true);
            }
            else
            {
                itemNameText.text = string.Empty;
                itemIconImage.gameObject.SetActive(false);
                itemIconImage.sprite = null;
                armorStats.SetActive(false);
            }

        }

        //���·�����������״̬
        public void UpdateSpellItemStats(SpellItem spell)
        {
            CloseAllStatWindlws();
            if (spell != null)
            {
                if (spell.itemName != null)
                {
                    itemNameText.text = spell.itemName;
                }
                else
                {
                    itemNameText.text = string.Empty;   //""
                }

                if (spell.itemIcon != null)
                {
                    itemIconImage.gameObject.SetActive(true);
                    itemIconImage.enabled = true;
                    itemIconImage.sprite = spell.itemIcon;
                }
                else
                {
                    itemIconImage.gameObject.SetActive(false);
                    itemIconImage.enabled = false;
                    itemIconImage.sprite = null;
                }
                //��������״̬����ʾ������״̬UI
                spellDescriptionText.text = spell.spellDescription.ToString();
                //...

                spellStats.SetActive(true);
            }
            else
            {
                itemNameText.text = string.Empty;
                itemIconImage.gameObject.SetActive(false);
                itemIconImage.sprite = null;
                spellStats.SetActive(false);
            }

        }

        //������Ʒ״̬
        public void UpdateConsumableItemStats(ConsumableItem item)
        {
            CloseAllStatWindlws();
            if (item != null)
            {
                if (item.itemName != null)
                {
                    itemNameText.text = item.itemName;
                }
                else
                {
                    itemNameText.text = string.Empty;   //""
                }

                if (item.itemIcon != null)
                {
                    itemIconImage.gameObject.SetActive(true);
                    itemIconImage.enabled = true;
                    itemIconImage.sprite = item.itemIcon;
                }
                else
                {
                    itemIconImage.gameObject.SetActive(false);
                    itemIconImage.enabled = false;
                    itemIconImage.sprite = null;
                }
                //��������״̬����ʾ������״̬UI
                itemsDescriptionText.text = item.itemsDescription.ToString();
                //...

                consumableItemsStats.SetActive(true);
            }
            else
            {
                itemNameText.text = string.Empty;
                itemIconImage.gameObject.SetActive(false);
                itemIconImage.sprite = null;
                consumableItemsStats.SetActive(false);
            }

        }

        //���½�ָ��Ʒ״̬

        //����ʾǰ�ر���������������ʾ����
        private void CloseAllStatWindlws()
        {
            weaponStats.SetActive(false);
            armorStats.SetActive(false);
            spellStats.SetActive(false);
            consumableItemsStats.SetActive(false);
        }
    }
}
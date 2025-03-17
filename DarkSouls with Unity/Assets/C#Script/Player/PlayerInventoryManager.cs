using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public PlayerManager player;

        public List<WeaponItem> weaponsInventory;                       //背包内武器
        public List<HelmetEquipment> headEquipmentInventory;    //背包内头盔
        public List<BodyEquipment> bodyEquipmentInventory;      //背包内胸甲
        public List<LegEquipment> legEquipmentInventory;            //背包内腿甲
        public List<HandEquipment> handEquipmentInventory;      //背包内手甲
        public List<SpellItem> spellInventory;                            //背包内法术
        public List<ConsumableItem> consumableItemInventory;    //背包内物品

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        //private void Start()
        //{
        //    rightWeapon = weaponsInRightHandSlots[0];
        //    leftWeapon = weaponsInLeftHandSlots[0];
        //    playerWeaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        //    playerWeaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        //    /*
        //    rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
        //    leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
        //    weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        //    weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);          
        //    */
        //}

        public void ChangeRightWeapon()
        {

            currentRightWeaponIndex = currentRightWeaponIndex + 1;
            
            if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null) 
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false); //false指右手武器 bool isLeft
            }           //初始或武器未装备时 c+1=0 ，装备右手武器
            else if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null) 
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }           //初始或武器未装备时，c+1=0，但装备里没设置武器，c+1=1，此时武器槽没有武器，c=1时切换下一把武器，转到下个if语句，如果也没有，转到未装备

            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }           //第一个武器没有，c=1装备第二个武器
            else
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }           //第二个武器也没有，转到未装备

            if(currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1) 
            {
                currentRightWeaponIndex = -1;
                rightWeapon = character.characterWeaponSlotManager.unarmedWeapon;
                character.characterWeaponSlotManager.LoadWeaponOnSlot(character.characterWeaponSlotManager.unarmedWeapon, false);
            }           //武器未装备，c=1 > 0 ，重置c=-1
        }

        public void ChangeLeftWeapon()
        {

            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true); //true左手武器 bool isLeft
            }
            else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }

            else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }
            else
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }

            if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
            {
                currentLeftWeaponIndex = -1;
                leftWeapon = character.characterWeaponSlotManager.unarmedWeapon;
                character.characterWeaponSlotManager.LoadWeaponOnSlot(character.characterWeaponSlotManager.unarmedWeapon, true);
            }
        }

        public void ChangeSpell()
        {
            int startIndex = currentUpSpellIndex;
            int length = SpellInUpSlots.Length;

            // 遍历所有可能的索引
            for (int i = 0; i < length; i++)
            {
                currentUpSpellIndex = (currentUpSpellIndex + 1) % length; // 循环递增

                if (SpellInUpSlots[currentUpSpellIndex] != null)
                {
                    currentSpell = SpellInUpSlots[currentUpSpellIndex];
                    player.uiManager.quickSlotsUI.UpdateCurrentSpellIcon(currentSpell);
                    return;
                }
            }

            // 所有slot均为null
            currentUpSpellIndex = -1;
            currentSpell = null;
            player.uiManager.quickSlotsUI.UpdateCurrentSpellIcon(null);
        }


        public void ChangeConsumableItem()
        {
            int startIndex = currentDownItemIndex;
            int length = ItemsInDownSlots.Length;

            for (int i = 0; i < length; i++)
            {
                currentDownItemIndex = (currentDownItemIndex + 1) % length;

                if (ItemsInDownSlots[currentDownItemIndex] != null)
                {
                    currentConsumable = ItemsInDownSlots[currentDownItemIndex];
                    player.uiManager.quickSlotsUI.UpdateCurrentConsumableIcon(currentConsumable);
                    return;
                }
            }

            // 所有slot均为null
            currentDownItemIndex = -1;
            currentConsumable = null;
            player.uiManager.quickSlotsUI.UpdateCurrentConsumableIcon(null);
        }
    }
}

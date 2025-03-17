using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public PlayerManager player;

        public List<WeaponItem> weaponsInventory;                       //����������
        public List<HelmetEquipment> headEquipmentInventory;    //������ͷ��
        public List<BodyEquipment> bodyEquipmentInventory;      //�������ؼ�
        public List<LegEquipment> legEquipmentInventory;            //�������ȼ�
        public List<HandEquipment> handEquipmentInventory;      //�������ּ�
        public List<SpellItem> spellInventory;                            //�����ڷ���
        public List<ConsumableItem> consumableItemInventory;    //��������Ʒ

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
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false); //falseָ�������� bool isLeft
            }           //��ʼ������δװ��ʱ c+1=0 ��װ����������
            else if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null) 
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }           //��ʼ������δװ��ʱ��c+1=0����װ����û����������c+1=1����ʱ������û��������c=1ʱ�л���һ��������ת���¸�if��䣬���Ҳû�У�ת��δװ��

            else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }           //��һ������û�У�c=1װ���ڶ�������
            else
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }           //�ڶ�������Ҳû�У�ת��δװ��

            if(currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1) 
            {
                currentRightWeaponIndex = -1;
                rightWeapon = character.characterWeaponSlotManager.unarmedWeapon;
                character.characterWeaponSlotManager.LoadWeaponOnSlot(character.characterWeaponSlotManager.unarmedWeapon, false);
            }           //����δװ����c=1 > 0 ������c=-1
        }

        public void ChangeLeftWeapon()
        {

            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                character.characterWeaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true); //true�������� bool isLeft
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

            // �������п��ܵ�����
            for (int i = 0; i < length; i++)
            {
                currentUpSpellIndex = (currentUpSpellIndex + 1) % length; // ѭ������

                if (SpellInUpSlots[currentUpSpellIndex] != null)
                {
                    currentSpell = SpellInUpSlots[currentUpSpellIndex];
                    player.uiManager.quickSlotsUI.UpdateCurrentSpellIcon(currentSpell);
                    return;
                }
            }

            // ����slot��Ϊnull
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

            // ����slot��Ϊnull
            currentDownItemIndex = -1;
            currentConsumable = null;
            player.uiManager.quickSlotsUI.UpdateCurrentConsumableIcon(null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterInventoryManager : MonoBehaviour
    {
        protected CharacterManager character;

        [Header("Current Item Being Used")]
        public Item currentItemBeingUsed;

        [Header("Quick Slot Items")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;
        public ConsumableItem currentConsumable;
        public RangedAmmoItem currentAmmo;

        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public BodyEquipment currentBodyEquipment;
        public LegEquipment currentLegEquipment;
        public HandEquipment currentHandEquipment;
        public RingItem ringSlot01;
        public RingItem ringSlot02;
        public RingItem ringSlot03;
        public RingItem ringSlot04;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1]; //[1] 表示新创建一个能容纳一个WeaponItem类型的数组，因为武器槽只同时显示一种
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];
        public SpellItem[] SpellInUpSlots = new SpellItem[3];
        public ConsumableItem[] ItemsInDownSlots = new ConsumableItem[3];

        public int currentRightWeaponIndex = 0;//空手-1 ，后修改为开局手持 所以unity设为0 下同
        public int currentLeftWeaponIndex = 0;
        public int currentUpSpellIndex = 0;
        public int currentDownItemIndex = 0;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            character.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
            PlayerManager player = character as PlayerManager;

            if(player != null)
            {
                player.uiManager.quickSlotsUI.UpdateCurrentSpellIcon(currentSpell);
                player.uiManager.quickSlotsUI.UpdateCurrentConsumableIcon(currentConsumable);
            }
        }

        //在加载装备后加载戒指
        public virtual void LoadRingEffects()
        {
            if(ringSlot01 != null)
            {
                ringSlot01.EquipRing(character);
            }
            if (ringSlot02 != null)
            {
                ringSlot02.EquipRing(character);
            }
            if (ringSlot03 != null)
            {
                ringSlot03.EquipRing(character);
            }
            if (ringSlot04 != null)
            {
                ringSlot04.EquipRing(character);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Consumeables/Weapon Buff")]
    public class WeaponBuffConsumeableItem : ConsumableItem
    {
        [Header("Effect")]
        [SerializeField] WeaponBuffEffect weaponBuffEffect;

        [Header("Buff SFX")]
        [SerializeField] AudioClip buffTriggerSound;

        public override void AttemtToConsumeItem(PlayerManager player)
        {
            //如果不能用该物品，返回
            if (!CanIUseThisItem(player))
                return;

            if (currentItemAmount > 0)
            {
                player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting, true);
            }
            else
            {
                player.playerAnimatorManager.PlayTargetAnimation("Drink_Empty", true);
            }
        }

        public override void SucessfullyConsumeItem(PlayerManager player)
        {
            base.SucessfullyConsumeItem(player);

            player.characterSoundFXManager.PlaySoundFX(buffTriggerSound);
            WeaponBuffEffect weaponBuff = Instantiate(weaponBuffEffect);
            weaponBuff.isRightHandedBuff = true;
            player.playerEffectsManager.rightWeaponBuffEffect = weaponBuff;
            player.playerEffectsManager.ProcessWeaponBuffs();
        }

        public override bool CanIUseThisItem(PlayerManager player)
        {
            if (player.playerInventoryManager.currentConsumable.currentItemAmount <= 0)
                return false;

            MeleeWeaponItem meleeWeapon = player.playerInventoryManager.rightWeapon as MeleeWeaponItem;

            if(meleeWeapon != null && meleeWeapon.canBeBuffed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
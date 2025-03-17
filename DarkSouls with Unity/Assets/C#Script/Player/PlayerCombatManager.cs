using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        //private void PerformRBRangedAction()
        //{
        //    if (playerStatsManager.currentStamina <= 0)
        //        return;

        //    //playerManager.isInteracting = animator.GetBool("isInteracting");
        //    playerAnimatorManager.EraseHandIKForWeapon();
        //    playerAnimatorManager.animator.SetBool("isUsingRightHand", true);

        //    if(!playerManager.isHoldingArrow)
        //    {
        //        //如果有弓箭，Draw the Arrow装备弓箭，当松开RB时射出
        //        //如果没有弓箭，播放无弹药动画
        //        if(playerInventoryManager.currentAmmo != null)
        //        {
        //            DrawArrowAction();
        //        }
        //        else
        //        {
        //            playerAnimatorManager.PlayTargetAnimation("Shrug", true);
        //        }
        //    }
        //}

        public override void DrainStaminaBasedOnAttack()
        {
            if (player.isUsingRightHand || player.isUsingLeftHand)
            {
                WeaponItem weapon = player.isUsingRightHand ? player.playerInventoryManager.rightWeapon : player.playerInventoryManager.leftWeapon;
                switch (player.characterCombatManager.currentAttackType)
                {
                    case AttackType.lightAttack:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.lightAttackStaminaMultiplier);
                        break;
                    case AttackType.lightAttackCombo:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.lightAttackComboStaminaMultiplier);
                        break;
                    case AttackType.runningAttack:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.runningAttackStaminaMultiplier);
                        break;
                    case AttackType.heavyAttack:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.heavyAttackStaminaMultiplier);
                        break;
                    case AttackType.heavyAttackCombo:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.heavyAttackComboStaminaMultiplier);
                        break;
                    case AttackType.jumpingAttack:
                        player.playerStatsManager.DeductStamina(weapon.baseStaminaCost * weapon.jumpingAttackStaminaMultiplier);
                        break;
                }
            }
            #region 替代
            //if (player.isUsingRightHand)
            //{
            //    if (player.characterCombatManager.currentAttackType == AttackType.lightAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.lightAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.lightAttackCombo)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.lightAttackComboStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.runningAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.runningAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.heavyAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.heavyAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.heavyAttackCombo)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.heavyAttackComboStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.jumpingAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.jumpingAttackStaminaMultiplier);
            //    }
            //}
            //else if (player.isUsingLeftHand)
            //{
            //    if (player.characterCombatManager.currentAttackType == AttackType.lightAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.lightAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.lightAttackCombo)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.lightAttackComboStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.runningAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.runningAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.heavyAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.heavyAttackStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.heavyAttackCombo)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.heavyAttackComboStaminaMultiplier);
            //    }
            //    else if (player.characterCombatManager.currentAttackType == AttackType.jumpingAttack)
            //    {
            //        player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStaminaCost * player.playerInventoryManager.rightWeapon.jumpingAttackStaminaMultiplier);
            //    }
            //}
            #endregion
        }
    }
}

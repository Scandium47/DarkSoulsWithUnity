using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/Humanoid Actions/Item Based Attack Action")]
    public class ItemBasedAttackAction : ScriptableObject
    {
        [Header("Attack Type")]
        public AIAttackActionType actionAttackType = AIAttackActionType.meleeAttackAction;
        public AttackType attackType = AttackType.lightAttack;

        [Header("Action Combo Settings")]
        public bool actionCanCombo = false;

        [Header("Right Hand Or Left Hand Action")]
        bool isRightHandedAction = true;

        [Header("Action Settings")]
        public int attackScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 3;

        public void PerformAttackAction(EnemyManager enemy)
        {
            if(isRightHandedAction)
            {
                enemy.UpdateWhitchHandCharacterIsUsing(true);
                PerformRightHandItemActionBasedOnAttackType(enemy);
            }
            else
            {
                enemy.UpdateWhitchHandCharacterIsUsing(false);
                PerformLeftHandItemActionBasedOnAttackType(enemy);
            }
        }

        //决定哪个手施展动作
        private void PerformRightHandItemActionBasedOnAttackType(EnemyManager enemy)
        {
            if(actionAttackType == AIAttackActionType.meleeAttackAction)
            {
                PerformRightHandMeleeAction(enemy);
            }
            else if(actionAttackType != AIAttackActionType.rangedAttackAction)
            {

            }
        }

        private void PerformLeftHandItemActionBasedOnAttackType(EnemyManager enemy)
        {
            if (actionAttackType == AIAttackActionType.meleeAttackAction)
            {

            }
            else if (actionAttackType != AIAttackActionType.rangedAttackAction)
            {

            }
        }

        private void PerformRightHandMeleeAction(EnemyManager enemy)
        {
            if(enemy.isTwoHandingWeapon)
            {
                if(attackType == AttackType.lightAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.th_tap_RB_Action.PerformAction(enemy);
                }
                else if(attackType == AttackType.heavyAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.th_tap_RT_Action.PerformAction(enemy);
                }
            }
            else
            {
                if (attackType == AttackType.lightAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.oh_tap_RB_Action.PerformAction(enemy);
                }
                else if (attackType == AttackType.heavyAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.oh_tap_RT_Action.PerformAction(enemy);
                }
            }
        }

    }
}
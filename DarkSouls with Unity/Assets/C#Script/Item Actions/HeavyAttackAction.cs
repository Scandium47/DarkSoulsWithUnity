using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Heavy Attack Action")]
    public class HeavyAttackAction : ItemAction
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.characterStatsManager.currentStamina <= 0)
                return;

            character.isAttacking = true;
            character.characterAnimatorManager.EraseHandIKForWeapon();
            character.characterEffectsManager.PlayWeaponFX(false);

            if (character.isSprinting)
            {
                //冲刺攻击
                HandleJumpingAttack(character);
                character.characterCombatManager.currentAttackType = AttackType.jumpingAttack;
                return;
            }

            if (character.canDoCombo)
            {
                HandleHeavyWeaponCombo(character);
                character.characterCombatManager.currentAttackType = AttackType.heavyAttackCombo;
            }
            else
            {
                if (character.isInteracting)
                    return;
                if (character.canDoCombo)
                    return;

                HandleHeavyAttack(character);
                character.characterCombatManager.currentAttackType = AttackType.heavyAttack;
            }
        }

        private void HandleHeavyAttack(CharacterManager character)
        {
            if (character.isUsingLeftHand)
            {
                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
            }
            else if(character.isUsingRightHand)
            {
                if(character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_01;
                }
                else
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                }
            }
        }

        private void HandleJumpingAttack(CharacterManager character)
        {
            if (character.isUsingLeftHand)
            {
                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_jumping_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_jumping_attack_01;
            }
            else if (character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_jumping_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_jumping_attack_01;
                }
                else
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_jumping_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_jumping_attack_01;
                }
            }
        }

        private void HandleHeavyWeaponCombo(CharacterManager character)
        {
            if (character.canDoCombo)
            {
                character.animator.SetBool("canDoCombo", true);

                if (character.isUsingLeftHand)
                {
                    //重击二连循环
                    if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_01)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_02, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_02;
                    }
                    else if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_02)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                    }
                    //如果是轻击连重击也可以，接重击一
                    else
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                    }
                }
                else if (character.isUsingRightHand)
                {
                    if (character.isTwoHandingWeapon)
                    {
                        //双持重击二连循环
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_heavy_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_02;
                        }
                        else if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_heavy_attack_02)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_01;
                        }
                        //如果是轻击接重击也可以，接重击一
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_01;
                        }
                    }
                    else
                    {
                        //重击二连循环
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_02;
                        }
                        else if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_02)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                        }
                        //如果是轻击连重击也可以，接重击一
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true, false, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                        }
                    }
                }

                character.animator.SetBool("canDoCombo", false);
            }
        }
    }
}
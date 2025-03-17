using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Light Attack Action")]
    public class LightAttackAction : ItemAction
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
                //��̹���
                HandleRunningAttack(character);
                character.characterCombatManager.currentAttackType = AttackType.runningAttack;
                return;
            }

            if (character.canDoCombo)
            {
                HandleLightWeaponCombo(character);
                character.characterCombatManager.currentAttackType = AttackType.lightAttackCombo;
            }
            else
            {
                if (character.isInteracting)
                    return;
                if (character.canDoCombo)
                    return;

                HandleLightAttack(character);
                character.characterCombatManager.currentAttackType = AttackType.lightAttack;
            }
        }

        private void HandleLightAttack(CharacterManager character)
        {
            if(character.isUsingLeftHand)
            {
                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
            }
            else if(character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_light_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_light_attack_01;
                }
                else
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
                }
            }
        }

        private void HandleRunningAttack(CharacterManager character)
        {
            if(character.isUsingLeftHand)
            {
                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_running_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_running_attack_01;
            }
            else if(character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_running_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_running_attack_01;
                }
                else
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_running_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_running_attack_01;
                }
            }
        }

        private void HandleLightWeaponCombo(CharacterManager character)
        {
            if (character.canDoCombo)
            {
                character.animator.SetBool("canDoCombo", true);
                if(character.isUsingLeftHand)
                {
                    //�����������ѭ��
                    if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_light_attack_01)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_02, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_02;
                    }
                    else if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_light_attack_02)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
                    }
                    //������ػ������Ҳ���ԣ������һ
                    else
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
                    }
                }
                else if(character.isUsingRightHand)
                {
                    if (character.isTwoHandingWeapon)
                    {
                        //˫���������ѭ��
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_light_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_light_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_light_attack_02;
                        }
                        else if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_light_attack_02)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_light_attack_03, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_light_attack_03;
                        }
                        else if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_light_attack_03)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_light_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_light_attack_01;
                        }
                        //������ػ������Ҳ���ԣ������һ
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_light_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_light_attack_01;
                        }
                    }
                    else
                    {
                        //�����������ѭ��
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_light_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_02;
                        }
                        else if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_light_attack_02)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
                        }
                        //������ػ������Ҳ���ԣ������һ
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_light_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_light_attack_01;
                        }
                    }
                }
                character.animator.SetBool("canDoCombo", false);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Take Blocked Damage")]
    public class TakeBlockedDamageEffect : CharacterEffect
    {
        //将造成伤害的角色加到列表
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;

        [Header("BaseDamage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;
        public float staminaDamage = 0;
        public float poiseDamage = 0;

        [Header("Aniamtion")]
        public string blockAnimation;

        public override void ProecssEffect(CharacterManager character)
        {
            //如果角色死亡或无敌，返回不执行任何逻辑
            if (character.isDead || character.isInvulnerable)
                return;

            CalculateDamage(character);
            CalculateStaminaDamage(character);
            DecideBlockAnimationBasedOnPoiseDamage(character);
            PlayBlockSoundFX(character);
            AssignNewAITarget(character);

            if(character.isDead)
            {
                character.characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
            }
            else
            {
                if(character.characterStatsManager.currentStamina <= 0)
                {
                    character.characterAnimatorManager.PlayTargetAnimation("Guard_Break_01", true);
                    character.canBeParried = true;
                    //播放破盾音效 character.characterSoundFXManager.PlayGuardBreakSound
                    character.isBlocking = false;
                }
                else
                {
                    character.characterAnimatorManager.PlayTargetAnimation(blockAnimation, true);
                    character.isAttacking = false;
                }
            }
        }

        private void CalculateDamage(CharacterManager character)
        {
            //如果角色死亡或无敌，返回不执行任何逻辑
            if (character.isDead)
                return;

            if(characterCausingDamage != null)
            {
                physicalDamage = Mathf.RoundToInt(physicalDamage * (characterCausingDamage.characterStatsManager.physicalDamagePercentageModifier / 100));
                fireDamage = Mathf.RoundToInt(fireDamage * (characterCausingDamage.characterStatsManager.fireDamagePercentageModifier / 100));
            }

            character.characterAnimatorManager.EraseHandIKForWeapon();

            //物理伤害
            float totalPhysicalDamageAbsorption = 1 -
                (1 - character.characterStatsManager.physicalDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionHands / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionLegs / 100);
            physicalDamage = Mathf.RoundToInt(physicalDamage - (physicalDamage * totalPhysicalDamageAbsorption));

            //火焰伤害
            float totalFireDamageAbsorption = 1 -
                (1 - character.characterStatsManager.fireDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionHands / 100) *
                (1 - character.characterStatsManager.fireDamageAbsorptionLegs / 100);
            fireDamage = Mathf.RoundToInt(fireDamage - (fireDamage * totalFireDamageAbsorption));

            //Debug.Log("Total Damage Absorption is " + totalPhysicalDamageAbsorption + "%");
            physicalDamage = physicalDamage - Mathf.RoundToInt(physicalDamage * (character.characterStatsManager.physicalAbsorptionPercentageModifier / 100));
            fireDamage = fireDamage - Mathf.RoundToInt(fireDamage * (character.characterStatsManager.fireAbsorptionPercentageModifier / 100));
            float finalDamage = physicalDamage + fireDamage; //+ extra damage

            //if (characterCausingDamage != null && characterCausingDamage.isPerformingFullyChargedAttack)
            //{
            //    finalDamage = finalDamage * 2;
            //}

            Debug.Log("最终伤害：" + finalDamage);
            character.characterStatsManager.currentHealth = Mathf.RoundToInt(character.characterStatsManager.currentHealth - finalDamage);
            //Debug.Log("Total Damage Dealt is " + finalDamage);

            if (character.characterStatsManager.currentHealth <= 0)
            {
                character.characterStatsManager.currentHealth = 0;
                character.isDead = true;
            }

            //盾牌被击打声
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
            float staminaDamageAbsorption = staminaDamage * (character.characterStatsManager.blockingStabilityRating / 100);
            float staminaDamageAfterAbsorption = staminaDamage - staminaDamageAbsorption;
            character.characterStatsManager.currentStamina -= staminaDamageAfterAbsorption;
        }

        private void DecideBlockAnimationBasedOnPoiseDamage(CharacterManager character)
        {
            if(!character.isTwoHandingWeapon)
            {
                if (poiseDamage <= 24 && poiseDamage >= 0)
                {
                    blockAnimation = "OH_Block_Guard_Ping_01";
                    return;
                }
                else if (poiseDamage <= 49 && poiseDamage >= 25)
                {
                    blockAnimation = "OH_Block_Guard_Light_01";
                    return;
                }
                else if (poiseDamage <= 74 && poiseDamage >= 50)
                {
                    blockAnimation = "OH_Block_Guard_Medium_01";
                    return;
                }
                else if (poiseDamage >= 75)
                {
                    blockAnimation = "OH_Block_Guard_Heavy_01";
                    return;
                }
            }
            else
            {
                if (poiseDamage <= 24 && poiseDamage >= 0)
                {
                    blockAnimation = "TH_Block_Guard_Ping_01";
                    return;
                }
                else if (poiseDamage <= 49 && poiseDamage >= 25)
                {
                    blockAnimation = "TH_Block_Guard_Light_01";
                    return;
                }
                else if (poiseDamage <= 74 && poiseDamage >= 50)
                {
                    blockAnimation = "TH_Block_Guard_Medium_01";
                    return;
                }
                else if (poiseDamage >= 75)
                {
                    blockAnimation = "TH_Block_Guard_Heavy_01";
                    return;
                }
            }
        }

        private void PlayBlockSoundFX(CharacterManager character)
        {
            if(character.isTwoHandingWeapon)
            {
                character.characterSoundFXManager.PlayRandomSoundFXFromArray(character.characterInventoryManager.rightWeapon.blockingNoises);
            }
            else
            {
                character.characterSoundFXManager.PlayRandomSoundFXFromArray(character.characterInventoryManager.leftWeapon.blockingNoises);
            }
        }

        private void AssignNewAITarget(CharacterManager character)
        {
            EnemyManager aiCharacter = character as EnemyManager;

            if (aiCharacter != null && characterCausingDamage != null)
            {
                aiCharacter.currentTarget = characterCausingDamage;
            }
        }
    }
}
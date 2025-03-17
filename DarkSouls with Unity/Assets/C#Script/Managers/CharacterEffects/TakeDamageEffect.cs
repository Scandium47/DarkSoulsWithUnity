using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Take Damage")]
    public class TakeDamageEffect : CharacterEffect
    {
        //������˺��Ľ�ɫ�ӵ��б�
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage;

        [Header("Damage")]
        public float physicalDamage = 0;
        public float fireDamage = 0;

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false;

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("SFX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundSFX;           //  ��ɻ����˺�ʱ���ŵĶ���Ԫ����Ч�����棬ħ�����ڰ���������

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;
        public Vector3 contactPoint;        //��ɫ���ĸ������ܵ����˺�

        public override void ProecssEffect(CharacterManager character)
        {
            //�����ɫ�������޵У����ز�ִ���κ��߼�
            if (character.isDead || character.isInvulnerable)
                return;

            //�������˺�
            CalculateDamage(character);
            //����˺����򣬲��Ŷ�Ӧ�������ܻ���Ч��ѪҺ�ɽ���Ч
            CheckWhichDirectionDamageCameFrom(character);
            PlayDamageAnimation(character);
            PlayDamageSoundFX(character);
            PlayBloodSplatter(character);
            //����˺��󣬽��ܻ���ɫ��Ŀ��ת��Ϊ�����
            AssignNewAITarget(character);
        }

        private void CalculateDamage(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;
            EnemyManager enemy = character as EnemyManager;

            if (characterCausingDamage != null)
            {
                physicalDamage = Mathf.RoundToInt(physicalDamage * (characterCausingDamage.characterStatsManager.physicalDamagePercentageModifier / 100));
                fireDamage = Mathf.RoundToInt(fireDamage * (characterCausingDamage.characterStatsManager.fireDamagePercentageModifier / 100));
            }

            character.characterAnimatorManager.EraseHandIKForWeapon();

            //�����˺�
            float totalPhysicalDamageAbsorption = 1 -
                (1 - character.characterStatsManager.physicalDamageAbsorptionHead / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionBody / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionHands / 100) *
                (1 - character.characterStatsManager.physicalDamageAbsorptionLegs / 100);
            physicalDamage = Mathf.RoundToInt(physicalDamage - (physicalDamage * totalPhysicalDamageAbsorption));

            //�����˺�
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

            if (characterCausingDamage != null && characterCausingDamage.isPerformingFullyChargedAttack)
            {
                finalDamage = finalDamage * 2;
            }

            Debug.Log("�����˺���" + finalDamage);
            character.characterStatsManager.currentHealth = Mathf.RoundToInt(character.characterStatsManager.currentHealth - finalDamage);
            //Debug.Log("Total Damage Dealt is " + finalDamage);
            if (player != null)
            {
                player.playerStatsManager.healthBar.SetCurrentHealth(character.characterStatsManager.currentHealth);
                Debug.Log("���Ѫ��ui��" + character.characterStatsManager.currentHealth);
            }
            if (enemy != null)
            {
                if(enemy.enemyStatsManager.isBoss && enemy.enemyBossManager != null)
                {
                    enemy.enemyBossManager.UpdateBossHealthBar(character.characterStatsManager.currentHealth, enemy.enemyStatsManager.maxHealth);
                    Debug.Log("BossѪ��ui��" + character.characterStatsManager.currentHealth);
                }
                else
                {
                    enemy.enemyStatsManager.enemyHealthBar.SetHealth(character.characterStatsManager.currentHealth);
                    Debug.Log("����Ѫ��ui��" + character.characterStatsManager.currentHealth);
                }
            }

            if (character.characterStatsManager.totalPoiseDefence < poiseDamage)
            {
                poiseIsBroken = true;
            }

            if (character.characterStatsManager.currentHealth <= 0)
            {
                character.characterStatsManager.currentHealth = 0;
                character.isDead = true;
            }
        }

        private void CheckWhichDirectionDamageCameFrom(CharacterManager character)
        {
            //Debug.Log(direction);
            if (manuallySelectDamageAnimation)
                return;

            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                ChooseDamageAnimationForward(character);
            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                ChooseDamageAnimationForward(character);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                ChooseDamageAnimationBack(character);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                ChooseDamageAnimationLeft(character);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                ChooseDamageAnimationRight(character);
            }
        }

        private void ChooseDamageAnimationForward(CharacterManager character)
        {
            if(poiseDamage <= 24 && poiseDamage>=0)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Forward);
                return;
            }
            else if(poiseDamage <= 49 && poiseDamage >=25)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Forward);
                return;
            }
            else if(poiseDamage <=74 && poiseDamage >= 50)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Heavy_Forward);
                return;
            }
            else if(poiseDamage >= 75)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Colossal_Forward);
                return;
            }
        }

        private void ChooseDamageAnimationBack(CharacterManager character)
        {
            if (poiseDamage <= 24 && poiseDamage >= 0)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Backward);
                return;
            }
            else if (poiseDamage <= 49 && poiseDamage >= 25)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Backward);
                return;
            }
            else if (poiseDamage <= 74 && poiseDamage >= 50)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Heavy_Backward);
                return;
            }
            else if (poiseDamage >= 75)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Colossal_Backward);
                return;
            }
        }

        private void ChooseDamageAnimationLeft(CharacterManager character)
        {
            if (poiseDamage <= 24 && poiseDamage >= 0)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Left);
                return;
            }
            else if (poiseDamage <= 49 && poiseDamage >= 25)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Left);
                return;
            }
            else if (poiseDamage <= 74 && poiseDamage >= 50)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Heavy_Left);
                return;
            }
            else if (poiseDamage >= 75)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Colossal_Left);
                return;
            }
        }

        private void ChooseDamageAnimationRight(CharacterManager character)
        {
            if (poiseDamage <= 24 && poiseDamage >= 0)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Right);
                return;
            }
            else if (poiseDamage <= 49 && poiseDamage >= 25)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Medium_Right);
                return;
            }
            else if (poiseDamage <= 74 && poiseDamage >= 50)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Heavy_Right);
                return;
            }
            else if (poiseDamage >= 75)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomDamageAnimationFromList(character.characterAnimatorManager.Damage_Animations_Colossal_Right);
                return;
            }
        }

        private void PlayDamageSoundFX(CharacterManager character)
        {
            character.characterSoundFXManager.PlayRandomDamageSoundFX();

            if(fireDamage > 0)
            {
                character.characterSoundFXManager.PlaySoundFX(elementalDamageSoundSFX);
            }
        }

        private void PlayDamageAnimation(CharacterManager character)
        {
            PlayerManager player =  character as PlayerManager;

            //������������������󲥷��ػ��˺�������ͬʱذ�׹����������ٲ�����΢�˺�����
            if(character.isInteracting && character.characterCombatManager.previousPoiseDamageTaken > poiseDamage)
            {
                return;
            }

            if(character.isDead)
            {
                character.characterWeaponSlotManager.CloseDamageCollider();
                character.characterAnimatorManager.PlayTargetAnimation("Dead_01", true);
                if (player != null)
                {
                    player.isInteracting = player.animator.GetBool("isInteracting");
                    //���˫��IK��Ȩ�ؽ�Ϊ��0��
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                }
                return;
            }

            if(!poiseIsBroken)
            {
                return;
            }
            else
            {
                if(playDamageAnimation)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(damageAnimation, true);
                    if (player != null)
                    {
                        player.isInteracting = player.animator.GetBool("isInteracting");
                        //���˫��IK��Ȩ�ؽ�Ϊ��0��
                        player.playerAnimatorManager.EraseHandIKForWeapon();
                    }
                }
            }
        }

        private void PlayBloodSplatter(CharacterManager character)
        {
            character.characterEffectsManager.PlayBloodSplatterFX(contactPoint);
        }

        private void AssignNewAITarget(CharacterManager character)
        {
            EnemyManager aiCharacter = character as EnemyManager;

            if(aiCharacter != null && characterCausingDamage != null)
            {
                aiCharacter.currentTarget = characterCausingDamage;
            }
        }
    }
}
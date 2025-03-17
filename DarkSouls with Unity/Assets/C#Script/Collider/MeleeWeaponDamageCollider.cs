using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Weapon Buff Damage")]
        public float physicalBuffDamage;
        public float fireBuffDamage;
        public float poiseBuffDamage;

        protected override void DealDamage(CharacterManager enemyManager)
        {
            //�����ڹ����Ľ�ɫ��ȡ�������ͣ�Ӧ�ù������͵ı��ʣ����˺����ݸ���������

            float finalPhysicalDamage = physicalDamage + physicalBuffDamage;
            float finalFireDamage = fireDamage + fireBuffDamage;
            float finalDamage = 0;

            // ��ȡ��ǰʹ�õ�����
            var currentHandWeapon = characterManager.isUsingRightHand ? characterManager.characterInventoryManager.rightWeapon
                : characterManager.isUsingLeftHand ? characterManager.characterInventoryManager.leftWeapon
                : null;

            // ����е�ǰʹ�õ�����
            if (currentHandWeapon != null)
            {
                switch (characterManager.characterCombatManager.currentAttackType)
                {
                    case AttackType.lightAttack:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.lightAttackDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.lightAttackDamageModifier;
                        break;
                    case AttackType.lightAttackCombo:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.lightAttackComboDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.lightAttackComboDamageModifier;
                        break;
                    case AttackType.runningAttack:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.runningAttackDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.runningAttackDamageModifier;
                        break;
                    case AttackType.heavyAttack:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.heavyAttackDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.heavyAttackDamageModifier;
                        break;
                    case AttackType.heavyAttackCombo:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.heavyAttackComboDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.heavyAttackComboDamageModifier;
                        break;
                    case AttackType.jumpingAttack:
                        finalDamage = finalPhysicalDamage * currentHandWeapon.jumpingAttackDamageModifier;
                        finalDamage += finalFireDamage * finalPhysicalDamage * currentHandWeapon.jumpingAttackDamageModifier;
                        break;
                }
            }

            TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            takeDamageEffect.physicalDamage = finalPhysicalDamage;
            takeDamageEffect.fireDamage = finalFireDamage;
            takeDamageEffect.poiseDamage = poiseDamage;
            takeDamageEffect.contactPoint = contactPoint;
            takeDamageEffect.angleHitFrom = angleHitFrom;
            enemyManager.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
        }
    }
}
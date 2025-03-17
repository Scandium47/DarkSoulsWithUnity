using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        public CharacterManager characterManager;
        protected Collider damageCollider;
        public bool enabledDamageColliderOnStartUp = false;

        [Header("Team I.D")]
        public int teamIDNumber = 0;

        [Header("Poise")]
        public float poiseDamage;
        public float offensivePoiseBonus;

        [Header("Damage")]
        public int physicalDamage;
        public int fireDamage;
        public int magicDamage;
        public int lightningDamage;
        public int darkDamage;

        [Header("Guard Break Modifier")]
        public float guardBreakModifier = 1;

        protected bool hasBeenParried;
        protected bool shieldHasBeenHit;
        protected string currentDamageAnimation;

        protected Vector3 contactPoint;
        protected float angleHitFrom;

        //����һ�����б����ڴ洢�˴ι����Ӵ�������ײ����ֱ��ÿ�ιر��˺���ײ��ʱ��ո��б�
        private List<CharacterManager> charactersDamagedDuringThisCalculation = new List<CharacterManager>();

        protected virtual void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledDamageColliderOnStartUp;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisaleDamageCollider()
        {
            if(charactersDamagedDuringThisCalculation.Count > 0)
            {
                charactersDamagedDuringThisCalculation.Clear();
            }

            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            //ע�⣺����enemyManagerΪ��������ײ���������Ķ����ͳ�ƣ�characterManager��������ײ�����������ߣ�������
            if (collision.gameObject.layer == LayerMask.NameToLayer("Damageable Character"))
            {
                hasBeenParried = false;
                shieldHasBeenHit = false;
                CharacterManager enemyManager = collision.GetComponentInParent<CharacterManager>();

                if (enemyManager != null && !enemyManager.isDead)
                {
                    EnemyManager aiCharacter = enemyManager as EnemyManager;

                    //����˺��б��ڰ������Ѿ����ڵ���ײ��������
                    if (charactersDamagedDuringThisCalculation.Contains(enemyManager))
                        return;
                    //����˺��б���û�д���ײ���������б�
                    charactersDamagedDuringThisCalculation.Add(enemyManager);

                    if (enemyManager.characterStatsManager.teamIDNumber == teamIDNumber)
                        return;

                    CheckForParry(enemyManager);
                    CheckForBlock(enemyManager);

                    if (enemyManager.characterStatsManager.teamIDNumber == teamIDNumber)
                        return;

                    if (hasBeenParried)
                        return;

                    if (shieldHasBeenHit)
                        return;

                    enemyManager.characterStatsManager.poiseResetTimer = enemyManager.characterStatsManager.totalPoiseResetTime;
                    enemyManager.characterStatsManager.totalPoiseDefence = enemyManager.characterStatsManager.totalPoiseDefence - poiseDamage;
                    Debug.Log("������� " + enemyManager.characterStatsManager.totalPoiseDefence);

                    //����λ�ã����������һ�νӴ���ײ����λ�ã��Կռ�����Ϊ���ߣ������ǰ �� �ܻ���ǰ �ļн�
                    contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    angleHitFrom = (Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up));
                    DealDamage(enemyManager);

                    if(aiCharacter != null)
                    {
                        //�������AI�����ҹ����˵��ˣ����˵�Ŀ��תΪ����AI
                        Debug.Log(characterManager.gameObject.name);
                        aiCharacter.currentTarget = characterManager;
                    }
                }
            }

            #region ɾ���ĵ���damageCollider��⣬��EP78�� ��������ҹ���һ����ײ�����
            //if(collision.tag == "Enemy")
            //{
            //    EnemyStatsManager enemyStats = collision.GetComponent<EnemyStatsManager>();
            //    CharacterManager enemycharacterManager = collision.GetComponent<CharacterManager>();
            //    CharacterEffectsManager enemyEffectsManager = collision.GetComponent<CharacterEffectsManager>();
            //    BlockingCollider shield = collision.transform.GetComponentInChildren<BlockingCollider>();

            //    if (enemycharacterManager != null)
            //    {
            //        if (enemycharacterManager.isParrying)
            //        {
            //            //check here if you are parryable
            //            characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
            //            return;
            //        }
            //        else if (shield != null && enemycharacterManager.isBlocking)
            //        {
            //            //�ٷֱ������˺�
            //            float physicalDamageAfterBlock =
            //                physicalDamage - (physicalDamage * shield.blockingPhysicalDamageAbsorption) / 100;

            //            if (enemyStats != null)
            //            {
            //                enemyStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Block Guard");
            //                return;
            //            }
            //        }
            //    }

            //    if (enemyStats != null)
            //    {
            //        enemyStats.poiseResetTimer = enemyStats.totalPoiseResetTime;
            //        enemyStats.totalPoiseDefence = enemyStats.totalPoiseDefence - poiseBreak;
            //        Debug.Log("�������� " + enemyStats.totalPoiseDefence);

            //        //����λ�ã����������һ�νӴ���ײ����λ��
            //        Vector3 contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //        enemyEffectsManager.PlayBloodSplatterFX(contactPoint);

            //        if (enemyStats.isBoss)
            //        {
            //            if (enemyStats.totalPoiseDefence > poiseBreak)
            //            {
            //                enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
            //            }
            //            else
            //            {
            //                enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
            //                enemyStats.BreakGoard();
            //            }
            //        }
            //        else
            //        {
            //            if (enemyStats.totalPoiseDefence > poiseBreak)
            //            {
            //                enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
            //            }
            //            else
            //            {
            //                enemyStats.TakeDamage(currentWeaponDamage);
            //            }
            //        }
            //    }
            //}
            #endregion

            if (collision.gameObject.tag == "Illusionary Wall")
            {
                IllusionaryWall illusionaryWall = collision.GetComponent<IllusionaryWall>();

                illusionaryWall.wallHasBeenHit = true;
            }
        }

        protected virtual void CheckForParry(CharacterManager enemyManager)
        {
            if (enemyManager.isParrying)
            {
                //����Ƿ񱻵���
                characterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        protected virtual void CheckForBlock(CharacterManager enemyManager)
        {
            Vector3 directionFromPlayerToEnemy = (characterManager.transform.position - enemyManager.transform.position);
            float dotValueFromPlayerToEnemy = Vector3.Dot(directionFromPlayerToEnemy, enemyManager.transform.forward);

            if (enemyManager.isBlocking && dotValueFromPlayerToEnemy > 0.3f)
            {            
                shieldHasBeenHit = true;

                TakeBlockedDamageEffect takeBlockedDamage = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);
                takeBlockedDamage.physicalDamage = physicalDamage;
                takeBlockedDamage.fireDamage = fireDamage;
                takeBlockedDamage.poiseDamage = poiseDamage;
                takeBlockedDamage.staminaDamage = poiseDamage;

                enemyManager.characterEffectsManager.ProcessEffectInstantly(takeBlockedDamage);
            }
        }

        protected virtual void DealDamage(CharacterManager enemyManager)
        {
            //�����ڹ����Ľ�ɫ��ȡ�������ͣ�Ӧ�ù������͵ı��ʣ����˺����ݸ���������

            #region �滻�����
            //float finalPhysicalDamage = physicalDamage;

            //if(characterManager.isUsingRightHand)
            //{
            //    if(characterManager.characterCombatManager.currentAttackType == AttackType.lightAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.lightAttackDamageModifier;
            //    }
            //    else if(characterManager.characterCombatManager.currentAttackType == AttackType.lightAttackCombo)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.lightAttackComboDamageModifier;
            //    }
            //    else if(characterManager.characterCombatManager.currentAttackType == AttackType.runningAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.runningAttackDamageModifier;
            //    }
            //    else if(characterManager.characterCombatManager.currentAttackType == AttackType.heavyAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.heavyAttackDamageModifier;
            //    }
            //    else if(characterManager.characterCombatManager.currentAttackType == AttackType.heavyAttackCombo)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.heavyAttackComboDamageModifier;
            //    }
            //    else if(characterManager.characterCombatManager.currentAttackType == AttackType.jumpingAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.rightWeapon.jumpingAttackDamageModifier;
            //    }
            //}
            //else if(characterManager.isUsingLeftHand)
            //{
            //    if (characterManager.characterCombatManager.currentAttackType == AttackType.lightAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.lightAttackDamageModifier;
            //    }
            //    else if (characterManager.characterCombatManager.currentAttackType == AttackType.lightAttackCombo)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.lightAttackComboDamageModifier;
            //    }
            //    else if (characterManager.characterCombatManager.currentAttackType == AttackType.runningAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.runningAttackDamageModifier;
            //    }
            //    else if (characterManager.characterCombatManager.currentAttackType == AttackType.heavyAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.heavyAttackDamageModifier;
            //    }
            //    else if (characterManager.characterCombatManager.currentAttackType == AttackType.heavyAttackCombo)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.heavyAttackComboDamageModifier;
            //    }
            //    else if (characterManager.characterCombatManager.currentAttackType == AttackType.jumpingAttack)
            //    {
            //        finalPhysicalDamage = finalPhysicalDamage * characterManager.characterInventoryManager.leftWeapon.jumpingAttackDamageModifier;
            //    }
            //}
            #endregion

            float finalPhysicalDamage = physicalDamage;
            float finalFireDamage = fireDamage;

            // ��ȡ��ǰʹ�õ�����
            var currentHandWeapon = characterManager.isUsingRightHand
                ? characterManager.characterInventoryManager.rightWeapon
                : characterManager.isUsingLeftHand
                    ? characterManager.characterInventoryManager.leftWeapon
                    : null;

            // ����е�ǰʹ�õ�����
            if (currentHandWeapon != null)
            {
                switch (characterManager.characterCombatManager.currentAttackType)
                {
                    case AttackType.lightAttack:
                        finalPhysicalDamage *= currentHandWeapon.lightAttackDamageModifier;
                        finalFireDamage *= currentHandWeapon.lightAttackDamageModifier;
                        break;
                    case AttackType.lightAttackCombo:
                        finalPhysicalDamage *= currentHandWeapon.lightAttackComboDamageModifier;
                        finalFireDamage *= currentHandWeapon.lightAttackComboDamageModifier;
                        break;
                    case AttackType.runningAttack:
                        finalPhysicalDamage *= currentHandWeapon.runningAttackDamageModifier;
                        finalFireDamage *= currentHandWeapon.runningAttackDamageModifier;
                        break;
                    case AttackType.heavyAttack:
                        finalPhysicalDamage *= currentHandWeapon.heavyAttackDamageModifier;
                        finalFireDamage *= currentHandWeapon.heavyAttackDamageModifier;
                        break;
                    case AttackType.heavyAttackCombo:
                        finalPhysicalDamage *= currentHandWeapon.heavyAttackComboDamageModifier;
                        finalFireDamage *= currentHandWeapon.heavyAttackComboDamageModifier;
                        break;
                    case AttackType.jumpingAttack:
                        finalPhysicalDamage *= currentHandWeapon.jumpingAttackDamageModifier;
                        finalFireDamage *= currentHandWeapon.jumpingAttackDamageModifier;
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

            //if (enemyStats.totalPoiseDefence > poiseDamage)
            //{
            //    Debug.Log("�޶����˺�");
            //    enemyStats.TakeDamageNoAnimation(Mathf.RoundToInt(finalPhysicalDamage), fireDamage);
            //}
            //else
            //{
            //    Debug.Log("�ж����˺�");
            //    enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), 0, currentDamageAnimation, characterManager);
            //}
        }
    }
}

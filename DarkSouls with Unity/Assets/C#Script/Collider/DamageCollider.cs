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

        //创建一个新列表，用于存储此次攻击接触到的碰撞器，直到每次关闭伤害碰撞器时清空该列表
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
            //注意：以下enemyManager为被武器碰撞器攻击到的对象的统称，characterManager是武器碰撞的武器持有者（父级）
            if (collision.gameObject.layer == LayerMask.NameToLayer("Damageable Character"))
            {
                hasBeenParried = false;
                shieldHasBeenHit = false;
                CharacterManager enemyManager = collision.GetComponentInParent<CharacterManager>();

                if (enemyManager != null && !enemyManager.isDead)
                {
                    EnemyManager aiCharacter = enemyManager as EnemyManager;

                    //如果伤害列表内包含了已经存在的碰撞器，返回
                    if (charactersDamagedDuringThisCalculation.Contains(enemyManager))
                        return;
                    //如果伤害列表内没有此碰撞器，加入列表
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
                    Debug.Log("玩家韧性 " + enemyManager.characterStatsManager.totalPoiseDefence);

                    //击中位置，检测武器第一次接触碰撞器的位置，以空间向上为法线，打击者前 与 受击者前 的夹角
                    contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    angleHitFrom = (Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up));
                    DealDamage(enemyManager);

                    if(aiCharacter != null)
                    {
                        //如果白灵AI存在且攻击了敌人，敌人的目标转为白灵AI
                        Debug.Log(characterManager.gameObject.name);
                        aiCharacter.currentTarget = characterManager;
                    }
                }
            }

            #region 删掉的敌人damageCollider检测，在EP78后 敌人与玩家共用一个碰撞器检测
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
            //            //百分比吸收伤害
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
            //        Debug.Log("敌人韧性 " + enemyStats.totalPoiseDefence);

            //        //击中位置，检测武器第一次接触碰撞器的位置
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
                //检测是否被弹反
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
            //从正在攻击的角色获取攻击类型，应用攻击类型的倍率，把伤害传递给攻击对象

            #region 替换代码↓
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

            // 获取当前使用的武器
            var currentHandWeapon = characterManager.isUsingRightHand
                ? characterManager.characterInventoryManager.rightWeapon
                : characterManager.isUsingLeftHand
                    ? characterManager.characterInventoryManager.leftWeapon
                    : null;

            // 如果有当前使用的武器
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
            //    Debug.Log("无动画伤害");
            //    enemyStats.TakeDamageNoAnimation(Mathf.RoundToInt(finalPhysicalDamage), fireDamage);
            //}
            //else
            //{
            //    Debug.Log("有动画伤害");
            //    enemyStats.TakeDamage(Mathf.RoundToInt(finalPhysicalDamage), 0, currentDamageAnimation, characterManager);
            //}
        }
    }
}

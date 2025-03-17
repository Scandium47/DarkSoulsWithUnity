using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterCombatManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Combat Transform")]
        public Transform backStabReceiverTransform;
        public Transform riposteReceiverTransform;

        public LayerMask characterLayer;
        public float criticalAttackRange = 0.7f;

        [Header("Last Amount of Poise Damage Taken")]
        public int previousPoiseDamageTaken;

        [Header("Attack Type")]
        public AttackType currentAttackType;

        [Header("Attack Animations")]
        public string oh_light_attack_01 = "OH_Light_Attack_01";
        public string oh_light_attack_02 = "OH_Light_Attack_02";
        public string oh_heavy_attack_01 = "OH_Heavy_Attack_01";
        public string oh_heavy_attack_02 = "OH_Heavy_Attack_02";
        public string oh_running_attack_01 = "OH_Running_Attack_01";
        public string oh_jumping_attack_01 = "OH_Jumping_Attack_01";

        public string oh_charge_attack_01 = "OH_Changing_Attack_Change_01";
        public string oh_charge_attack_02 = "OH_Changing_Attack_Change_02";

        public string th_light_attack_01 = "TH_Light_Attack_01";
        public string th_light_attack_02 = "TH_Light_Attack_02";
        public string th_light_attack_03 = "TH_Light_Attack_03";
        public string th_heavy_attack_01 = "TH_Heavy_Attack_01";
        public string th_heavy_attack_02 = "TH_Heavy_Attack_02";
        public string th_running_attack_01 = "TH_Running_Attack_01";
        public string th_jumping_attack_01 = "TH_Jumping_Attack_01";

        public string th_charge_attack_01 = "TH_Changing_Attack_Change_01";
        public string th_charge_attack_02 = "TH_Changing_Attack_Change_02";

        public string weapon_art = "Parry";
        public int pendingCriticalDamage;
        public string lastAttack;

        protected virtual void Awake()
        {
           character = GetComponent<CharacterManager>(); 
        }

        public virtual void SetBlockingAbsorptionsFromBlockingWeapon()
        {
            if(character.isUsingRightHand)
            {
                character.characterStatsManager.blockingPhysicalDamageAbsorption = character.characterInventoryManager.rightWeapon.physicalBlockingDamageAbsorption;
                character.characterStatsManager.blockingFireDamageAbsorption = character.characterInventoryManager.rightWeapon.fireBlockingDamageAbsorption;
                character.characterStatsManager.blockingStabilityRating = character.characterInventoryManager.rightWeapon.stability;
            }
            else if(character.isUsingLeftHand)
            {
                character.characterStatsManager.blockingPhysicalDamageAbsorption = character.characterInventoryManager.leftWeapon.physicalBlockingDamageAbsorption;
                character.characterStatsManager.blockingFireDamageAbsorption = character.characterInventoryManager.leftWeapon.fireBlockingDamageAbsorption;
                character.characterStatsManager.blockingStabilityRating = character.characterInventoryManager.leftWeapon.stability;
            }
        }

        public virtual void DrainStaminaBasedOnAttack()
        {
            character = GetComponent<CharacterManager>();
        }

        private void SuccessfullyCastSpell()
        {
            character.characterInventoryManager.currentSpell.SuccessfullyCastSpell(character);
            //player.animator.SetBool("isFiringSpell", true);
        }

        IEnumerator ForceMoveCharacterToEnemyBackStabPosition(CharacterManager characterPerformingBackStab)
        {
            for (float timer = 0.05f;  timer < 0.5f; timer = timer + 0.05f)
            {
                Quaternion backstabRotation = Quaternion.LookRotation(characterPerformingBackStab.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, backstabRotation, 1);
                transform.parent = characterPerformingBackStab.characterCombatManager.backStabReceiverTransform;
                transform.localPosition = characterPerformingBackStab.characterCombatManager.backStabReceiverTransform.localPosition; ;
                transform.parent = null;
                Debug.Log("正在协程");
                yield return new WaitForSeconds(0.05f);
            }
        }

        IEnumerator ForceMoveCharacterToEnemyRipostedPosition(CharacterManager characterPerformingRiposte)
        {
            for (float timer = 0.05f; timer < 0.5f; timer = timer + 0.05f)
            {
                Quaternion riposteRotation = Quaternion.LookRotation(-characterPerformingRiposte.transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, riposteRotation, 1);
                transform.parent = characterPerformingRiposte.characterCombatManager.riposteReceiverTransform;
                transform.localPosition = characterPerformingRiposte.characterCombatManager.riposteReceiverTransform.localPosition; ;
                transform.parent = null;
                Debug.Log("正在协程");
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void GetBackStabbed(CharacterManager characterPerformingBackStab)
        {
            //Sound FX
            character.isBeingBackstabbed = true;

            //与背刺者对齐，锁定位置
            StartCoroutine(ForceMoveCharacterToEnemyBackStabPosition(characterPerformingBackStab));
            
            character.characterAnimatorManager.PlayTargetAnimation("Back Stabbed", true);
        }

        public void GetRiposted(CharacterManager characterPerformingRiposted)
        {
            //Sound FX
            character.isBeingRiposted = true;

            //与背刺者对齐，锁定位置
            StartCoroutine(ForceMoveCharacterToEnemyRipostedPosition(characterPerformingRiposted));

            character.characterAnimatorManager.PlayTargetAnimation("Riposted", true);
        }

        public void AttemptBackStabOrRiposte()
        {
            if (character.isInteracting)
                return;

            if (character.characterStatsManager.currentStamina <= 0)
                return;

            RaycastHit hit;

            if (Physics.Raycast(character.criticalAttackRayCastStartPoint.transform.position, character.transform.TransformDirection(Vector3.forward), out hit, criticalAttackRange, characterLayer))
            {
                CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();
                Vector3 directionFromCharacterToEnemy = transform.position - enemyCharacter.transform.position;
                float dotValue = Vector3.Dot(directionFromCharacterToEnemy, enemyCharacter.transform.forward);

                Debug.Log("点值为：" + dotValue);

                if(enemyCharacter.canBeRiposted)
                {
                    if (dotValue <= 1.2f && dotValue >= 0.6f)
                    {
                        //处决
                        AttemptRiposte(hit);
                        return;
                    }
                }

                if(dotValue >= -0.9f && dotValue <= -0.6f)
                {
                    //背刺
                    AttemptBackStab(hit);
                }
            }
        }

        private void AttemptBackStab(RaycastHit hit)
        {
            CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();

            if(enemyCharacter !=  null)
            {
                if(!enemyCharacter.isBeingBackstabbed || !enemyCharacter.isBeingRiposted)
                {
                    //处决或背刺无敌
                    EnableIsInvulnerable();
                    character.isPerformingBackstab = true;
                    character.characterAnimatorManager.EraseHandIKForWeapon();

                    character.characterAnimatorManager.PlayTargetAnimation("Back Stab", true);

                    float criticalDamage = (character.characterInventoryManager.rightWeapon.criticalDamageMuiltiplier *
                        (character.characterInventoryManager.rightWeapon.physicalDamage +
                        character.characterInventoryManager.rightWeapon.fireDamage));

                    int roundedCriticalDamage = Mathf.RoundToInt(criticalDamage);
                    enemyCharacter.characterCombatManager.pendingCriticalDamage = roundedCriticalDamage;
                    enemyCharacter.characterCombatManager.GetBackStabbed(character);
                }
            }
        }

        private void AttemptRiposte(RaycastHit hit)
        {
            CharacterManager enemyCharacter = hit.transform.GetComponent<CharacterManager>();

            if (enemyCharacter != null)
            {
                if (!enemyCharacter.isBeingBackstabbed || !enemyCharacter.isBeingRiposted)
                {
                    //处决或背刺无敌
                    EnableIsInvulnerable();
                    character.isPerformingRiposte = true;
                    character.characterAnimatorManager.EraseHandIKForWeapon();

                    character.characterAnimatorManager.PlayTargetAnimation("Riposte", true);

                    float criticalDamage = (character.characterInventoryManager.rightWeapon.criticalDamageMuiltiplier *
                        (character.characterInventoryManager.rightWeapon.physicalDamage +
                        character.characterInventoryManager.rightWeapon.fireDamage));

                    int roundedCriticalDamage = Mathf.RoundToInt(criticalDamage);
                    enemyCharacter.characterCombatManager.pendingCriticalDamage = roundedCriticalDamage;
                    enemyCharacter.characterCombatManager.GetRiposted(character);
                }
            }
        }

        private void EnableIsInvulnerable()
        {
            character.animator.SetBool("isInvulnerable", true);
        }

        public void ApplyPendingDamage()
        {
            character.characterStatsManager.TakeDamageNoAnimation(pendingCriticalDamage, 0);
        }

        public void EnableCanBeParried()
        {
            character.canBeParried = true;
        }

        public void DisableCanBeParried()
        {
            character.canBeParried = false;
        }

        #region 弃用以射线检测碰撞器的方式处理背刺和处决
        //public void AttemptBackStabOrRiposte()
        //{
        //    if (character.characterStatsManager.currentStamina <= 0)
        //        return;

        //    RaycastHit hit;

        //    if (Physics.Raycast(character.criticalAttackRayCastStartPoint.position,
        //        transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer))
        //    {
        //        CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
        //        DamageCollider rightWeapon = character.characterWeaponSlotManager.rightHandDamageCollider;

        //        if (enemyCharacterManager != null)
        //        {
        //            //Check for team I.D ( So you cant back stab friends or yourself?)

        //            //Pull is into a transform behind the enemy so the backstab looks clean 转到敌人后方位置
        //            character.transform.position = enemyCharacterManager.backStabCollider.criticalDamagerStandPosition.position;

        //            //rotate us towards that transform 转向敌人后方 
        //            Vector3 rotationDirection = character.transform.root.eulerAngles;
        //            rotationDirection = hit.transform.position - character.transform.position;
        //            rotationDirection.y = 0;
        //            rotationDirection.Normalize();
        //            Quaternion tr = Quaternion.LookRotation(rotationDirection);
        //            Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, tr, 500 * Time.deltaTime);
        //            character.transform.rotation = targetRotation;

        //            //致命伤害
        //            int criticalDamage = character.characterInventoryManager.rightWeapon.criticalDamageMuiltiplier * rightWeapon.physicalDamage;
        //            enemyCharacterManager.pendingCriticalDamage = criticalDamage;

        //            //play animation 引用被刺动画
        //            character.characterAnimatorManager.PlayTargetAnimation("Back Stab", true);
        //            character.isInteracting = character.animator.GetBool("isInteracting");
        //            //清除双持IK（权重降为零0）
        //            character.characterAnimatorManager.EraseHandIKForWeapon();
        //            enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Back Stabbed", true);
        //        }
        //    }
        //    else if (Physics.Raycast(character.criticalAttackRayCastStartPoint.position,
        //        transform.TransformDirection(Vector3.forward), out hit, 0.7f, riposteLayer))
        //    {
        //        //Charck for them I.D
        //        CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
        //        DamageCollider rightWeapon = character.characterWeaponSlotManager.rightHandDamageCollider;

        //        if (enemyCharacterManager != null && enemyCharacterManager.canBeRiposted == true)
        //        {
        //            character.transform.position = enemyCharacterManager.riposteCollider.criticalDamagerStandPosition.position;

        //            Vector3 rotationDirection = character.transform.root.eulerAngles;
        //            rotationDirection = hit.transform.position - character.transform.position;
        //            rotationDirection.y = 0;
        //            Quaternion tr = Quaternion.LookRotation(rotationDirection);
        //            Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, tr, 500 * Time.deltaTime);
        //            character.transform.rotation = targetRotation;

        //            int criticalDamage = character.characterInventoryManager.rightWeapon.criticalDamageMuiltiplier * rightWeapon.physicalDamage;
        //            enemyCharacterManager.pendingCriticalDamage = criticalDamage;

        //            character.characterAnimatorManager.PlayTargetAnimation("Riposte", true);
        //            character.isInteracting = character.animator.GetBool("isInteracting");
        //            //清除双持IK（权重降为零0）
        //            character.characterAnimatorManager.EraseHandIKForWeapon();
        //            enemyCharacterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Riposted", true);
        //        }
        //    }
        //}
        #endregion
    }
}
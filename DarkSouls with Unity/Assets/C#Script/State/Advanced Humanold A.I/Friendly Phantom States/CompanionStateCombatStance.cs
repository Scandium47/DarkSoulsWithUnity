using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStateCombatStance : State
    {
        //战斗状态
        public ItemBasedAttackAction[] enemyAttacks;

        CompanionStateIdle idleState;
        CompanionStateAttackTarget attackState;
        CompanionStateFollowHost followHostState;
        CompanionStatePursueTarget pursueTargetState;

        protected bool randomDestinationSet = false;
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;

        [Header("State Flags")]
        public bool willPerformBlock = false;
        public bool willPerformDodge = false;
        public bool willPerformParry = false;

        public bool hasPerformedDodge = false;
        public bool hasPerformedParry = false;
        public bool hasRandomDodgeDirection = false;
        public bool hasAmmoLoaded = false;

        Quaternion targetDodgeDirection;

        private void Awake()
        {
            idleState = GetComponent<CompanionStateIdle>();
            attackState = GetComponent<CompanionStateAttackTarget>();
            followHostState = GetComponent<CompanionStateFollowHost>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            //如果离玩家太远，回到玩家
            if (aiCharacter.distanceFromCompanion > aiCharacter.maxDistanceFromCompanion)
            {
                return followHostState;
            }

            if (aiCharacter.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldCombatStyle(aiCharacter);
            }
            else if (aiCharacter.combatStyle == AICombatStyle.archer)
            {
                return ProcessArcherCombatStyle(aiCharacter);
            }
            else
            {
                return this;
            }
        }

        private State ProcessSwordAndShieldCombatStyle(EnemyManager aiCharacter)
        {
            //如果A.I正在坠落，或者正在做其他动画，停止移动
            if (!aiCharacter.isGrounded || aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //如果目标死亡，重置攻击
            if (aiCharacter.currentTarget != null)
            {
                if (aiCharacter.currentTarget.isDead)
                {
                    ResetStateFlags();
                    aiCharacter.currentTarget = null;
                    return idleState;
                }
            }

            //如果玩家抛出范围，切换到追踪状态
            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)
            {
                return pursueTargetState;
            }

            //随机绕着玩家的周旋模式
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(aiCharacter.enemyAnimatorManager);
            }

            //下面的if返回到这里，弹反后如果可以处决，处决
            if (aiCharacter.allowAIToPerformParry)
            {
                if (aiCharacter.currentTarget!= null && aiCharacter.currentTarget.canBeRiposted)
                {
                    CheckForRiposte(aiCharacter);
                    return this;
                }
            }

            if (aiCharacter.allowAIToPerformBlock)
            {
                //随机一个概率格挡
                RollForBlockChance(aiCharacter);
            }

            if (aiCharacter.allowAIToPerformDodge)
            {
                //随机一个概率翻滚
                RollForDodgeChance(aiCharacter);
            }

            if (aiCharacter.allowAIToPerformParry)
            {
                //随机一个概率弹反
                RollForParryChance(aiCharacter);
            }

            //如果对方正在攻击，随机到弹反，并且不是正在弹反状态中，函数内如果在范围内，弹反，返回到上面判断是否能处决
            if (aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                if (willPerformParry && !hasPerformedParry)
                {
                    ParryCurrentTarget(aiCharacter);
                    return this;
                }
            }

            //如果随机到可以格挡，加入韧性，格挡
            if (willPerformBlock)
            {
                BlockUsingOffHand(aiCharacter);
            }

            //如果随机到可以翻滚，并且对方正在攻击，翻滚
            if (willPerformDodge && aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                //如果被攻击，翻滚
                Dodge(aiCharacter);
            }

            HandleRotateTowardsTarget(aiCharacter);

            //如果进入攻击范围转到攻击状态
            if (aiCharacter.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                ResetStateFlags();
                return attackState;
            }
            else
            {
                GetNewAttack(aiCharacter);
            }

            HandleMovement(aiCharacter);

            return this;
        }

        private State ProcessArcherCombatStyle(EnemyManager aiCharacter)
        {
            //如果A.I正在坠落，或者正在做其他动画，停止移动
            if (!aiCharacter.isGrounded || aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //如果目标死亡，重置攻击
            if (aiCharacter.currentTarget != null)
            {
                if (aiCharacter.currentTarget.isDead)
                {
                    ResetStateFlags();
                    aiCharacter.currentTarget = null;
                    return idleState;
                }
            }

            //如果玩家超出范围，切换到追踪状态
            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }

            //随机绕着玩家的周旋模式
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(aiCharacter.enemyAnimatorManager);
            }

            if (aiCharacter.allowAIToPerformDodge)
            {
                //随机一个概率翻滚
                RollForDodgeChance(aiCharacter);
            }

            if (willPerformDodge && aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                //如果被攻击，翻滚
                Dodge(aiCharacter);
            }

            HandleRotateTowardsTarget(aiCharacter);

            if (!hasAmmoLoaded)
            {
                DrawArrow(aiCharacter);
                //在射箭前瞄准敌人
                AimAtTargetBeforeFiring(aiCharacter);
            }

            //如果进入攻击范围转到攻击状态
            if (aiCharacter.currentRecoveryTime <= 0 && hasAmmoLoaded)
            {
                ResetStateFlags();
                return attackState;
            }

            if (aiCharacter.isStationaryArcher)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            }
            else
            {
                HandleMovement(aiCharacter);
            }

            return this;
        }

        protected void HandleRotateTowardsTarget(EnemyManager aiCharacter)
        {
            Vector3 direction = aiCharacter.currentTarget.transform.position - aiCharacter.transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, aiCharacter.rotationSpeed * Time.deltaTime);
        }

        protected void DecideCirclingAction(EnemyAnimatorManager enemyAnimatorManager)
        {
            WalkAroundTarget(enemyAnimatorManager);
        }

        protected void WalkAroundTarget(EnemyAnimatorManager enemyAnimatorManager)
        {
            verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);

            if (horizontalMovementValue <= 1 && horizontalMovementValue >= 0)
            {
                horizontalMovementValue = 0.5f;
            }
            else if (horizontalMovementValue >= -1 && horizontalMovementValue < 0)
            {
                horizontalMovementValue = -0.5f;
            }
        }

        protected virtual void GetNewAttack(EnemyManager aiCharacter)
        {
            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];

                if (aiCharacter.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && aiCharacter.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (aiCharacter.viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && aiCharacter.viewableAngle >= enemyAttackAction.attackScore)
                    {
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];

                if (aiCharacter.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && aiCharacter.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (aiCharacter.viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && aiCharacter.viewableAngle >= enemyAttackAction.attackScore)
                    {
                        if (attackState.currentAttack != null)
                            return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if (temporaryScore > randomValue)
                        {
                            attackState.currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }

        //AI 翻滚
        private void RollForBlockChance(EnemyManager aiCharacter)
        {
            int blockChance = Random.Range(0, 100);

            if (blockChance <= aiCharacter.blockLikelyHood)
            {
                willPerformBlock = true;
            }
            else
            {
                willPerformBlock = false;
            }
        }

        private void RollForDodgeChance(EnemyManager aiCharacter)
        {
            int dodgeChance = Random.Range(0, 100);

            if (dodgeChance <= aiCharacter.dodgeLikelyHood)
            {
                willPerformDodge = true;
            }
            else
            {
                willPerformDodge = false;
            }
        }

        private void RollForParryChance(EnemyManager aiCharacter)
        {
            int parryChance = Random.Range(0, 100);

            if (parryChance <= aiCharacter.parryLikelyHood)
            {
                willPerformParry = true;
            }
            else
            {
                willPerformParry = false;
            }
        }

        private void ResetStateFlags()
        {
            hasRandomDodgeDirection = false;
            hasPerformedDodge = false;
            hasAmmoLoaded = false;
            hasPerformedParry = false;

            randomDestinationSet = false;

            willPerformBlock = false;
            willPerformDodge = false;
            willPerformParry = false;
        }

        //AI Actions
        private void BlockUsingOffHand(EnemyManager aiCharacter)
        {
            if (aiCharacter.isBlocking == false)
            {
                if (aiCharacter.allowAIToPerformBlock)
                {
                    aiCharacter.isBlocking = true;
                    aiCharacter.UpdateWhitchHandCharacterIsUsing(false);
                    aiCharacter.characterInventoryManager.currentItemBeingUsed = aiCharacter.characterInventoryManager.leftWeapon;
                    aiCharacter.characterCombatManager.SetBlockingAbsorptionsFromBlockingWeapon();
                }
            }
        }

        private void Dodge(EnemyManager aiCharacter)
        {
            if (!hasPerformedDodge)
            {
                if (!hasRandomDodgeDirection)
                {
                    float randomDodgeDirection;

                    hasRandomDodgeDirection = true;
                    randomDodgeDirection = Random.Range(0, 360);
                    targetDodgeDirection = Quaternion.Euler(aiCharacter.transform.eulerAngles.x, randomDodgeDirection, aiCharacter.transform.eulerAngles.z);
                }

                if (aiCharacter.transform.rotation != targetDodgeDirection)
                {
                    Quaternion targetRotatiom = Quaternion.Slerp(aiCharacter.transform.rotation, targetDodgeDirection, 1f);
                    aiCharacter.transform.rotation = targetRotatiom;

                    float targetYRotation = targetDodgeDirection.eulerAngles.y;
                    float currentYRotation = aiCharacter.transform.eulerAngles.y;
                    float rotationDifference = Mathf.Abs(targetYRotation - currentYRotation);

                    if (rotationDifference <= 5)
                    {
                        hasPerformedDodge = true;
                        aiCharacter.transform.rotation = targetDodgeDirection;
                        aiCharacter.enemyAnimatorManager.PlayTargetAnimation("Rolling", true);
                    }
                }
            }
        }

        private void DrawArrow(EnemyManager aiCharacter)
        {
            if (!aiCharacter.isTwoHandingWeapon)
            {
                aiCharacter.isTwoHandingWeapon = true;
                aiCharacter.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
            }
            else
            {
                hasAmmoLoaded = true;
                aiCharacter.characterInventoryManager.currentItemBeingUsed = aiCharacter.characterInventoryManager.rightWeapon;
                aiCharacter.characterInventoryManager.rightWeapon.th_hold_RB_Action.PerformAction(aiCharacter);
            }
        }

        private void AimAtTargetBeforeFiring(EnemyManager aiCharacter)
        {
            float timeUntilAmmoIsShotAtTarget = Random.Range(aiCharacter.minimumTimeToAimAtTarget, aiCharacter.maximumTimeToAimTarget);
            aiCharacter.currentRecoveryTime = timeUntilAmmoIsShotAtTarget;
        }

        private void ParryCurrentTarget(EnemyManager aiCharacter)
        {
            if (aiCharacter.currentTarget.canBeParried)
            {
                if (aiCharacter.distanceFromTarget <= 2)
                {
                    hasPerformedParry = true;
                    aiCharacter.isParrying = true;
                    aiCharacter.enemyAnimatorManager.PlayTargetAnimation("Parry", true);
                }
            }
        }

        private void CheckForRiposte(EnemyManager aiCharacter)
        {
            //弹反后去对方面前
            if (aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                return;
            }
            if (aiCharacter.distanceFromTarget >= 1)
            {
                HandleRotateTowardsTarget(aiCharacter);
                aiCharacter.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);
            }
            else
            {
                Debug.Log("进入距离可以处决");
                aiCharacter.isBlocking = false;
                if (!aiCharacter.isInteracting && !aiCharacter.currentTarget.isBeingRiposted && !aiCharacter.currentTarget.isBeingBackstabbed)
                {
                    Debug.Log("雀食可以处决");
                    aiCharacter.enemyRigidBody.velocity = Vector3.zero;
                    aiCharacter.animator.SetFloat("Vertical", 0);
                    aiCharacter.characterCombatManager.AttemptBackStabOrRiposte();
                }
            }
        }

        private void HandleMovement(EnemyManager aiCharacter)
        {
            if (aiCharacter.distanceFromTarget <= aiCharacter.stoppingDistance)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
            else
            {
                aiCharacter.animator.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
        }
    }
}
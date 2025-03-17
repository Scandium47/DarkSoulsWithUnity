using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStateCombatStance : State
    {
        //ս��״̬
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
            //��������̫Զ���ص����
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
            //���A.I����׹�䣬��������������������ֹͣ�ƶ�
            if (!aiCharacter.isGrounded || aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //���Ŀ�����������ù���
            if (aiCharacter.currentTarget != null)
            {
                if (aiCharacter.currentTarget.isDead)
                {
                    ResetStateFlags();
                    aiCharacter.currentTarget = null;
                    return idleState;
                }
            }

            //�������׳���Χ���л���׷��״̬
            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)
            {
                return pursueTargetState;
            }

            //���������ҵ�����ģʽ
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(aiCharacter.enemyAnimatorManager);
            }

            //�����if���ص����������������Դ���������
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
                //���һ�����ʸ�
                RollForBlockChance(aiCharacter);
            }

            if (aiCharacter.allowAIToPerformDodge)
            {
                //���һ�����ʷ���
                RollForDodgeChance(aiCharacter);
            }

            if (aiCharacter.allowAIToPerformParry)
            {
                //���һ�����ʵ���
                RollForParryChance(aiCharacter);
            }

            //����Է����ڹ�������������������Ҳ������ڵ���״̬�У�����������ڷ�Χ�ڣ����������ص������ж��Ƿ��ܴ���
            if (aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                if (willPerformParry && !hasPerformedParry)
                {
                    ParryCurrentTarget(aiCharacter);
                    return this;
                }
            }

            //�����������Ը񵲣��������ԣ���
            if (willPerformBlock)
            {
                BlockUsingOffHand(aiCharacter);
            }

            //�����������Է��������ҶԷ����ڹ���������
            if (willPerformDodge && aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                //���������������
                Dodge(aiCharacter);
            }

            HandleRotateTowardsTarget(aiCharacter);

            //������빥����Χת������״̬
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
            //���A.I����׹�䣬��������������������ֹͣ�ƶ�
            if (!aiCharacter.isGrounded || aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //���Ŀ�����������ù���
            if (aiCharacter.currentTarget != null)
            {
                if (aiCharacter.currentTarget.isDead)
                {
                    ResetStateFlags();
                    aiCharacter.currentTarget = null;
                    return idleState;
                }
            }

            //�����ҳ�����Χ���л���׷��״̬
            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }

            //���������ҵ�����ģʽ
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(aiCharacter.enemyAnimatorManager);
            }

            if (aiCharacter.allowAIToPerformDodge)
            {
                //���һ�����ʷ���
                RollForDodgeChance(aiCharacter);
            }

            if (willPerformDodge && aiCharacter.currentTarget != null && aiCharacter.currentTarget.isAttacking)
            {
                //���������������
                Dodge(aiCharacter);
            }

            HandleRotateTowardsTarget(aiCharacter);

            if (!hasAmmoLoaded)
            {
                DrawArrow(aiCharacter);
                //�����ǰ��׼����
                AimAtTargetBeforeFiring(aiCharacter);
            }

            //������빥����Χת������״̬
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

        //AI ����
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
            //������ȥ�Է���ǰ
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
                Debug.Log("���������Դ���");
                aiCharacter.isBlocking = false;
                if (!aiCharacter.isInteracting && !aiCharacter.currentTarget.isBeingRiposted && !aiCharacter.currentTarget.isBeingBackstabbed)
                {
                    Debug.Log("ȸʳ���Դ���");
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
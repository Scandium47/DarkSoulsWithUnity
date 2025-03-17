using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CombatStanceStateHumanoid : State
    {
        //ս��״̬
        public IdleStateHumanoid idleState;
        public AttackStateHumanoid attackState;
        public ItemBasedAttackAction[] enemyAttacks;
        public PursueTargetStateHumanoid pursueTargetState;

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
            idleState = GetComponent<IdleStateHumanoid>();
            attackState = GetComponent<AttackStateHumanoid>();
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
        }

        public override State Tick(EnemyManager enemy)
        {
            if(enemy.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldCombatStyle(enemy);
            }
            else if(enemy.combatStyle == AICombatStyle.archer)
            {
                return ProcessArcherCombatStyle(enemy);
            }
            else
            {
                return this;
            }
        }

        private State ProcessSwordAndShieldCombatStyle(EnemyManager enemy)
        {
            //���A.I����׹�䣬��������������������ֹͣ�ƶ�
            if(!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //���Ŀ�����������ù���
            if (enemy.currentTarget != null)
            {
                if (enemy.currentTarget.isDead)
                {
                    ResetStateFlags();
                    enemy.currentTarget = null;
                    return idleState;
                }
            }

            //�������׳���Χ���л���׷��״̬
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                return pursueTargetState;
            }

            //���������ҵ�����ģʽ
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy.enemyAnimatorManager);
            }

            //�����if���ص����������������Դ���������
            if(enemy.allowAIToPerformParry)
            {
                if(enemy.currentTarget != null && enemy.currentTarget.canBeRiposted)
                {
                    CheckForRiposte(enemy);
                    return this;
                }
            }

            if(enemy.allowAIToPerformBlock)
            {
                //���һ�����ʸ�
                RollForBlockChance(enemy);
            }

            if (enemy.allowAIToPerformDodge)
            {
                //���һ�����ʷ���
                RollForDodgeChance(enemy);
            }

            if (enemy.allowAIToPerformParry)
            {
                //���һ�����ʵ���
                RollForParryChance(enemy);
            }

            //����Է����ڹ�������������������Ҳ������ڵ���״̬�У�����������ڷ�Χ�ڣ����������ص������ж��Ƿ��ܴ���
            if(enemy.currentTarget != null && enemy.currentTarget.isAttacking)
            {
                if (willPerformParry && !hasPerformedParry)
                {
                    ParryCurrentTarget(enemy);
                    return this;
                }
            }

            //�����������Ը񵲣��������ԣ���
            if (willPerformBlock)
            {
                BlockUsingOffHand(enemy);
            }

            //�����������Է��������ҶԷ����ڹ���������
            if(willPerformDodge && enemy.currentTarget != null && enemy.currentTarget.isAttacking)
            {
                //���������������
                Dodge(enemy);
            }

            HandleRotateTowardsTarget(enemy);

            //������빥����Χת������״̬
            if (enemy.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                ResetStateFlags();
                return attackState;
            }
            else
            {
                GetNewAttack(enemy);
            }

            HandleMovement(enemy);

            return this;
        }

        private State ProcessArcherCombatStyle(EnemyManager enemy)
        {
            //���A.I����׹�䣬��������������������ֹͣ�ƶ�
            if (!enemy.isGrounded || enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            //���Ŀ�����������ù���
            if (enemy.currentTarget != null)
            {
                if (enemy.currentTarget.isDead)
                {
                    ResetStateFlags();
                    enemy.currentTarget = null;
                    return idleState;
                }
            }

            //�����ҳ�����Χ���л���׷��״̬
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }

            //���������ҵ�����ģʽ
            if (!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy.enemyAnimatorManager);
            }

            if (enemy.allowAIToPerformDodge)
            {
                //���һ�����ʷ���
                RollForDodgeChance(enemy);
            }

            if (willPerformDodge && enemy.currentTarget != null && enemy.currentTarget.isAttacking)
            {
                //���������������
                Dodge(enemy);
            }

            HandleRotateTowardsTarget(enemy);

            if(!hasAmmoLoaded)
            {
                DrawArrow(enemy);
                //�����ǰ��׼����
                AimAtTargetBeforeFiring(enemy);
            }

            //������빥����Χת������״̬
            if (enemy.currentRecoveryTime <= 0 && hasAmmoLoaded)
            {
                ResetStateFlags();
                return attackState;
            }

            if(enemy.isStationaryArcher)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            }
            else
            {
                HandleMovement(enemy);
            }

            return this;
        }

        protected void HandleRotateTowardsTarget(EnemyManager enemy)
        {
            Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);

            /*
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            */
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

        protected virtual void GetNewAttack(EnemyManager enemy)
        {
            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                ItemBasedAttackAction enemyAttackAction = enemyAttacks[i];

                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && enemy.viewableAngle >= enemyAttackAction.attackScore)
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

                if (enemy.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && enemy.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (enemy.viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && enemy.viewableAngle >= enemyAttackAction.attackScore)
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
        private void RollForBlockChance(EnemyManager enemy)
        {
           int blockChance = Random.Range(0, 100);

            if(blockChance <= enemy.blockLikelyHood)
            {
                willPerformBlock = true;
            }
            else
            {
                willPerformBlock = false;
            }
        }

        private void RollForDodgeChance(EnemyManager enemy)
        {
            int dodgeChance = Random.Range(0, 100);

            if(dodgeChance <= enemy.dodgeLikelyHood)
            {
                willPerformDodge = true;
            }
            else
            {
                willPerformDodge= false;
            }
        }

        private void RollForParryChance(EnemyManager enemy)
        {
            int parryChance = Random.Range(0, 100);

            if(parryChance <= enemy.parryLikelyHood)
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
        private void BlockUsingOffHand(EnemyManager enemy)
        {
            if(enemy.isBlocking == false)
            {
                if(enemy.allowAIToPerformBlock)
                {
                    enemy.isBlocking = true;
                    enemy.UpdateWhitchHandCharacterIsUsing(false);
                    enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.leftWeapon;
                    enemy.characterCombatManager.SetBlockingAbsorptionsFromBlockingWeapon();
                }
            }
        }

        private void Dodge(EnemyManager enemy)
        {
            if(!hasPerformedDodge)
            {
                if(!hasRandomDodgeDirection)
                {
                    float randomDodgeDirection;

                    hasRandomDodgeDirection = true;
                    randomDodgeDirection = Random.Range(0, 360);
                    targetDodgeDirection = Quaternion.Euler(enemy.transform.eulerAngles.x, randomDodgeDirection, enemy.transform.eulerAngles.z);
                }

                if(enemy.transform.rotation != targetDodgeDirection)
                {
                    Quaternion targetRotatiom = Quaternion.Slerp(enemy.transform.rotation, targetDodgeDirection, 1f);
                    enemy.transform.rotation = targetRotatiom;

                    float targetYRotation = targetDodgeDirection.eulerAngles.y;
                    float currentYRotation = enemy.transform.eulerAngles.y;
                    float rotationDifference = Mathf.Abs(targetYRotation - currentYRotation);

                    if(rotationDifference <= 5)
                    {
                        hasPerformedDodge = true;
                        enemy.transform.rotation = targetDodgeDirection;
                        enemy.enemyAnimatorManager.PlayTargetAnimation("Rolling", true);
                    }
                }
            }
        }

        private void DrawArrow(EnemyManager enemy)
        {
            if(!enemy.isTwoHandingWeapon)
            {
                enemy.isTwoHandingWeapon = true;
                enemy.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
            }
            else
            {
                hasAmmoLoaded = true;
                enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.rightWeapon;
                enemy.characterInventoryManager.rightWeapon.th_hold_RB_Action.PerformAction(enemy);
            }
        }

        private void AimAtTargetBeforeFiring(EnemyManager enemy)
        {
            float timeUntilAmmoIsShotAtTarget = Random.Range(enemy.minimumTimeToAimAtTarget, enemy.maximumTimeToAimTarget);
            enemy.currentRecoveryTime = timeUntilAmmoIsShotAtTarget;
        }

        private void ParryCurrentTarget(EnemyManager enemy)
        {
            if(enemy.currentTarget.canBeParried)
            {
                if(enemy.distanceFromTarget <= 2)
                {
                    hasPerformedParry = true;
                    enemy.isParrying = true;
                    enemy.enemyAnimatorManager.PlayTargetAnimation("Parry", true);
                }
            }
        }

        private void CheckForRiposte(EnemyManager enemy)
        {
            //������ȥ�Է���ǰ
            if(enemy.isInteracting)
            {
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                return;
            }
            if(enemy.distanceFromTarget >= 1)
            {
                HandleRotateTowardsTarget(enemy);
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Vertical", 1, 0.2f, Time.deltaTime);
            }
            else
            {
                Debug.Log("���������Դ���");
                enemy.isBlocking = false;
                if(!enemy.isInteracting && !enemy.currentTarget.isBeingRiposted && !enemy.currentTarget.isBeingBackstabbed)
                {
                    Debug.Log("ȸʳ���Դ���");
                    enemy.enemyRigidBody.velocity = Vector3.zero;
                    enemy.animator.SetFloat("Vertical", 0);
                    enemy.characterCombatManager.AttemptBackStabOrRiposte();
                }
            }
        }

        private void HandleMovement(EnemyManager enemy)
        {
            if(enemy.distanceFromTarget <= enemy.stoppingDistance)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
            else
            {
                enemy.animator.SetFloat("Vertical", verticalMovementValue, 0.2f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", horizontalMovementValue, 0.2f, Time.deltaTime);
            }
        }
    }
}
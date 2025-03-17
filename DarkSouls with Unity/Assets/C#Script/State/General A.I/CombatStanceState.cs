using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CombatStanceState : State
    {
        //ս��״̬
        public IdleState idleState;
        public AttackState attackState;
        public EnemyAttackAction[] enemyAttacks;
        public PursueTargetState pursueTargetState;

        protected bool randomDestinationSet = false;
        protected float verticalMovementValue = 0;
        protected float horizontalMovementValue = 0;
        public override State Tick(EnemyManager enemy)
        {
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

            attackState.hasPerformedAttack = false;

            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                //if the player runs out of range return the pursuetarget state
                return pursueTargetState;
            }

            if(!randomDestinationSet)
            {
                randomDestinationSet = true;
                DecideCirclingAction(enemy.enemyAnimatorManager);
            }

            HandleRotateTowardsTarget(enemy);

            //Check for attack range
            if(enemy.currentRecoveryTime <= 0 && attackState.currentAttack != null)
            {
                randomDestinationSet = false;
                return attackState;
            }
            else
            {
                GetNewAttack(enemy);
            }

            CheckIfWeAreTooClose(enemy);

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

            #region ���õ�navimesh��ת
            /*
            //Rotate manually
            if (enemy.isPreformingAction)
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
            }
            //Rotate with pathfinding(navmesh)
            else
            {
                //���л���������������ת��ʽ
                //����navMeshAgent�����õ���ǰ����Ŀ��λ��
                enemy.navMeshAgent.enabled = true;
                enemy.navMeshAgent.SetDestination(enemy.currentTarget.transform.position);

                //������Ҫ��ת�ĽǶ� �� Ŀ�����
                float rotationToApplyToDynamicEnemy =
                    Quaternion.Angle(enemy.transform.rotation, Quaternion.LookRotation(enemy.navMeshAgent.desiredVelocity.normalized));
                float distanceFromTarget = Vector3.Distance(enemy.currentTarget.transform.position, enemy.transform.position);

                //���ݾ������Ҫ��ת�ĽǶȵ������ٶ�
                if (distanceFromTarget > 5)
                {
                    enemy.navMeshAgent.angularSpeed = 500f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) < 30)
                {
                    enemy.navMeshAgent.angularSpeed = 50f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) > 30)
                {
                    enemy.navMeshAgent.angularSpeed = 500f;
                }

                //������룬Ŀ��λ��-��������λ��
                Vector3 targetDirection = enemy.currentTarget.transform.position - enemy.transform.position;
                //ͨ�����뷽λ���㿴����˵���ת�Ƕ�
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
                //navMeshAgent�������ٶ�>0���������ڵ����ƶ���ȡ����������ת��һ�󵼺�
                if (enemy.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    enemy.navMeshAgent.updateRotation = false;
                    enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation,
                        Quaternion.LookRotation(enemy.navMeshAgent.desiredVelocity.normalized), enemy.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                //navMeshAgent�������ٶ�<=0�����˾�ֹ��ʹ������ļ���Ƕ���ת
                else
                {
                    enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation,
                        rotationToApplyToStaticEnemy, enemy.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
            */

            /*
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            */
            #endregion
        }

        protected void DecideCirclingAction(EnemyAnimatorManager enemyAnimatorManager)
        {
            WalkAroundTarget(enemyAnimatorManager);
        }

        protected void WalkAroundTarget(EnemyAnimatorManager enemyAnimatorManager)
        {
            verticalMovementValue = 0.5f;

            horizontalMovementValue = Random.Range(-1, 1);

            if(horizontalMovementValue <= 1 && horizontalMovementValue >= 0)
            {
                horizontalMovementValue = 0.5f;
            }
            else if(horizontalMovementValue >= -1 && horizontalMovementValue < 0)
            {
                horizontalMovementValue = -0.5f;
            }
        }

        protected virtual void GetNewAttack(EnemyManager enemy)
        {
            int maxScore = 0;

            for (int i = 0; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

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
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

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

        private void CheckIfWeAreTooClose(EnemyManager enemy)
        {
            if (enemy.distanceFromTarget <= enemy.stoppingDistance)
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

        private void ResetStateFlags()
        {
            attackState.willDoComboOnNextAttack = false;
            attackState.hasPerformedAttack = false;
        }
    }
}
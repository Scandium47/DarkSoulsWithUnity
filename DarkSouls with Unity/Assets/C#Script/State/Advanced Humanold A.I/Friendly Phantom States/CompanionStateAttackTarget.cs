using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStateAttackTarget : State
    {
        //����״̬
        CompanionStateIdle idleState;
        CompanionStateRotateTowardsTarget rotateTowardsTargetState;
        CompanionStatePursueTarget combatStanceState;
        CompanionStatePursueTarget pursueTargetState;
        public ItemBasedAttackAction currentAttack;

        bool willDoComboOnNextAttack = false;
        public bool hasPerformedAttack = false;

        private void Awake()
        {
            idleState = GetComponent<CompanionStateIdle>();
            rotateTowardsTargetState = GetComponent<CompanionStateRotateTowardsTarget>();
            combatStanceState = GetComponent<CompanionStatePursueTarget>();
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
        }

        public override State Tick(EnemyManager enemy)
        {
            if (enemy.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldCombatStyle(enemy);
            }
            else if (enemy.combatStyle == AICombatStyle.archer)
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

            RotateTowardsTargetWhilstAttacking(enemy);

            Debug.Log(enemy.distanceFromTarget);

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                return pursueTargetState;
            }

            if (willDoComboOnNextAttack && enemy.canDoCombo)
            {
                AttackTargetWithCombo(enemy);
            }

            if (!hasPerformedAttack)
            {
                AttackTarget(enemy);
                RollForComboChance(enemy);
            }

            if (willDoComboOnNextAttack && hasPerformedAttack)
            {
                enemy.animator.SetBool("canDoCombo", true);
                return this;    //Goes back up to preform the combo
            }

            ResetStateFlags();

            return rotateTowardsTargetState;

            #region ���õ�ս��״̬��
            //if (enemyManager.isInteracting && enemyManager.canDoCombo == false)
            //{
            //    return this;
            //}
            //else if(enemyManager.isInteracting && enemyManager.canDoCombo)
            //{
            //    if (willDoComboOnNextAttack)
            //    {
            //        willDoComboOnNextAttack = false;
            //        enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //    }
            //}

            //Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            //float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            //float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            //HandleRotateTowardsTarget(enemyManager);

            //if (enemyManager.isPreformingAction)
            //{
            //    return combatStanceState;
            //}

            //if(currentAttack != null)
            //{
            //    //If we are too close to the enemy to preform current attack, get a new attack �������˲�������ѡһ���¹���
            //    if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
            //    {
            //        return this;
            //    }
            //    //If we are close enough to attack, then let us proceed �����ڷ�Χ��
            //    else if(distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
            //    {
            //        //If our enemy is within our attacks viewable angel, we attack �Ƕ��ڷ�Χ��
            //        if(viewableAngle <= currentAttack.maximumAttackAngle &&
            //            viewableAngle >= currentAttack.minimumAttackAngle)
            //        {
            //            //if the attack is viewable, stop our movement and attack our target ֹͣ�ƶ�������
            //            if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPreformingAction == false)
            //            {
            //                ////�������Ԫ�ظ�Ϊ��������Ҫ����Ƿ񵯷��������һ�ι������ᱻ���� ���ܺ�������
            //                //PlayerManager playerManager = FindObjectOfType<PlayerManager>();
            //                //if (playerManager != null && playerManager.isParrying)
            //                //{
            //                //    enemyAnimatorManager.PlayTargetAnimation("Parried", true);
            //                //}
            //                //else
            //                //{
            //                enemyAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            //                enemyAnimatorManager.anim.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            //                enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //                enemyManager.isPreformingAction = true;
            //                RollForComboChance(enemyManager);

            //                //����������������ظ�״̬
            //                if (currentAttack.canCombo && willDoComboOnNextAttack)
            //                {
            //                    currentAttack = currentAttack.comboAction;
            //                    return this;
            //                }
            //                //�����������
            //                else
            //                {
            //                    //set our recovery timer to the attacks recovery time ������ȴʱ��
            //                    enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
            //                    currentAttack = null;
            //                    //return the combat stance state ���ص�ս������״̬
            //                    return combatStanceState;
            //                }
            //                //}
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    GetNewAttack(enemyManager);
            //}

            //return combatStanceState;
            #endregion
        }

        private State ProcessArcherCombatStyle(EnemyManager enemy)
        {
            if (enemy.currentTarget != null)
            {
                //���Ŀ�����������ù���
                if (enemy.currentTarget.isDead)
                {
                    ResetStateFlags();
                    enemy.currentTarget = null;
                    return idleState;
                }
            }

            RotateTowardsTargetWhilstAttacking(enemy);

            Debug.Log(enemy.distanceFromTarget);

            if (enemy.isInteracting)
                return this;

            //��������ϣ�����û�й��������ù���������ս��״̬
            if (!enemy.isHoldingArrow)
            {
                ResetStateFlags();
                return combatStanceState;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }

            if (!hasPerformedAttack)
            {
                Debug.Log("���");
                FireAmmo(enemy);
            }

            ResetStateFlags();

            return rotateTowardsTargetState;
        }

        private void AttackTarget(EnemyManager enemy)
        {
            currentAttack.PerformAttackAction(enemy);
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            hasPerformedAttack = true;
        }

        private void AttackTargetWithCombo(EnemyManager enemy)
        {
            currentAttack.PerformAttackAction(enemy);
            willDoComboOnNextAttack = false;
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
        }

        private void RotateTowardsTargetWhilstAttacking(EnemyManager enemyManager)
        {
            //Rotate manually
            if (enemyManager.canRotate && enemyManager.isInteracting)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
            }
            //Rotate with pathfinding(navmesh)
            #region ������Navmesh������ת����
            //else
            //{

            //    //���л���������������ת��ʽ
            //    //����navMeshAgent�����õ���ǰ����Ŀ��λ��
            //    enemyManager.navMeshAgent.enabled = true;
            //    enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);

            //    //������Ҫ��ת�ĽǶ� �� Ŀ�����
            //    float rotationToApplyToDynamicEnemy =
            //        Quaternion.Angle(enemyManager.transform.rotation, Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized));
            //    float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            //    //���ݾ������Ҫ��ת�ĽǶȵ������ٶ�
            //    if (distanceFromTarget > 5)
            //    {
            //        enemyManager.navMeshAgent.angularSpeed = 500f;
            //    }
            //    else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) < 30)
            //    {
            //        enemyManager.navMeshAgent.angularSpeed = 50f;
            //    }
            //    else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) > 30)
            //    {
            //        enemyManager.navMeshAgent.angularSpeed = 500f;
            //    }

            //    //������룬Ŀ��λ��-��������λ��
            //    Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            //    //ͨ�����뷽λ���㿴����˵���ת�Ƕ�
            //    Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
            //    //navMeshAgent�������ٶ�>0���������ڵ����ƶ���ȡ����������ת��һ�󵼺�
            //    if (enemyManager.navMeshAgent.desiredVelocity.magnitude > 0)
            //    {
            //        enemyManager.navMeshAgent.updateRotation = false;
            //        enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
            //            Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized), enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
            //    }
            //    //navMeshAgent�������ٶ�<=0�����˾�ֹ��ʹ������ļ���Ƕ���ת
            //    else
            //    {
            //        enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
            //            rotationToApplyToStaticEnemy, enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
            //    }
            //}
            #endregion

            /*
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            */
        }

        private void RollForComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);

            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood)
            {
                if (currentAttack.actionCanCombo)
                {
                    willDoComboOnNextAttack = true;
                }
                else
                {
                    willDoComboOnNextAttack = false;
                    currentAttack = null;
                }
            }
        }

        private void ResetStateFlags()
        {
            willDoComboOnNextAttack = false;
            hasPerformedAttack = false;
        }

        private void FireAmmo(EnemyManager enemy)
        {
            if (enemy.isHoldingArrow)
            {
                hasPerformedAttack = true;
                enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.rightWeapon;
                enemy.characterInventoryManager.rightWeapon.th_tap_RB_Action.PerformAction(enemy);
            }
        }
    }
}
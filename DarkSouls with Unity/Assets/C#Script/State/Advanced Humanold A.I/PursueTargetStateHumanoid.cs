using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PursueTargetStateHumanoid : State
    {
        //׷��״̬
        public CombatStanceStateHumanoid combatStanceStateHumanoid;

        private void Awake()
        {
            combatStanceStateHumanoid = GetComponent<CombatStanceStateHumanoid>();
        }

        public override State Tick(EnemyManager enemy)
        {
            if(enemy.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldPursueTargetStyle(enemy);
            }
            else if(enemy.combatStyle == AICombatStyle.archer)
            {
                return ProcessArcherPursueTargetStyle(enemy);
            }
            else
            {
                return this;
            }
        }

        private State ProcessArcherPursueTargetStyle(EnemyManager enemy)
        {
            //Angle��0 ���� 180�㣬SignedAngle��-180�� ���� 180��

            HandleRotateTowardsTarget(enemy);

            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                return this;
            }

            if (enemy.isPreformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)   //û������ҵ�ֹͣ���룬�����ߣ�����ֹͣ
            {
                if(!enemy.isStationaryArcher)
                {
                    enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceStateHumanoid;
            }
            else
            {
                return this;
            }
        }

        private State ProcessSwordAndShieldPursueTargetStyle(EnemyManager enemy)
        {
            //Angle��0 ���� 180�㣬SignedAngle��-180�� ���� 180��

            HandleRotateTowardsTarget(enemy);

            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (enemy.isInteracting)
                return this;

            if (enemy.isPreformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)   //û������ҵ�ֹͣ����1.5f�������ߣ�����ֹͣ
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceStateHumanoid;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            //�ֶ���ת������������ִ�ж���ʱ��
            if (enemyManager.isPreformingAction)
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
            //navimesh��ת��������δִ�ж���ʱ��
            else
            {
                //����navMeshAgent�����õ���ǰ����Ŀ��λ��
                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);

                //������Ҫ��ת�ĽǶ� �� Ŀ�����
                float rotationToApplyToDynamicEnemy =
                    Quaternion.Angle(enemyManager.transform.rotation, Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized));
                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

                //���ݾ������Ҫ��ת�ĽǶȵ������ٶ�
                if (distanceFromTarget > 5)
                {
                    enemyManager.navMeshAgent.angularSpeed = 500f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) < 30)
                {
                    enemyManager.navMeshAgent.angularSpeed = 50f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) > 30)
                {
                    enemyManager.navMeshAgent.angularSpeed = 500f;
                }

                //������룬Ŀ��λ��-��������λ��
                Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                //ͨ�����뷽λ���㿴����˵���ת�Ƕ�
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
                //navMeshAgent�������ٶ�>0���������ڵ����ƶ���ȡ����������ת��һ�󵼺�
                if (enemyManager.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    enemyManager.navMeshAgent.updateRotation = false;
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
                        Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized), enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                //navMeshAgent�������ٶ�<=0�����˾�ֹ��ʹ������ļ���Ƕ���ת
                else
                {
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
                        rotationToApplyToStaticEnemy, enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }
}
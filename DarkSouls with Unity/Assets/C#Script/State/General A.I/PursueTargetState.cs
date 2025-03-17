using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PursueTargetState : State
    {
        //׷��״̬
        public CombatStanceState combatStanceState;

        public override State Tick(EnemyManager enemy)
        {
            //Angle��0 ���� 180�㣬SignedAngle��-180�� ���� 180��

            HandleRotateTowardsTarget(enemy);

            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                enemy.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if (enemy.isPreformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)   //û������ҵ�ֹͣ���룬�����ߣ�����ֹͣ
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                enemy.animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            }

            if(enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            //Rotate manually
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
            //Rotate with pathfinding(navmesh)
            else
            {
                //���л���������������ת��ʽ
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
                if(enemyManager.navMeshAgent.desiredVelocity.magnitude > 0)
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

            /*
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
            */
        }
    }
}
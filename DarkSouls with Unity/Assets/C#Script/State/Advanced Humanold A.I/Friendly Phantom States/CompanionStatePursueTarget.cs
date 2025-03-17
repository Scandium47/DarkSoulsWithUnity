using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStatePursueTarget : State
    {
        //׷��״̬
        CompanionStateCombatStance combatStanceState;
        CompanionStateFollowHost followHostState;

        private void Awake()
        {
            combatStanceState = GetComponent<CompanionStateCombatStance>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            if (aiCharacter.distanceFromCompanion > aiCharacter.maxDistanceFromCompanion)
            {
                return followHostState;
            }

            if (aiCharacter.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldPursueTargetStyle(aiCharacter);
            }
            else if (aiCharacter.combatStyle == AICombatStyle.archer)
            {
                return ProcessArcherPursueTargetStyle(aiCharacter);
            }
            else
            {
                return this;
            }
        }

        private State ProcessArcherPursueTargetStyle(EnemyManager aiCharacter)
        {
            //Angle��0 ���� 180�㣬SignedAngle��-180�� ���� 180��

            HandleRotateTowardsTarget(aiCharacter);

            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (aiCharacter.isInteracting)
            {
                aiCharacter.animator.SetFloat("Vertical", 0);
                return this;
            }

            if (aiCharacter.isPreformingAction)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)   //û������ҵ�ֹͣ���룬�����ߣ�����ֹͣ
            {
                if (!aiCharacter.isStationaryArcher)
                {
                    aiCharacter.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
            }

            if (aiCharacter.distanceFromTarget <= aiCharacter.maximumAggroRadius)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
        }

        private State ProcessSwordAndShieldPursueTargetStyle(EnemyManager aiCharacter)
        {
            //Angle��0 ���� 180�㣬SignedAngle��-180�� ���� 180��

            HandleRotateTowardsTarget(aiCharacter);

            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (aiCharacter.isInteracting)
                return this;

            if (aiCharacter.isPreformingAction)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)   //û������ҵ�ֹͣ���룬�����ߣ�����ֹͣ
            {
                aiCharacter.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            if (aiCharacter.distanceFromTarget <= aiCharacter.maximumAggroRadius)
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager aiCharacter)
        {
            //Rotate manually
            if (aiCharacter.isPreformingAction)
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
            //Rotate with pathfinding(navmesh)
            else
            {
                //���л���������������ת��ʽ
                //����navMeshAgent�����õ���ǰ����Ŀ��λ��
                aiCharacter.navMeshAgent.enabled = true;
                aiCharacter.navMeshAgent.SetDestination(aiCharacter.currentTarget.transform.position);

                //������Ҫ��ת�ĽǶ� �� Ŀ�����
                float rotationToApplyToDynamicEnemy =
                    Quaternion.Angle(aiCharacter.transform.rotation, Quaternion.LookRotation(aiCharacter.navMeshAgent.desiredVelocity.normalized));
                float distanceFromTarget = Vector3.Distance(aiCharacter.currentTarget.transform.position, aiCharacter.transform.position);

                //���ݾ������Ҫ��ת�ĽǶȵ������ٶ�
                if (distanceFromTarget > 5)
                {
                    aiCharacter.navMeshAgent.angularSpeed = 500f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) < 30)
                {
                    aiCharacter.navMeshAgent.angularSpeed = 50f;
                }
                else if (distanceFromTarget < 5 && Mathf.Abs(rotationToApplyToDynamicEnemy) > 30)
                {
                    aiCharacter.navMeshAgent.angularSpeed = 500f;
                }

                //������룬Ŀ��λ��-��������λ��
                Vector3 targetDirection = aiCharacter.currentTarget.transform.position - aiCharacter.transform.position;
                //ͨ�����뷽λ���㿴����˵���ת�Ƕ�
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
                //navMeshAgent�������ٶ�>0���������ڵ����ƶ���ȡ����������ת��һ�󵼺�
                if (aiCharacter.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    aiCharacter.navMeshAgent.updateRotation = false;
                    aiCharacter.transform.rotation = Quaternion.RotateTowards(aiCharacter.transform.rotation,
                        Quaternion.LookRotation(aiCharacter.navMeshAgent.desiredVelocity.normalized), aiCharacter.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                //navMeshAgent�������ٶ�<=0�����˾�ֹ��ʹ������ļ���Ƕ���ת
                else
                {
                    aiCharacter.transform.rotation = Quaternion.RotateTowards(aiCharacter.transform.rotation,
                        rotationToApplyToStaticEnemy, aiCharacter.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }
}
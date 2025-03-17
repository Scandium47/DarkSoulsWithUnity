using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStateFollowHost : State
    {
        CompanionStateIdle idleState;

        private void Awake()
        {
            idleState = GetComponent<CompanionStateIdle>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            //If we are preforming an action, stop our movement ���������������������ֹͣ�ƶ��͵���
            if (aiCharacter.isInteracting)
                return this;

            if (aiCharacter.isPreformingAction)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            HandleRotateTowardsTarget(aiCharacter);

            if (aiCharacter.distanceFromCompanion > aiCharacter.maxDistanceFromCompanion)
            {
                aiCharacter.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            if (aiCharacter.distanceFromCompanion <= aiCharacter.retuanDistanceFromCompanion)
            {
                return idleState;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager aiCharacter)
        {
            //����navMeshAgent�����õ���ǰ����Ŀ��λ��
            aiCharacter.navMeshAgent.enabled = true;
            aiCharacter.navMeshAgent.SetDestination(aiCharacter.companion.transform.position);

            //������Ҫ��ת�ĽǶ� �� Ŀ�����
            float rotationToApplyToDynamicEnemy =
                Quaternion.Angle(aiCharacter.transform.rotation, Quaternion.LookRotation(aiCharacter.navMeshAgent.desiredVelocity.normalized));
            float distanceFromTarget = Vector3.Distance(aiCharacter.companion.transform.position, aiCharacter.transform.position);

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
            Vector3 targetDirection = aiCharacter.companion.transform.position - aiCharacter.transform.position;
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
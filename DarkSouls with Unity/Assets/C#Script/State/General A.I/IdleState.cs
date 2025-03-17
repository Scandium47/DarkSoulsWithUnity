using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class IdleState : State
    {
        //վ��״̬
        public PursueTargetState pursueTargetState;

        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;

        public override State Tick(EnemyManager aiCharacter)
        {
            //�ڷ�Χ��Ѱ��Ŀ��
            //����ҵ�Ŀ�꣬�л���PursueTargetState
            //���û��Ŀ�귵�ظ�״̬
            Collider[] colliders = Physics.OverlapSphere(transform.position, aiCharacter.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter != null && !targetCharacter.isDead)
                {
                    //�ж�teamID
                    if (targetCharacter.characterStatsManager.teamIDNumber != aiCharacter.enemyStatsManager.teamIDNumber)
                    {
                        Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                        if (viewableAngle > aiCharacter.minimumDetectionAngle && viewableAngle < aiCharacter.maximumDetectionAngle)
                        {
                            //���AI��Ŀ��֮����ϰ������оͷ���
                            if (Physics.Linecast(aiCharacter.lockOnTransform.position, targetCharacter.lockOnTransform.position, layersThatBlockLineOfSight))
                            {
                                return this;
                            }
                            else
                            {
                                aiCharacter.currentTarget = targetCharacter;
                            }
                        }
                    }
                }
            }
            if (aiCharacter.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class IdleStateHumanoid : State
    {
        //������Ѱ��Ŀ��״̬
        public PursueTargetStateHumanoid pursueTargetStateHumanoid;

        public LayerMask detectionLayer;        //����̽�⵽������
        public LayerMask layersThatBlockLineOfSight;    //�ڵ����ߵ�����

        private void Awake()
        {
            pursueTargetStateHumanoid = GetComponent<PursueTargetStateHumanoid>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            //�ڷ�Χ��Ѱ��Ŀ��
            //����ҵ�Ŀ�꣬�л���PursueTargetState
            //���û��Ŀ�귵�ظ�״̬
            Collider[] colliders = Physics.OverlapSphere(transform.position, aiCharacter.detectionRadius, detectionLayer);      //̽��뾶Ϊ20����������������ײ��

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter != null)
                {
                    //�ж�teamID
                    if (targetCharacter.characterStatsManager.teamIDNumber != aiCharacter.enemyStatsManager.teamIDNumber)
                    {
                        Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                        if (viewableAngle > aiCharacter.minimumDetectionAngle && viewableAngle < aiCharacter.maximumDetectionAngle)
                        {
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
            if (aiCharacter.currentTarget != null && !aiCharacter.isDead)   //����Ŀ����û������
            {
                return pursueTargetStateHumanoid;
            }
            else
            {
                return this;
            }
        }
    }
}
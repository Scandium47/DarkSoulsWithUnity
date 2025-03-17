using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStateIdle : State
    {
        CompanionStatePursueTarget pursueTargetState;
        CompanionStateFollowHost followHostState;

        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;

        private void Awake()
        {
            pursueTargetState = GetComponent<CompanionStatePursueTarget>();
            followHostState = GetComponent<CompanionStateFollowHost>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            aiCharacter.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            aiCharacter.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

            if (aiCharacter.distanceFromCompanion > aiCharacter.maxDistanceFromCompanion)
            {
                return followHostState;
            }

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
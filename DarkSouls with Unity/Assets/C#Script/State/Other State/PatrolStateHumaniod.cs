using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PatrolStateHumanoid : State
    {
        //Ѳ��״̬
        public PursueTargetStateHumanoid pursueTargetState;

        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;

        public bool patrolComplete;
        public bool repeatPatrol;

        //���ִ����һ��Ѳ��
        [Header("Patrol Rest Time")]
        public float endOfPatrolResetTime;
        public float endOfPatrolTimer;

        [Header("Patrol Position")]
        public int patrolDestinationIndex;      //Ѳ�ߵ�����
        public bool hasPatrolDestination;       //�Ƿ���Ѳ�ߵ�
        public Transform currentPatrolDestination;      //��ʱ��Ѳ��λ��
        public float distanceFromCurrentPatrolPoint;        //��Ѳ�ߵ�ľ���
        public List<Transform> listOfPatrolDestinations = new List<Transform>();

        public override State Tick(EnemyManager aiCharacter)
        {
            SearchForTargetWhilstPatroling(aiCharacter);

            if(aiCharacter.isInteracting)
            {
                //���AI�����������������ƶ�����Ϊ��
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if(aiCharacter.currentTarget != null)
            {
                return pursueTargetState;
            }

            //���Ѳ���Ѿ���ɣ���Ҫ�ٴ�Ѳ��
            if(patrolComplete && repeatPatrol)
            {
                //ʱ���ʱ����������ֵ
                if(endOfPatrolResetTime > endOfPatrolTimer)
                {
                    aiCharacter.animator.SetFloat("Vertical", 0f, 0.2f, Time.deltaTime);
                    endOfPatrolTimer = endOfPatrolTimer + Time.deltaTime;
                    return this;
                }
                else if(endOfPatrolResetTime <= endOfPatrolTimer)
                {
                    patrolDestinationIndex = -1;
                    hasPatrolDestination = false;
                    currentPatrolDestination = null;
                    patrolComplete = false;
                    endOfPatrolTimer = 0;
                }
            }
            else if(patrolComplete && !repeatPatrol)
            {
                //���Ѳ�߻�û������Ѳ�ߣ��ر�navimesh
                aiCharacter.navMeshAgent.enabled = false;
                aiCharacter.animator.SetFloat("Vertical", 0f, 0.2f, Time.deltaTime);
                return this;
            }

            //�����Ѳ�ߵ㣬������룬����navimesh
            if(hasPatrolDestination)
            {
                if(currentPatrolDestination != null)
                {
                    distanceFromCurrentPatrolPoint = Vector3.Distance(aiCharacter.transform.position, currentPatrolDestination.transform.position);

                    if(distanceFromCurrentPatrolPoint >1)
                    {
                        aiCharacter.navMeshAgent.enabled = true;
                        aiCharacter.navMeshAgent.destination = currentPatrolDestination.transform.position;
                        Quaternion targetRotation = Quaternion.Lerp(aiCharacter.transform.rotation, aiCharacter.navMeshAgent.transform.rotation, 0.5f);
                        aiCharacter.transform.rotation = targetRotation;
                        aiCharacter.animator.SetFloat("Vertical", 0.5f, 0.2f, Time.deltaTime);
                    }
                    else
                    {
                        currentPatrolDestination = null;
                        hasPatrolDestination = false;
                    }
                }
            }

            //���û��Ѳ�ߵ㣬����+1��Ѳ����ɣ�Ѳ�ߵ�����Ϊ�б�����һ��
            if(!hasPatrolDestination)
            {
                patrolDestinationIndex = patrolDestinationIndex + 1;

                if(patrolDestinationIndex > listOfPatrolDestinations.Count -1)
                {
                    patrolComplete = true;
                    return this;
                }

                currentPatrolDestination = listOfPatrolDestinations[patrolDestinationIndex];
                hasPatrolDestination=true;
            }

            return this;
        }

        private void SearchForTargetWhilstPatroling(EnemyManager aiCharacter)
        {
            //�ڷ�Χ��Ѱ��Ŀ��
            //����ҵ�Ŀ�꣬�л���PursueTargetState
            //���û��Ŀ�귵�ظ�״̬
            Collider[] colliders = Physics.OverlapSphere(transform.position, aiCharacter.detectionRadius, detectionLayer);

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
                                return;
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
                return;
            }
            else
            {
                return;
            }
        }
    }
}
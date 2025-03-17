using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PatrolStateHumanoid : State
    {
        //巡逻状态
        public PursueTargetStateHumanoid pursueTargetState;

        public LayerMask detectionLayer;
        public LayerMask layersThatBlockLineOfSight;

        public bool patrolComplete;
        public bool repeatPatrol;

        //多久执行下一次巡逻
        [Header("Patrol Rest Time")]
        public float endOfPatrolResetTime;
        public float endOfPatrolTimer;

        [Header("Patrol Position")]
        public int patrolDestinationIndex;      //巡逻点索引
        public bool hasPatrolDestination;       //是否有巡逻点
        public Transform currentPatrolDestination;      //此时的巡逻位置
        public float distanceFromCurrentPatrolPoint;        //与巡逻点的距离
        public List<Transform> listOfPatrolDestinations = new List<Transform>();

        public override State Tick(EnemyManager aiCharacter)
        {
            SearchForTargetWhilstPatroling(aiCharacter);

            if(aiCharacter.isInteracting)
            {
                //如果AI正在做动作，设置移动向量为零
                aiCharacter.animator.SetFloat("Vertical", 0);
                aiCharacter.animator.SetFloat("Horizontal", 0);
                return this;
            }

            if(aiCharacter.currentTarget != null)
            {
                return pursueTargetState;
            }

            //如果巡逻已经完成，需要再次巡逻
            if(patrolComplete && repeatPatrol)
            {
                //时间计时结束后，重设值
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
                //完成巡逻还没有重新巡逻，关闭navimesh
                aiCharacter.navMeshAgent.enabled = false;
                aiCharacter.animator.SetFloat("Vertical", 0f, 0.2f, Time.deltaTime);
                return this;
            }

            //如果有巡逻点，计算距离，开启navimesh
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

            //如果没有巡逻点，索引+1，巡逻完成，巡逻点设置为列表内下一个
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
            //在范围内寻找目标
            //如果找到目标，切换到PursueTargetState
            //如果没有目标返回该状态
            Collider[] colliders = Physics.OverlapSphere(transform.position, aiCharacter.detectionRadius, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter != null)
                {
                    //判定teamID
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
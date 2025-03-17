using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class IdleStateHumanoid : State
    {
        //待机，寻找目标状态
        public PursueTargetStateHumanoid pursueTargetStateHumanoid;

        public LayerMask detectionLayer;        //可以探测到的遮罩
        public LayerMask layersThatBlockLineOfSight;    //遮挡视线的遮罩

        private void Awake()
        {
            pursueTargetStateHumanoid = GetComponent<PursueTargetStateHumanoid>();
        }

        public override State Tick(EnemyManager aiCharacter)
        {
            //在范围内寻找目标
            //如果找到目标，切换到PursueTargetState
            //如果没有目标返回该状态
            Collider[] colliders = Physics.OverlapSphere(transform.position, aiCharacter.detectionRadius, detectionLayer);      //探测半径为20球形区域内所有碰撞体

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
            if (aiCharacter.currentTarget != null && !aiCharacter.isDead)   //存在目标且没有死亡
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
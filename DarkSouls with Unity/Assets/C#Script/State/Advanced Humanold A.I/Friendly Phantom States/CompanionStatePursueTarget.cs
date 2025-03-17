using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CompanionStatePursueTarget : State
    {
        //追踪状态
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
            //Angle是0 —— 180°，SignedAngle是-180° —— 180°

            HandleRotateTowardsTarget(aiCharacter);

            //If we are preforming an action, stop our movement 如果敌人在做其他动画，停止移动和导航
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

            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)   //没到与玩家的停止距离，继续走，到了停止
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
            //Angle是0 —— 180°，SignedAngle是-180° —— 180°

            HandleRotateTowardsTarget(aiCharacter);

            //If we are preforming an action, stop our movement 如果敌人在做其他动画，停止移动和导航
            if (aiCharacter.isInteracting)
                return this;

            if (aiCharacter.isPreformingAction)
            {
                aiCharacter.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (aiCharacter.distanceFromTarget > aiCharacter.maximumAggroRadius)   //没到与玩家的停止距离，继续走，到了停止
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
                //↓切换成了评论区的旋转方式
                //启用navMeshAgent并设置敌人前往的目标位置
                aiCharacter.navMeshAgent.enabled = true;
                aiCharacter.navMeshAgent.SetDestination(aiCharacter.currentTarget.transform.position);

                //计算需要旋转的角度 和 目标距离
                float rotationToApplyToDynamicEnemy =
                    Quaternion.Angle(aiCharacter.transform.rotation, Quaternion.LookRotation(aiCharacter.navMeshAgent.desiredVelocity.normalized));
                float distanceFromTarget = Vector3.Distance(aiCharacter.currentTarget.transform.position, aiCharacter.transform.position);

                //根据距离和需要旋转的角度调整角速度
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

                //计算距离，目标位置-敌人自身位置
                Vector3 targetDirection = aiCharacter.currentTarget.transform.position - aiCharacter.transform.position;
                //通过距离方位计算看向敌人的旋转角度
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
                //navMeshAgent的期望速度>0→敌人正在导航移动，取消导航，旋转归一后导航
                if (aiCharacter.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    aiCharacter.navMeshAgent.updateRotation = false;
                    aiCharacter.transform.rotation = Quaternion.RotateTowards(aiCharacter.transform.rotation,
                        Quaternion.LookRotation(aiCharacter.navMeshAgent.desiredVelocity.normalized), aiCharacter.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                //navMeshAgent的期望速度<=0→敌人静止，使用上面的计算角度旋转
                else
                {
                    aiCharacter.transform.rotation = Quaternion.RotateTowards(aiCharacter.transform.rotation,
                        rotationToApplyToStaticEnemy, aiCharacter.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }
}
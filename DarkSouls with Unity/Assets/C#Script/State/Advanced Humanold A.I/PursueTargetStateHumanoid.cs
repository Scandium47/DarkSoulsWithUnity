using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PursueTargetStateHumanoid : State
    {
        //追踪状态
        public CombatStanceStateHumanoid combatStanceStateHumanoid;

        private void Awake()
        {
            combatStanceStateHumanoid = GetComponent<CombatStanceStateHumanoid>();
        }

        public override State Tick(EnemyManager enemy)
        {
            if(enemy.combatStyle == AICombatStyle.swordAndShield)
            {
                return ProcessSwordAndShieldPursueTargetStyle(enemy);
            }
            else if(enemy.combatStyle == AICombatStyle.archer)
            {
                return ProcessArcherPursueTargetStyle(enemy);
            }
            else
            {
                return this;
            }
        }

        private State ProcessArcherPursueTargetStyle(EnemyManager enemy)
        {
            //Angle是0 —— 180°，SignedAngle是-180° —— 180°

            HandleRotateTowardsTarget(enemy);

            //If we are preforming an action, stop our movement 如果敌人在做其他动画，停止移动和导航
            if (enemy.isInteracting)
            {
                enemy.animator.SetFloat("Vertical", 0);
                return this;
            }

            if (enemy.isPreformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)   //没到与玩家的停止距离，继续走，到了停止
            {
                if(!enemy.isStationaryArcher)
                {
                    enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceStateHumanoid;
            }
            else
            {
                return this;
            }
        }

        private State ProcessSwordAndShieldPursueTargetStyle(EnemyManager enemy)
        {
            //Angle是0 —— 180°，SignedAngle是-180° —— 180°

            HandleRotateTowardsTarget(enemy);

            //If we are preforming an action, stop our movement 如果敌人在做其他动画，停止移动和导航
            if (enemy.isInteracting)
                return this;

            if (enemy.isPreformingAction)
            {
                enemy.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)   //没到与玩家的停止距离1.5f，继续走，到了停止
            {
                enemy.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }

            if (enemy.distanceFromTarget <= enemy.maximumAggroRadius)
            {
                return combatStanceStateHumanoid;
            }
            else
            {
                return this;
            }
        }

        private void HandleRotateTowardsTarget(EnemyManager enemyManager)
        {
            //手动旋转（当敌人正在执行动作时）
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
            //navimesh旋转（当敌人未执行动作时）
            else
            {
                //启用navMeshAgent并设置敌人前往的目标位置
                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);

                //计算需要旋转的角度 和 目标距离
                float rotationToApplyToDynamicEnemy =
                    Quaternion.Angle(enemyManager.transform.rotation, Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized));
                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

                //根据距离和需要旋转的角度调整角速度
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

                //计算距离，目标位置-敌人自身位置
                Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                //通过距离方位计算看向敌人的旋转角度
                Quaternion rotationToApplyToStaticEnemy = Quaternion.LookRotation(targetDirection);
                //navMeshAgent的期望速度>0→敌人正在导航移动，取消导航，旋转归一后导航
                if (enemyManager.navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    enemyManager.navMeshAgent.updateRotation = false;
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
                        Quaternion.LookRotation(enemyManager.navMeshAgent.desiredVelocity.normalized), enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
                //navMeshAgent的期望速度<=0→敌人静止，使用上面的计算角度旋转
                else
                {
                    enemyManager.transform.rotation = Quaternion.RotateTowards(enemyManager.transform.rotation,
                        rotationToApplyToStaticEnemy, enemyManager.navMeshAgent.angularSpeed * Time.deltaTime);
                }
            }
        }
    }
}
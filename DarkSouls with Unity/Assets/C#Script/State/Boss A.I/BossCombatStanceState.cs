using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class BossCombatStanceState : CombatStanceState
    {
        //BossÕ½¶·×´Ì¬
        [Header("Second Phase Attacks")]
        public bool hasPhaseShifted;
        public EnemyAttackAction[] secondPhaseEnemyAttacks;

        protected override void GetNewAttack(EnemyManager enemyManager)
        {
            if(hasPhaseShifted)
            {
                Vector3 targetsDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
                float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

                int maxScore = 0;

                for (int i = 0; i < secondPhaseEnemyAttacks.Length; i++)
                {
                    EnemyAttackAction enemyAttackAction = secondPhaseEnemyAttacks[i];

                    if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                    {
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                            && viewableAngle >= enemyAttackAction.attackScore)
                        {
                            maxScore += enemyAttackAction.attackScore;
                        }
                    }
                }

                int randomValue = Random.Range(0, maxScore);
                int temporaryScore = 0;

                for (int i = 0; i < secondPhaseEnemyAttacks.Length; i++)
                {
                    EnemyAttackAction enemyAttackAction = secondPhaseEnemyAttacks[i];

                    if (distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                        && distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                    {
                        if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                            && viewableAngle >= enemyAttackAction.attackScore)
                        {
                            if (attackState.currentAttack != null)
                                return;

                            temporaryScore += enemyAttackAction.attackScore;

                            if (temporaryScore > randomValue)
                            {
                                attackState.currentAttack = enemyAttackAction;
                            }
                        }
                    }
                }
            }
            else
            {
                base.GetNewAttack(enemyManager); 
            }
        }
    }
}
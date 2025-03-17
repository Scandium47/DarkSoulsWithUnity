using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace SG
{
    public class EnemyManager : CharacterManager
    {
        public EnemyBossManager enemyBossManager;
        public EnemyLocomotionManager enemyLocomotionManager;
        public EnemyAnimatorManager enemyAnimatorManager;
        public EnemyStatsManager enemyStatsManager;
        public EnemyEffectsManager enemyEffectsManager;

        public State currentState;
        public CharacterManager currentTarget;
        public NavMeshAgent navMeshAgent;
        public Rigidbody enemyRigidBody;

        public bool isPreformingAction;
        //public float distanceFromTarget;
        public float rotationSpeed = 30;
        public float maximumAggroRadius = 1.5f;

        [Header("A.I Settings")]
        public float detectionRadius = 20;
        //角度最高和最低的探测视野范围
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;
        public float stoppingDistance = 2f;       //与玩家之间的停止距离

        //这些设定只设置在Humanoid States，角色A.I状态池
        [Header("Advanced A.I Settings")]
        public bool allowAIToPerformBlock;
        public int blockLikelyHood = 50;        //0-100%
        public bool allowAIToPerformDodge;
        public int dodgeLikelyHood = 50;
        public bool allowAIToPerformParry;
        public int parryLikelyHood = 50;

        [Header("A.I Combat Settings")]
        public bool allowAIToPerformCombos;
        public bool isPhaseShifting;
        public float comboLikelyHood;
        public AICombatStyle combatStyle;

        [Header("A.I Archery Settings")]
        public bool isStationaryArcher;
        public float minimumTimeToAimAtTarget = 2f;
        public float maximumTimeToAimTarget = 4f;

        //白灵设定
        [Header("A.I Companion Settings")]
        public float maxDistanceFromCompanion;      //可以离开玩家的最大距离
        public float minDistanceFromCompanion;      //与玩家的最小距离，不妨碍玩家行动
        public float retuanDistanceFromCompanion = 2;   //当离开玩家最大距离后，回到玩家附近，此时与玩家的距离
        public float distanceFromCompanion;     
        public CharacterManager companion;

        [Header("A.I Target Information")]
        public float distanceFromTarget;
        public Vector3 targetsDirection;
        public float viewableAngle;

        protected override void Awake()
        {
            base.Awake();
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyBossManager = GetComponent<EnemyBossManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyStatsManager = GetComponent<EnemyStatsManager>();
            enemyEffectsManager = GetComponent<EnemyEffectsManager>();
            enemyRigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            navMeshAgent.enabled = false;
        }
        protected override void Start()
        {
            base.Start();
            enemyRigidBody.isKinematic = false;
        }

        protected override void Update()
        {
            base.Update();
            HandleRecoveryTimer();
            HnadleStateMachine();

            isRotatingWithRootMotion = animator.GetBool("isRotatingWithRootMotion");
            isInteracting = animator.GetBool("isInteracting");
            isPhaseShifting = animator.GetBool("isPhaseShifting");
            isInvulnerable = animator.GetBool("isInvulnerable");
            isHoldingArrow = animator.GetBool("isHoldingArrow");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            animator.SetBool("isDead", isDead);
            animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
            animator.SetBool("isBlocking", isBlocking);

            if(currentTarget != null)
            {
                distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
                targetsDirection = currentTarget.transform.position - transform.position;
                viewableAngle = Vector3.Angle(targetsDirection, transform.forward);

            }

            if(companion != null)
            {
                distanceFromCompanion = Vector3.Distance(companion.transform.position, transform.position);
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void LateUpdate()
        {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        //将HandleCurrentAction修改为HandleStateMachine
        private void HnadleStateMachine()
        {
            if (isDead)
            {
                return;
            }
            if(currentState !=null)
            {
                State nextState = currentState.Tick(this);

                if(nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }

            #region 不用的注释，判断敌人与玩家距离
            //if(enemyLocomotionManager.currentTarget != null)
            //{
            //    enemyLocomotionManager.distanceFromTarget =
            //        Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
            //}

            //if (enemyLocomotionManager.currentTarget == null)
            //{
            //    enemyLocomotionManager.HandleDetection();
            //}
            //else if(enemyLocomotionManager.distanceFromTarget >= enemyLocomotionManager.stoppingDistance)
            //{
            //    enemyLocomotionManager.HandleMoveToTarget();
            //}
            //else if(enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
            //{
            //    AttackTarget();
            //}
            #endregion
        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        private void HandleRecoveryTimer()
        {
            if(currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if(isPreformingAction)
            {
                if(currentRecoveryTime <=0)
                {
                    isPreformingAction = false;
                }
            }
        }

        #region 敌人视野可视化
        private void OnDrawGizmosSelected()
        {
            //蓝色双边缘线探测
            Vector3 fovLine1 = Quaternion.AngleAxis(maximumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(minimumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
            //红色球体探测
            //Gizmos.color = Color.red; //replace red with whatever color you prefer
            //Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
        #endregion
    }
}
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
        //�Ƕ���ߺ���͵�̽����Ұ��Χ
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        public float currentRecoveryTime = 0;
        public float stoppingDistance = 2f;       //�����֮���ֹͣ����

        //��Щ�趨ֻ������Humanoid States����ɫA.I״̬��
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

        //�����趨
        [Header("A.I Companion Settings")]
        public float maxDistanceFromCompanion;      //�����뿪��ҵ�������
        public float minDistanceFromCompanion;      //����ҵ���С���룬����������ж�
        public float retuanDistanceFromCompanion = 2;   //���뿪���������󣬻ص���Ҹ�������ʱ����ҵľ���
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

        //��HandleCurrentAction�޸�ΪHandleStateMachine
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

            #region ���õ�ע�ͣ��жϵ�������Ҿ���
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

        #region ������Ұ���ӻ�
        private void OnDrawGizmosSelected()
        {
            //��ɫ˫��Ե��̽��
            Vector3 fovLine1 = Quaternion.AngleAxis(maximumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(minimumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
            //��ɫ����̽��
            //Gizmos.color = Color.red; //replace red with whatever color you prefer
            //Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
        #endregion
    }
}
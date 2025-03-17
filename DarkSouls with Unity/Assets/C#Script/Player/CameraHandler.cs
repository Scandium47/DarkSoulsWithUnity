using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;

        public Transform targetTransform;                               //�������Ŀ�꣨��ң�
        public Transform targetTransformWhileAiming;          //��׼ʱ���������Ŀ��
        public Transform cameraTransform;       //�����λ��
        public Camera cameraObject;
        public Transform cameraPivotTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float leftAndRightLookSpeed = 250f;
        public float leftAndRightAimingLookSpeed = 25f;
        public float followSpeed = 1f;
        public float aerialFollowSpeed = 1f;
        public float upAndDownLookSpeed = 250f;
        public float upAndDownAimingLookSpeed = 25f;

        private float targetPosition;
        private float defaultPosition;      //�������������Ĭ�Ͼ���
        private float leftAndRightAngle;
        private float upAndDownAngle;
        public float minimumLookDownAngle = -35;
        public float maximumLookUpAngle = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffSet = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        public CharacterManager currentLockOnTarget;

        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;
        public float maximumLockOnDistance = 30;

        private void Awake()
        {
            singleton = this;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 9 | 1 << 10 | 1 << 11 | 1 << 12 | 1 << 13);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
            cameraObject = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        //�������
        public void FollowTarget()
        {
            if(playerManager.isAiming)
            {
                Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransformWhileAiming.position, ref cameraFollowVelocity, Time.deltaTime / followSpeed);
                transform.position = targetPosition;
            }
            else
            {
                //�����������/�������ص����� Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
                if (playerManager.isGrounded)
                {
                    Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, Time.deltaTime / followSpeed);
                    transform.position = targetPosition;
                }
                else
                {
                    Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, Time.deltaTime / aerialFollowSpeed);
                    transform.position = targetPosition;
                }
            }

            HandleCameraCollisions();
        }

        //��ת���
        public void HandleCameraRotation()
        {
            if(inputHandler.lockOnFlag && currentLockOnTarget != null)
            {
                HandleLockedCameraRotation();
            }
            else if(playerManager.isAiming)
            {
                HandleAimedCameraRotation();
            }
            else
            {
                HandleStandardCameraRotation();
            }
        }

        public void HandleStandardCameraRotation()
        {
            //lookAngle += (mouseXInput * lookSpeed) / delta;
            //pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            //fixed ��
            leftAndRightAngle += inputHandler.mouseX * leftAndRightLookSpeed * Time.deltaTime;                       //���X�ᣬ���ҷ�����Y���������������ͷҪ˳ʱ��ת��������ӱ���������+=
            upAndDownAngle -= inputHandler.mouseY * upAndDownLookSpeed * Time.deltaTime;                     //���Y�ᣬ���·�����X���������������ͷҪ��ʱ��ת��������-=
            upAndDownAngle = Mathf.Clamp(upAndDownAngle, minimumLookDownAngle, maximumLookUpAngle);       //���������ӽ������Сֵ��������������Сֵ���Ǿ�ȡ�����Сֵ

            //ת������Ԫ��
            Vector3 rotation = Vector3.zero;
            rotation.y = leftAndRightAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = upAndDownAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleLockedCameraRotation()
        {
            Vector3 dir = currentLockOnTarget.lockOnTransform.position - transform.position;    //�����Ŀ������λ�õ���������һ����y=0ȷ��ֻˮƽ��ת
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;        //�����dir�ķ�����ת

            dir = currentLockOnTarget.lockOnTransform.position - cameraPivotTransform.position;     //������ᵽĿ������λ�õ���������һ��
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivotTransform.localEulerAngles = eulerAngle;
        }

        private void HandleAimedCameraRotation()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            cameraPivotTransform.rotation = Quaternion.Euler(0, 0, 0);

            Quaternion targetRotationX;
            Quaternion targetRotationY;

            Vector3 cameraRotationX = Vector3.zero;
            Vector3 cameraRotationY = Vector3.zero;

            leftAndRightAngle += (inputHandler.mouseX * leftAndRightAimingLookSpeed) * Time.deltaTime;
            upAndDownAngle -= (inputHandler.mouseY * upAndDownAimingLookSpeed) * Time.deltaTime;

            cameraRotationY.y = leftAndRightAngle;
            targetRotationY = Quaternion.Euler(cameraRotationY);
            targetRotationY = Quaternion.Slerp(transform.rotation,  targetRotationY,  1);
            transform.localRotation = targetRotationY;

            cameraRotationX.x = upAndDownAngle;
            targetRotationX = Quaternion.Euler(cameraRotationX); 
            targetRotationX = Quaternion.Slerp(cameraTransform.localRotation, targetRotationX,  1);
            cameraTransform.localRotation = targetRotationX;
        }

        public void ResetAimCameraRotations()
        {
            cameraTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //��ײ��⣨������������������ײ����������壬ʼ�ո����ɫ��
        private void HandleCameraCollisions()
        {
            targetPosition = defaultPosition;       // defaultPosition = cameraTransform.localPosition.z; �������λ�õ�Zֵ
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;       //������� �� ��� ����������һ��
            direction.Normalize();

            if(Physics.SphereCast       //����������� ������� �� ��� �ķ����԰뾶Ϊ0.2f��������������򷢳����ߣ���������߳���Ϊ ������� �� ��� ��Ĭ�Ͼ��룬ͬʱ���Բ��ᱻ����ͼ��
                (cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition)   
                , ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);     //���� ������� �� ������ײ�� �ľ���
                targetPosition = -(dis - cameraCollisionOffSet);        //����0.2f ����ֱ�ӽӴ���ײ����
            }

            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)  //����������̫���ˣ�С����С���루0.2f��������Ϊ0.2f
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, Time.deltaTime / 0.2f);  //��Z��ƽ������ targetPosition �ľ��룬Ҳ���ǽӴ�����ײ��ʱ���ᴩ����ײ��
            cameraTransform.localPosition = cameraTransformPosition;

        }

        //����
        public void HandleLockOn()
        {
            availableTargets.Clear();

            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);     //�԰뾶Ϊ2�����η�Χ�����Χ������ײ��

            if (colliders.Length > 0)
            {
                //����Χ�����пɼ�����ӵ��б���
                for (int i = 0; i < colliders.Length; i++)
                {
                    CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                    if (character != null && !character.isDead)     //̽�⵽�Ķ��������û������
                    {
                        Vector3 lockTargetDirection = character.transform.position - targetTransform.position;      //��ҵ����������
                        float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);        //��ҵ�����ľ���
                        float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);          //�����ǰ����Ŀ��ĽǶ�
                        RaycastHit hit;

                        if (character.transform.root != targetTransform.transform.root      //��������ұ���Ҳ�����Ƕ��ѣ��Ƕȴ���50С��-50������С�������� 30
                            && character.characterStatsManager.teamIDNumber != playerManager.characterStatsManager.teamIDNumber
                            && viewableAngle > -50 && viewableAngle < 50
                            && distanceFromTarget <= maximumLockOnDistance)
                        {
                            if(Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))       //����ҵ�Ŀ��λ�÷�������
                            {
                                Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                                if(hit.transform.gameObject.layer == environmentLayer)
                                {
                                    //����л����ڵ����޷������������뵽�ɼ�Ŀ����
                                }
                                else
                                {
                                    availableTargets.Add(character);
                                }
                            }
                        }
                    }
                }

                if (availableTargets.Count > 0)
                {
                    for (int k = 0; k < availableTargets.Count; k++)
                    {
                        //�����ҵ������Ŀ��nearestLockOnTarget
                        float distanceFormTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                        if (distanceFormTarget < shortestDistance)
                        {
                            shortestDistance = distanceFormTarget;
                            nearestLockOnTarget = availableTargets[k];      //shortestDistanceÿ�ε��ö��ᱻ���ó�����������ҵ������Ŀ��
                        }

                        //��������������ҵ����Ҳ������Ŀ��
                        if (inputHandler.lockOnFlag)
                        {
                            /*
                            Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                            var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                            var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;
                            */
                            Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);       //����ǰĿ�����������ת��Ϊ�������ҵľֲ�����
                            var distanceFromLeftTarget = relativeEnemyPosition.x;       //�����ж�Ŀ���ڵ�ǰ����Ŀ������Ҳ�
                            var distanceFromRightTarget = relativeEnemyPosition.x;

                            //shortestDistanceOfLeftTargetÿ�ε��ö��ᱻ���óɸ���������ҵ��������Ŀ��
                            if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget 
                                && availableTargets[k] != currentLockOnTarget)
                            {
                                shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                                leftLockTarget = availableTargets[k];
                            }
                            //shortestDistanceOfRightTargetÿ�ε��ö��ᱻ���ó�����������ҵ�����Ҳ��Ŀ��
                            else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget
                                && availableTargets[k] != currentLockOnTarget)
                            {
                                shortestDistanceOfRightTarget = distanceFromRightTarget;
                                rightLockTarget = availableTargets[k];
                            }
                        }
                    }

                    //�����ǰû��currentLockOnTarget������������������ж���ͻᵼ��currentLockOnTarget=null)������ʱ�����Ŀ��nearestLockOnTarget���ڲ�������Ĵ����ҵ�
                    if (currentLockOnTarget == null && nearestLockOnTarget != null)
                    {
                        // ��ǰ������Ŀ�� �� �Զ��������Ŀ�꣨ԭ���ڰ�����������ʱ��ִ��currentLockOnTarget = nearestLockOnTarget;��
                        currentLockOnTarget = nearestLockOnTarget;
                    }
                }
            }

           
        }

        //��鵱ǰ����Ŀ���Ƿ�����
        public void CheckForDeadTarget()
        {
            // ��鵱ǰ����Ŀ���Ƿ�����
            if (currentLockOnTarget != null && currentLockOnTarget.isDead)
            {
                ClearLockOnTargets();   //�����ǰĿ����������������ж���
            }

            // ����availableTargets�е�����Ŀ��
            CleanupDeadTargets();

            if (nearestLockOnTarget == null && inputHandler.lockOnFlag)
            {
                //���²������η�Χ�ڵĵ��ˣ�������Ҵ�������ĵ��ˣ���Ŀ���л�Ϊ����ĵ���
                HandleLockOn();
                //�����Χ��û�е����ˣ�ȡ������
                if(availableTargets.Count == 0)
                {
                    inputHandler.lockOnFlag = false;
                }
            }
        }

        private void CleanupDeadTargets()
        {
            // ���� availableTargets
            for (int i = availableTargets.Count - 1; i >= 0; i--)
            {
                CharacterManager target = availableTargets[i];
                if (target == null || target.isDead)
                {
                    availableTargets.RemoveAt(i);
                }
            }

            // ��������Ŀ������
            if (leftLockTarget != null && leftLockTarget.isDead) leftLockTarget = null;
            if (rightLockTarget != null && rightLockTarget.isDead) rightLockTarget = null;

            // ȷ�� nearestLockOnTarget ��Ч
            if (nearestLockOnTarget != null && nearestLockOnTarget.isDead)
            {
                nearestLockOnTarget = null;
            }
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
            leftLockTarget = null;
            rightLockTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if(currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = 
                    Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = 
                    Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}
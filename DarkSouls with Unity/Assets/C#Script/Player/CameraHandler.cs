using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;

        public Transform targetTransform;                               //相机跟随目标（玩家）
        public Transform targetTransformWhileAiming;          //瞄准时的相机跟随目标
        public Transform cameraTransform;       //主相机位置
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
        private float defaultPosition;      //相机与相机枢轴的默认距离
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

        //跟随玩家
        public void FollowTarget()
        {
            if(playerManager.isAiming)
            {
                Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransformWhileAiming.position, ref cameraFollowVelocity, Time.deltaTime / followSpeed);
                transform.position = targetPosition;
            }
            else
            {
                //更改摄像机震动/抖动严重的问题 Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
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

        //旋转相机
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
            //fixed ↓
            leftAndRightAngle += inputHandler.mouseX * leftAndRightLookSpeed * Time.deltaTime;                       //鼠标X轴，左右方向，绕Y轴↑，往右移摄像头要顺时针转（俯瞰就是钟表），所以是+=
            upAndDownAngle -= inputHandler.mouseY * upAndDownLookSpeed * Time.deltaTime;                     //鼠标Y轴，上下方向，绕X轴→，往上移摄像头要逆时针转，所以是-=
            upAndDownAngle = Mathf.Clamp(upAndDownAngle, minimumLookDownAngle, maximumLookUpAngle);       //设置上下视角最大最小值，如果超出最大最小值，那就取最大最小值

            //转换成四元数
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
            Vector3 dir = currentLockOnTarget.lockOnTransform.position - transform.position;    //相机到目标锁定位置的向量，归一化，y=0确保只水平旋转
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;        //相机朝dir的方向旋转

            dir = currentLockOnTarget.lockOnTransform.position - cameraPivotTransform.position;     //相机枢轴到目标锁定位置的向量，归一化
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

        //碰撞检测（摄像机遇到物体产生碰撞不会进入物体，始终跟随角色）
        private void HandleCameraCollisions()
        {
            targetPosition = defaultPosition;       // defaultPosition = cameraTransform.localPosition.z; 相机本地位置的Z值
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;       //相机枢轴 到 相机 的向量，归一化
            direction.Normalize();

            if(Physics.SphereCast       //相机枢轴沿着 相机枢轴 到 相机 的方向，以半径为0.2f的球形向这个方向发出射线，且最大射线长度为 相机枢轴 到 相机 的默认距离，同时忽略不会被检测的图层
                (cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition)   
                , ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);     //计算 相机枢轴 到 射线碰撞点 的距离
                targetPosition = -(dis - cameraCollisionOffSet);        //拉回0.2f 避免直接接触碰撞物体
            }

            if(Mathf.Abs(targetPosition) < minimumCollisionOffset)  //如果这个距离太近了，小于最小距离（0.2f），设置为0.2f
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, Time.deltaTime / 0.2f);  //将Z轴平滑过渡 targetPosition 的距离，也就是接触到碰撞器时不会穿过碰撞器
            cameraTransform.localPosition = cameraTransformPosition;

        }

        //锁定
        public void HandleLockOn()
        {
            availableTargets.Clear();

            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);     //以半径为2的球形范围检测周围所有碰撞体

            if (colliders.Length > 0)
            {
                //将范围内所有可见对象加到列表内
                for (int i = 0; i < colliders.Length; i++)
                {
                    CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                    if (character != null && !character.isDead)     //探测到的对象存在且没有死亡
                    {
                        Vector3 lockTargetDirection = character.transform.position - targetTransform.position;      //玩家到对象的向量
                        float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);        //玩家到对象的距离
                        float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);          //相机正前方与目标的角度
                        RaycastHit hit;

                        if (character.transform.root != targetTransform.transform.root      //对象不是玩家本身，也不能是队友，角度大于50小于-50，距离小于最大距离 30
                            && character.characterStatsManager.teamIDNumber != playerManager.characterStatsManager.teamIDNumber
                            && viewableAngle > -50 && viewableAngle < 50
                            && distanceFromTarget <= maximumLockOnDistance)
                        {
                            if(Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))       //从玩家到目标位置发出射线
                            {
                                Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                                if(hit.transform.gameObject.layer == environmentLayer)
                                {
                                    //如果有环境遮挡，无法锁定，不加入到可见目标内
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
                        //遍历找到最近的目标nearestLockOnTarget
                        float distanceFormTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                        if (distanceFormTarget < shortestDistance)
                        {
                            shortestDistance = distanceFormTarget;
                            nearestLockOnTarget = availableTargets[k];      //shortestDistance每次调用都会被设置成正无穷，用来找到最近的目标
                        }

                        //如果正在锁定，找到左右侧最近的目标
                        if (inputHandler.lockOnFlag)
                        {
                            /*
                            Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                            var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                            var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;
                            */
                            Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);       //将当前目标的世界坐标转换为相对于玩家的局部坐标
                            var distanceFromLeftTarget = relativeEnemyPosition.x;       //用于判断目标在当前锁定目标的左右侧
                            var distanceFromRightTarget = relativeEnemyPosition.x;

                            //shortestDistanceOfLeftTarget每次调用都会被设置成负无穷，用来找到最近左侧的目标
                            if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget 
                                && availableTargets[k] != currentLockOnTarget)
                            {
                                shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                                leftLockTarget = availableTargets[k];
                            }
                            //shortestDistanceOfRightTarget每次调用都会被设置成正无穷，用来找到最近右侧的目标
                            else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget
                                && availableTargets[k] != currentLockOnTarget)
                            {
                                shortestDistanceOfRightTarget = distanceFromRightTarget;
                                rightLockTarget = availableTargets[k];
                            }
                        }
                    }

                    //如果当前没有currentLockOnTarget（敌人死亡后被清空所有对象就会导致currentLockOnTarget=null)，但此时最近的目标nearestLockOnTarget存在并被上面的代码找到
                    if (currentLockOnTarget == null && nearestLockOnTarget != null)
                    {
                        // 当前无锁定目标 → 自动锁定最近目标（原本在按下锁定键的时候执行currentLockOnTarget = nearestLockOnTarget;）
                        currentLockOnTarget = nearestLockOnTarget;
                    }
                }
            }

           
        }

        //检查当前锁定目标是否死亡
        public void CheckForDeadTarget()
        {
            // 检查当前锁定目标是否死亡
            if (currentLockOnTarget != null && currentLockOnTarget.isDead)
            {
                ClearLockOnTargets();   //如果当前目标死亡，先清空所有对象
            }

            // 清理availableTargets中的死亡目标
            CleanupDeadTargets();

            if (nearestLockOnTarget == null && inputHandler.lockOnFlag)
            {
                //重新查找球形范围内的敌人，如果有且存在最近的敌人，将目标切换为最近的敌人
                HandleLockOn();
                //如果范围内没有敌人了，取消锁定
                if(availableTargets.Count == 0)
                {
                    inputHandler.lockOnFlag = false;
                }
            }
        }

        private void CleanupDeadTargets()
        {
            // 清理 availableTargets
            for (int i = availableTargets.Count - 1; i >= 0; i--)
            {
                CharacterManager target = availableTargets[i];
                if (target == null || target.isDead)
                {
                    availableTargets.RemoveAt(i);
                }
            }

            // 清理左右目标引用
            if (leftLockTarget != null && leftLockTarget.isDead) leftLockTarget = null;
            if (rightLockTarget != null && rightLockTarget.isDead) rightLockTarget = null;

            // 确保 nearestLockOnTarget 有效
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
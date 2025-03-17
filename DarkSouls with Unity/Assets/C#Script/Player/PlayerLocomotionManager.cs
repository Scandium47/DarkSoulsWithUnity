using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager player;

        #region ��ײ�����߼��������÷��������ݽ�ɫ������ǰ�����¼���Ƿ��ڱ�Ե�������ʩ����������Ƴ�ȥ�����⿨�ڱ�Ե���׶ˣ����ǻῨ��
        //public new Rigidbody rigidbody;
        //[Header("Ground & Air Decetion Stats")]
        //[SerializeField]
        //float groundDetectionRayStartPoint = 0.5f;
        //[SerializeField]
        //float minimumDistanceNeededToBeginFall = 1f;
        //[SerializeField]
        //float groundDirectionRayDistance = -0.2f;
        //[SerializeField]
        //float fallingSpeed = 100;
        //public CapsuleCollider characterCollider;
        //public CapsuleCollider characterCollisionBlockerCollider;

        //Vector3 normalVector;
        //Vector3 targetPosition;

        //��start�
        //player.isGrounded = true;
        //Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);

        //��HandleMovement�У�
        //moveDirection = player.cameraHandler.cameraObject.transform.forward * player.inputHandler.vertical;
        //moveDirection += player.cameraHandler.cameraObject.transform.right * player.inputHandler.horizontal;
        //moveDirection.Normalize();
        //moveDirection.y = 0;

        //float speed = movementSpeed;

        //if (player.inputHandler.sprintFlag && player.inputHandler.moveAmount > 0.5f)
        //{
        //    speed = sprintSpeed;
        //    player.isSprinting = true;
        //    moveDirection *= speed;
        //    player.playerStatsManager.DeductStamina(sprintStaminaCost);
        //}
        //else
        //{
        //    if(player.inputHandler.moveAmount <= 0.5)
        //    {
        //        moveDirection *= walkingSpeed;
        //        player.isSprinting = false;
        //    }
        //    else
        //    {
        //        moveDirection *= speed;
        //        player.isSprinting = false;
        //    }
        //}

        //Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        //rigidbody.velocity = projectedVelocity;


        //public void HandleFalling(Vector3 moveDirection)
        //{
        //    player.isGrounded = false;
        //    RaycastHit hit;
        //    Vector3 origin = player.transform.position;
        //    origin.y += groundDetectionRayStartPoint;

        //    if(Physics.Raycast(origin, player.transform.forward, out hit, 0.4f))
        //    {
        //        moveDirection = Vector3.zero;
        //    }
        //    if(player.isInAir)
        //    {
        //        rigidbody.AddForce(-Vector3.up * fallingSpeed);
        //        rigidbody.AddForce(moveDirection * fallingSpeed / 10f);  //�ڱ�Ե������������ȥ����һ����΢���ƶ��������
        //    }

        //    Vector3 dir = moveDirection;
        //    dir.Normalize();
        //    origin = origin + dir * groundDirectionRayDistance;

        //    targetPosition = player.transform.position;

        //    Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        //    if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, GroundLayer))
        //    {
        //        normalVector = hit.normal;
        //        Vector3 tp = hit.point;
        //        player.isGrounded = true;
        //        targetPosition.y = tp.y;

        //        if(player.isInAir)
        //        {
        //            //if(inAirTimer > 0.1f)
        //            //{
        //                Debug.Log("You were in the air for " + inAirTimer);
        //                player.playerAnimatorManager.PlayTargetAnimation("Land", true);
        //                inAirTimer = 0;
        //            //}
        //            //else
        //            //{
        //            //    animatorHandler.PlayTargetAnimation("Empty", false);
        //            //    inAirTimer = 0;
        //            //}

        //            player.isInAir = false;
        //        }
        //    }
        //    else
        //    {
        //        if(player.isGrounded)
        //        {
        //            player.isGrounded = false;
        //        }

        //        if(player.isInAir == false)
        //        {
        //            if(player.isInteracting == false)
        //            {
        //                player.playerAnimatorManager.PlayTargetAnimation("Falling", true);
        //            }

        //            Vector3 vel = rigidbody.velocity; ;
        //            vel.Normalize();
        //            rigidbody.velocity = vel * (movementSpeed / 2);
        //            player.isInAir = true;
        //        }
        //    }

        //    if(player.isGrounded)
        //    {
        //        if(player.isInteracting || player.inputHandler.moveAmount > 0)
        //        {
        //            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, Time.deltaTime / 0.1f);
        //        }
        //        else
        //        {
        //            player.transform.position = targetPosition;
        //        }
        //    }
        //}
        #endregion

        [Header("Movement stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float walkingSpeed = 1;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;

        [Header("Stamina Costs")]
        [SerializeField]
        int rollStaminaCost = 30;
        int backstepStaminaCost = 24;
        int sprintStaminaCost = 3;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {

        }

        public void HandleRotation()
        {
            if (player.canRotate)
            {
                if(player.isAiming)
                {
                    Quaternion targetRotation = Quaternion.Euler(0, player.cameraHandler.cameraTransform.eulerAngles.y, 0);
                    Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = playerRotation;
                }
                else
                {
                    if (player.inputHandler.lockOnFlag)
                    {
                        if (player.isSprinting || player.inputHandler.rollFlag)
                        {
                            Vector3 targetDirection = Vector3.zero;
                            targetDirection = player.cameraHandler.cameraTransform.forward * player.inputHandler.vertical;
                            targetDirection += player.cameraHandler.cameraTransform.right * player.inputHandler.horizontal;
                            targetDirection.Normalize();
                            targetDirection.y = 0;

                            if (targetDirection == Vector3.zero)
                            {
                                targetDirection = transform.forward;
                            }

                            Quaternion tr = Quaternion.LookRotation(targetDirection);
                            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                            transform.rotation = targetRotation;
                        }
                        else
                        {
                            Vector3 rotationDirection = moveDirection;
                            rotationDirection = player.cameraHandler.currentLockOnTarget.transform.position - transform.position;
                            rotationDirection.y = 0;
                            rotationDirection.Normalize();
                            Quaternion tr = Quaternion.LookRotation(rotationDirection);
                            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                            transform.rotation = targetRotation;
                        }
                    }
                    else
                    {
                        Vector3 targetDir = Vector3.zero;
                        float moveOverride = player.inputHandler.moveAmount;

                        targetDir = player.cameraHandler.cameraObject.transform.forward * player.inputHandler.vertical;
                        targetDir += player.cameraHandler.cameraObject.transform.right * player.inputHandler.horizontal;

                        targetDir.Normalize();
                        targetDir.y = 0;

                        if (targetDir == Vector3.zero)
                            targetDir = player.transform.forward;
                        float rs = rotationSpeed;

                        Quaternion tr = Quaternion.LookRotation(targetDir);
                        Quaternion targetRotation = Quaternion.Slerp(player.transform.rotation, tr, rs * Time.deltaTime);

                        player.transform.rotation = targetRotation;
                    }
                }             
            }      
        }

        public void HandleGroundedMovement()
        {
            if (player.inputHandler.rollFlag)
                return;

            if (player.isInteracting)
                return;

            if (!player.isGrounded)
                return;

            moveDirection = player.cameraHandler.transform.forward * player.inputHandler.vertical;
            moveDirection = moveDirection + player.cameraHandler.transform.right * player.inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if(player.isSprinting && player.inputHandler.moveAmount > 0.5f)
            {
                speed = sprintSpeed;
                player.characterController.Move(moveDirection * speed * Time.deltaTime);
                player.playerStatsManager.DeductSprintingStamina(sprintStaminaCost);
            }
            else
            {
                if(player.inputHandler.moveAmount > 0.5f)
                {
                    player.characterController.Move(moveDirection * speed * Time.deltaTime);
                    player.isSprinting = false;
                }
                else if(player.inputHandler.moveAmount <= 0.5f)
                {
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                    player.isSprinting = false;
                }
            }

            if (player.inputHandler.lockOnFlag && player.isSprinting == false)
            {
                player.playerAnimatorManager.UpdateAnimatorValues(player.inputHandler.vertical, player.inputHandler.horizontal, player.isSprinting);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorValues(player.inputHandler.moveAmount, 0, player.isSprinting);
            }
        }

        public void HandleRollingAndSprinting()
        {
            //�����û��������û�о�return
            if (player.playerStatsManager.currentStamina <= 0)
                return;

            if(player.inputHandler.rollFlag)
            {
                player.inputHandler.rollFlag = false;

                if (!player.canRoll)
                    return;

                moveDirection = player.cameraHandler.cameraObject.transform.forward * player.inputHandler.vertical;
                moveDirection += player.cameraHandler.cameraObject.transform.right * player.inputHandler.horizontal;

                if (player.inputHandler.moveAmount > 0)
                {
                    //�ж����������������µķ������ƣ���ͬ����������
                    switch (player.playerStatsManager.encumbranceLevel)
                    {
                        case EncumbranceLevel.Light:
                            player.playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                            break;
                        case EncumbranceLevel.Medium:
                            player.playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                            break;
                        case EncumbranceLevel.Heavy:
                            player.playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                            break;
                        case EncumbranceLevel.Overloaded:
                            player.playerAnimatorManager.PlayTargetAnimation("Rolling", true);
                            break;
                        default:
                            break;
                    }
                    player.isInteracting = player.animator.GetBool("isInteracting");
                    //���˫��IK��Ȩ�ؽ�Ϊ��0��
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    player.transform.rotation = rollRotation;
                    player.playerStatsManager.DeductStamina(rollStaminaCost);
                }
                else
                {
                    player.playerAnimatorManager.PlayTargetAnimation("Backstep", true);
                    player.isInteracting = player.animator.GetBool("isInteracting");
                    //���˫��IK��Ȩ�ؽ�Ϊ��0��
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    player.playerStatsManager.DeductStamina(backstepStaminaCost);
                }
            }
        }

        public void HandleJumping()
        {
            if (player.isInteracting)
                return;

            if (player.playerStatsManager.currentStamina <= 0)
                return;

            if (player.inputHandler.jump_Input)
            {
                player.inputHandler.jump_Input = false;

                moveDirection = player.cameraHandler.cameraObject.transform.forward * player.inputHandler.vertical;
                moveDirection += player.cameraHandler.cameraObject.transform.right * player.inputHandler.horizontal;
                Debug.Log(moveDirection);

                if (player.inputHandler.moveAmount > 0)
                {
                    player.playerAnimatorManager.PlayTargetAnimation("Jump", true, true);
                    player.isInteracting = player.animator.GetBool("isInteracting");
                    //���˫��IK��Ȩ�ؽ�Ϊ��0��
                    player.playerAnimatorManager.EraseHandIKForWeapon();
                    moveDirection.y = 0;

                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    player.transform.rotation = jumpRotation;
                }
            }
        }
    }
}

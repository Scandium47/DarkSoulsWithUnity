using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        PlayerManager player;
        int vertical;
        int horizontal;

        private bool isIKEnabled;

        [SerializeField] public Transform leftHandTarget; // 左手目标，即拉杆的位置
        [SerializeField] public Transform rightHandTarget; // 右手目标，即拉杆的位置
        [SerializeField] private float leftHandIKPositionWeight = 1f;
        [SerializeField] private float leftHandIKRotationWeight = 1f;
        [SerializeField] private float rightHandIKPositionWeight = 1f;
        [SerializeField] private float rightHandIKRotationWeight = 1f;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0;
            if(verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if(verticalMovement >0.55f)
            {
                v = 1;
            }
            else if(verticalMovement <0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if(verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }

            #endregion

            if(isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            player.animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            player.animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);

        }

        public void EnableCollision()
        {
            foreach (var fogWall in player.worldEventManager.fogWalls)
            {
                fogWall.wall.enabled = false;
            }
        }

        public void DisableCollision()
        {
            foreach (var fogWall in player.worldEventManager.fogWalls)
            {
                fogWall.wall.enabled = true;
            }
        }

        public void EnableIK(Transform leftTarget, Transform rightTarget)
        {
            leftHandTarget = leftTarget;
            rightHandTarget = rightTarget;
            isIKEnabled = true;
        }

        public void DisableIK()
        {
            isIKEnabled = false;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (isIKEnabled && leftHandTarget != null && rightHandTarget != null)
            {
                // 设置左手的 IK 目标
                player.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKPositionWeight);
                player.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIKRotationWeight);
                player.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                player.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

                // 设置右手的 IK 目标
                player.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIKPositionWeight);
                player.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIKRotationWeight);
                player.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                player.animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
            else
            {
                // 禁用左手和右手的 IK
                player.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                player.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                player.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                player.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }

        public virtual void SucessfullyUseCurrentConsumable()
        {
            if (character.characterInventoryManager.currentConsumable != null)
            {
                character.characterInventoryManager.currentConsumable.SucessfullyConsumeItem(player);
            }
        }
    }
}
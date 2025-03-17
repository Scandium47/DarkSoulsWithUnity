using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class LeverInteractable : Interactable
    {
        [SerializeField] private ElevatorInteractable elevator;
        [SerializeField] private Animator leverAnimator;
        [SerializeField] private string leverPullAnimation = "Lever_Push_Back";
        [SerializeField] private Transform playerTargetPosition; // 玩家要移动到的目标位置
        [SerializeField] private float moveDuration = 0.5f; // 移动持续时间
        [SerializeField] public Transform leftHandTargetTransform;
        [SerializeField] public Transform rightHandTargetTransform;

        public override void Interact(PlayerManager playerManager)
        {
            interactableText = "Pull";
            //电梯不在低点，也没有正在移动，才可以互动
            if (!elevator.IsAtLowPosition() && !elevator.IsTravelling())
            {
                //协程移动玩家位置到拉杆
                StartCoroutine(MovePlayerToTargetPosition(playerManager));
            }
            else
            {
                Debug.Log("电梯不在合适位置或正在移动，无法拉动拉杆！");
            }
        }

        private void PullTheLever(PlayerManager playerManager)
        {
            PlayerAnimatorManager animator;
            animator = playerManager.GetComponentInChildren<PlayerAnimatorManager>();
            animator.PlayTargetAnimation("Pull_Lever", true);
            playerManager.playerAnimatorManager.EnableIK(leftHandTargetTransform, rightHandTargetTransform);
            leverAnimator.Play(leverPullAnimation);
        }

        private IEnumerator MovePlayerToTargetPosition(PlayerManager playerManager)
        {
            if (playerManager != null && playerTargetPosition != null)
            {
                Vector3 startPosition = playerManager.transform.position;
                Quaternion startRotation = playerManager.transform.rotation;
                float elapsedTime = 0f;

                while (elapsedTime < moveDuration)
                {
                    float t = elapsedTime / moveDuration;
                    playerManager.transform.position = Vector3.Lerp(startPosition, playerTargetPosition.position, t);
                    // 让玩家面向拉杆
                    Vector3 directionToLever = leverAnimator.transform.position - playerManager.transform.position;
                    directionToLever.y = 0; // 避免玩家在 Y 轴上旋转
                    if (directionToLever != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToLever);
                        playerManager.transform.rotation = Quaternion.Slerp(playerManager.transform.rotation, targetRotation, t);
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 确保最终位置和朝向准确
                playerManager.transform.position = playerTargetPosition.position;
                Vector3 finalDirectionToLever = leverAnimator.transform.position - playerManager.transform.position;
                finalDirectionToLever.y = 0;
                if (finalDirectionToLever != Vector3.zero)
                {
                    Quaternion finalTargetRotation = Quaternion.LookRotation(finalDirectionToLever);
                    playerManager.transform.rotation = finalTargetRotation;
                }

                // 播放拉杆动画
                PullTheLever(playerManager);

                // 等待 2.5 秒（拉杆后）
                yield return new WaitForSeconds(2.5f);

                // 启动电梯
                elevator.ActiveElevator();
                //恢复按压板以便于电梯降下后踩
                elevator.ReleaseButtonWhilePushTheLever();
            }
        }
    }
}
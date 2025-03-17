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
        [SerializeField] private Transform playerTargetPosition; // ���Ҫ�ƶ�����Ŀ��λ��
        [SerializeField] private float moveDuration = 0.5f; // �ƶ�����ʱ��
        [SerializeField] public Transform leftHandTargetTransform;
        [SerializeField] public Transform rightHandTargetTransform;

        public override void Interact(PlayerManager playerManager)
        {
            interactableText = "Pull";
            //���ݲ��ڵ͵㣬Ҳû�������ƶ����ſ��Ի���
            if (!elevator.IsAtLowPosition() && !elevator.IsTravelling())
            {
                //Э���ƶ����λ�õ�����
                StartCoroutine(MovePlayerToTargetPosition(playerManager));
            }
            else
            {
                Debug.Log("���ݲ��ں���λ�û������ƶ����޷��������ˣ�");
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
                    // �������������
                    Vector3 directionToLever = leverAnimator.transform.position - playerManager.transform.position;
                    directionToLever.y = 0; // ��������� Y ������ת
                    if (directionToLever != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(directionToLever);
                        playerManager.transform.rotation = Quaternion.Slerp(playerManager.transform.rotation, targetRotation, t);
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // ȷ������λ�úͳ���׼ȷ
                playerManager.transform.position = playerTargetPosition.position;
                Vector3 finalDirectionToLever = leverAnimator.transform.position - playerManager.transform.position;
                finalDirectionToLever.y = 0;
                if (finalDirectionToLever != Vector3.zero)
                {
                    Quaternion finalTargetRotation = Quaternion.LookRotation(finalDirectionToLever);
                    playerManager.transform.rotation = finalTargetRotation;
                }

                // �������˶���
                PullTheLever(playerManager);

                // �ȴ� 2.5 �루���˺�
                yield return new WaitForSeconds(2.5f);

                // ��������
                elevator.ActiveElevator();
                //�ָ���ѹ���Ա��ڵ��ݽ��º��
                elevator.ReleaseButtonWhilePushTheLever();
            }
        }
    }
}
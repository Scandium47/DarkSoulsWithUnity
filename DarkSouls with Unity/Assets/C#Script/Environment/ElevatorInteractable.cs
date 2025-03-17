using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ElevatorInteractable : Interactable
    {
        [Header("Character Tracking")]
        [SerializeField] private List<CharacterManager> charactersOnButton = new List<CharacterManager>();
        [SerializeField] private List<CharacterManager> charactersOnPlatform = new List<CharacterManager>();

        [Header("Interactable Collider")]
        [SerializeField] public Collider buttonTrigger;
        [SerializeField] public Collider platformTrigger;

        [Header("Destination")]
        [SerializeField] Vector3 destinationHigh;   //电梯最高点
        [SerializeField] Vector3 destinationLow;    //电梯最低点
        [SerializeField] bool isTravelling = false;
        [SerializeField] public bool buttonIsReleased = true;

        [Header("Animator")]
        [SerializeField] Animator elevatorAnimator;
        [SerializeField] string buttonPressAnimation = "Elevator_Button_Press_01";
        //[SerializeField] List<CharacterManager> charactersOnButton;     //如果要更多人进入电梯要有一个更大的触发器来统计
        //[SerializeField] List<CharacterManager> charactersOnPlatform;

        //玩家使用拉杆后释放按压板
        public void ReleaseButtonWhilePushTheLever()
        {
            StartCoroutine(ReleaseButton());
        }

        // 玩家进入按钮区域
        public void OnButtonTriggerEnter(CharacterManager character)
        {
            AddCharacterToList(charactersOnButton, character);
            if (!isTravelling && buttonIsReleased)
            {
                ActiveElevator();
            }
        }

        // 玩家离开按钮区域
        public void OnButtonTriggerExit(CharacterManager character)
        {
            RemoveCharacterFromList(charactersOnButton, character);
            StartCoroutine(ReleaseButton());
        }

        // 玩家进入平台区域
        public void OnPlatformTriggerEnter(CharacterManager character)
        {
            AddCharacterToList(charactersOnPlatform, character);
        }

        // 玩家离开平台区域
        public void OnPlatformTriggerExit(CharacterManager character)
        {
            RemoveCharacterFromList(charactersOnPlatform, character);
        }

        public void ActiveElevator()
        {
            elevatorAnimator.SetBool("isPressed", true);
            buttonIsReleased = false;
            elevatorAnimator.Play(buttonPressAnimation);

            if (transform.position == destinationHigh)
            {
                StartCoroutine(MoveElevator(destinationLow, 3));
            }
            else if (transform.position == destinationLow)
            {
                StartCoroutine(MoveElevator(destinationHigh, 3));
            }
        }

        private IEnumerator MoveElevator(Vector3 finalPosition, float duration)
        {
            isTravelling = true;

            float startTime = Time.time;
            Vector3 startPosition = transform.position;
            yield return null;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                Vector3 previousPosition = transform.position;
                transform.position = Vector3.Lerp(startPosition, finalPosition, t);
                // 计算电梯这一帧的位移
                Vector3 movementThisFrame = transform.position - previousPosition;
                Vector3 characterMovementVelocity = new Vector3(0, movementThisFrame.y, 0);

                // 检查角色是否还在电梯上
                for (int i = charactersOnPlatform.Count - 1; i >= 0; i--)
                {
                    CharacterManager character = charactersOnPlatform[i];
                    if (character != null && platformTrigger.bounds.Contains(character.transform.position))
                    {
                        character.characterController.Move(characterMovementVelocity);
                        Debug.Log(characterMovementVelocity);
                    }
                    else if (character != null)
                    {
                        // 角色不在电梯上，从列表中移除
                        RemoveCharacterFromList(charactersOnPlatform, character);
                    }
                }

                yield return null;
            }

            transform.position = finalPosition;
            isTravelling = false;
        }

        private IEnumerator ReleaseButton()
        {
            while (isTravelling)
                yield return null;

            yield return new WaitForSeconds(0.1f);     //压力板弹出时间

            if (charactersOnButton.Count == 0)
            {
                elevatorAnimator.SetBool("isPressed", false);
                buttonIsReleased = true;
            }
        }

        private void AddCharacterToList(List<CharacterManager> list, CharacterManager character)
        {
            if (!list.Contains(character))
            {
                list.Add(character);
            }
        }

        private void RemoveCharacterFromList(List<CharacterManager> list, CharacterManager character)
        {
            if (list.Contains(character))
            {
                list.Remove(character);
            }

            for (int i = list.Count - 1; i > -1; i--)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                }
            }
        }

        public bool IsAtLowPosition()
        {
            if (transform.position == destinationLow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsTravelling()
        {
            return isTravelling;
        }
    }
}
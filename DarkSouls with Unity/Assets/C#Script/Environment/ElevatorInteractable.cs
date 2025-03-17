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
        [SerializeField] Vector3 destinationHigh;   //������ߵ�
        [SerializeField] Vector3 destinationLow;    //������͵�
        [SerializeField] bool isTravelling = false;
        [SerializeField] public bool buttonIsReleased = true;

        [Header("Animator")]
        [SerializeField] Animator elevatorAnimator;
        [SerializeField] string buttonPressAnimation = "Elevator_Button_Press_01";
        //[SerializeField] List<CharacterManager> charactersOnButton;     //���Ҫ�����˽������Ҫ��һ������Ĵ�������ͳ��
        //[SerializeField] List<CharacterManager> charactersOnPlatform;

        //���ʹ�����˺��ͷŰ�ѹ��
        public void ReleaseButtonWhilePushTheLever()
        {
            StartCoroutine(ReleaseButton());
        }

        // ��ҽ��밴ť����
        public void OnButtonTriggerEnter(CharacterManager character)
        {
            AddCharacterToList(charactersOnButton, character);
            if (!isTravelling && buttonIsReleased)
            {
                ActiveElevator();
            }
        }

        // ����뿪��ť����
        public void OnButtonTriggerExit(CharacterManager character)
        {
            RemoveCharacterFromList(charactersOnButton, character);
            StartCoroutine(ReleaseButton());
        }

        // ��ҽ���ƽ̨����
        public void OnPlatformTriggerEnter(CharacterManager character)
        {
            AddCharacterToList(charactersOnPlatform, character);
        }

        // ����뿪ƽ̨����
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
                // ���������һ֡��λ��
                Vector3 movementThisFrame = transform.position - previousPosition;
                Vector3 characterMovementVelocity = new Vector3(0, movementThisFrame.y, 0);

                // ����ɫ�Ƿ��ڵ�����
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
                        // ��ɫ���ڵ����ϣ����б����Ƴ�
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

            yield return new WaitForSeconds(0.1f);     //ѹ���嵯��ʱ��

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
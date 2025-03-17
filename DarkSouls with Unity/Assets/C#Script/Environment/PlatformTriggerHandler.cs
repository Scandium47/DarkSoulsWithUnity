using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlatformTriggerHandler : MonoBehaviour
    {
        public ElevatorInteractable elevator;

        private void Awake()
        {
            elevator = GetComponentInParent<ElevatorInteractable>();
            if (elevator == null)
            {
                Debug.LogError("δ�ҵ��������ϵ� ElevatorInteractable �ű���");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // ֪ͨ�����壺��ҽ���ƽ̨����
                elevator.OnPlatformTriggerEnter(character);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // ֪ͨ�����壺����뿪ƽ̨����
                elevator.OnPlatformTriggerExit(character);
            }
        }
    }
}
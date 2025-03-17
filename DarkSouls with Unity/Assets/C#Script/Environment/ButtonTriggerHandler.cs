using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ButtonTriggerHandler : MonoBehaviour
    {
        // �������ϵĵ����߼��ű�
        public ElevatorInteractable elevator;

        private void Awake()
        {
            // ��ȡ������ĵ��ݽű�
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
                // ֪ͨ�����壺��ҽ��밴ť����
                elevator.OnButtonTriggerEnter(character);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // ֪ͨ�����壺����뿪��ť����
                elevator.OnButtonTriggerExit(character);
            }
        }
    }
}

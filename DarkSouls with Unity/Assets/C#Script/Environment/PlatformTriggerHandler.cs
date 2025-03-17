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
                Debug.LogError("未找到父物体上的 ElevatorInteractable 脚本！");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // 通知父物体：玩家进入平台区域
                elevator.OnPlatformTriggerEnter(character);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // 通知父物体：玩家离开平台区域
                elevator.OnPlatformTriggerExit(character);
            }
        }
    }
}
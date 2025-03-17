using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ButtonTriggerHandler : MonoBehaviour
    {
        // 父物体上的电梯逻辑脚本
        public ElevatorInteractable elevator;

        private void Awake()
        {
            // 获取父物体的电梯脚本
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
                // 通知父物体：玩家进入按钮区域
                elevator.OnButtonTriggerEnter(character);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CharacterManager character = other.GetComponent<CharacterManager>();
            if (character != null)
            {
                // 通知父物体：玩家离开按钮区域
                elevator.OnButtonTriggerExit(character);
            }
        }
    }
}

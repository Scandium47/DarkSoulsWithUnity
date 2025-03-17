using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class WeaponPickUP : Interactable
    {
        //独有的ID，每一个可拾取物品
        [Header("Item Information")]
        [SerializeField] public int itemPickUpId;
        [SerializeField] bool hasBeenLotted;

        [Header("Item")]
        public WeaponItem weapon;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            //如果存档里字典无法搜索到该物品ID，还没有被搜刮，将其ID键添加到字典并设置值为false
            if (!WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpId))
            {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpId, false);
            }

            hasBeenLotted = WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[itemPickUpId];

            if (hasBeenLotted)
            {
                gameObject.SetActive(false);
            }
        }

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            //记录物品已经被搜刮，不会再生成在区域内
            if (WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.ContainsKey(itemPickUpId))       //如果字典内物品ID键存在
            {
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Remove(itemPickUpId);       //先移除物品ID
            }

            WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld.Add(itemPickUpId, true);        //字典中此ID已被搜刮，物品ID键再次加入字典且值为true

            hasBeenLotted = true;

            //将物品放入玩家背包
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventoryManager playerInventory;
            PlayerLocomotionManager playerLocomotion;
            PlayerAnimatorManager animatorHandler;
            UIManager uiManager;

            playerInventory = playerManager.GetComponent<PlayerInventoryManager>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
            animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();
            uiManager = FindObjectOfType<UIManager>();

            playerLocomotion.GetComponent<Rigidbody>().velocity = Vector3.zero;     //捡东西时停止角色移动
            animatorHandler.PlayTargetAnimation("Pick Up Item", true);    //播放死尸掠夺动画
            playerInventory.weaponsInventory.Add(weapon);
            playerManager.itemInteractableGameObject.GetComponentInChildren<TextMeshProUGUI>().text = weapon.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
            playerManager.itemInteractableGameObject.SetActive(true);
            Destroy(gameObject);
            uiManager.UpdateUI();
        }
    }
}
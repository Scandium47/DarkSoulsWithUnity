using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class OpenChest : Interactable
    {
        Animator animator;
        OpenChest openChest;

        public Transform playerStandingPosition;
        public GameObject itemSpawner;
        public WeaponItem itemInChest;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            openChest = GetComponent<OpenChest>();
        }

        public override void Interact(PlayerManager playerManager)
        {
            //Rotaate our player towards the chest  让玩家转向宝箱
            Vector3 rotationDirection = transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            //Lock his transform to a certain point infront of the chest    将玩家锁定在宝箱前的位置
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            //open the chest lid, and animate the player    流畅的打开宝箱播放角色开箱动画
            playerManager.OpenChestInteraction(playerStandingPosition);
            animator.Play("Chest Open");

            //spawn an item inside the chest the player can pick up 在宝箱内放入物品，玩家可以拾取
            StartCoroutine(SpawnItemInChest());
            WeaponPickUP weaponPickUp = itemSpawner.GetComponent<WeaponPickUP>();

            if(weaponPickUp != null)
            {
                weaponPickUp.weapon = itemInChest;
            }
        }

        //IEnumerator迭代器 yield return 可以暂停并返回值，在合适时机（这里是等待一秒钟）继续在暂停处向下执行代码
        private IEnumerator SpawnItemInChest()
        {
            yield return new WaitForSeconds(1f);
            GameObject spawnedItem = Instantiate(itemSpawner, transform);
            WeaponPickUP weaponPickUp = spawnedItem.GetComponent<WeaponPickUP>();

            if(weaponPickUp != null)
            {
                // 分配动态生成的 ID
                weaponPickUp.itemPickUpId = WorldSaveGameManager.instance.currentCharacterSaveData.GetNextItemPickUpId();

                // 立即在存档中注册（未被拾取）
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[weaponPickUp.itemPickUpId] = false;
            }
            Destroy(openChest);
        }
    }
}
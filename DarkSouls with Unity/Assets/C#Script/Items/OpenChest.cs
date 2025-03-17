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
            //Rotaate our player towards the chest  �����ת����
            Vector3 rotationDirection = transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            //Lock his transform to a certain point infront of the chest    ����������ڱ���ǰ��λ��
            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 300 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            //open the chest lid, and animate the player    �����Ĵ򿪱��䲥�Ž�ɫ���䶯��
            playerManager.OpenChestInteraction(playerStandingPosition);
            animator.Play("Chest Open");

            //spawn an item inside the chest the player can pick up �ڱ����ڷ�����Ʒ����ҿ���ʰȡ
            StartCoroutine(SpawnItemInChest());
            WeaponPickUP weaponPickUp = itemSpawner.GetComponent<WeaponPickUP>();

            if(weaponPickUp != null)
            {
                weaponPickUp.weapon = itemInChest;
            }
        }

        //IEnumerator������ yield return ������ͣ������ֵ���ں���ʱ���������ǵȴ�һ���ӣ���������ͣ������ִ�д���
        private IEnumerator SpawnItemInChest()
        {
            yield return new WaitForSeconds(1f);
            GameObject spawnedItem = Instantiate(itemSpawner, transform);
            WeaponPickUP weaponPickUp = spawnedItem.GetComponent<WeaponPickUP>();

            if(weaponPickUp != null)
            {
                // ���䶯̬���ɵ� ID
                weaponPickUp.itemPickUpId = WorldSaveGameManager.instance.currentCharacterSaveData.GetNextItemPickUpId();

                // �����ڴ浵��ע�ᣨδ��ʰȡ��
                WorldSaveGameManager.instance.currentCharacterSaveData.itemsInWorld[weaponPickUp.itemPickUpId] = false;
            }
            Destroy(openChest);
        }
    }
}
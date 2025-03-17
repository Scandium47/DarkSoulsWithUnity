using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerWeaponSlotManager : CharacterWeaponSlotManager
    {
        PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public override void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (weaponItem != null)
            {
                if (isLeft)
                {
                    leftHandSlot.currentWeapon = weaponItem;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    player.uiManager.quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    //animator.CrossFade(weaponItem.left_hand_idle, 0.2f);  �������������� ��Ϊ��
                    //player.playerAnimatorManager.PlayTargetAnimation(weaponItem.offHandIdleAnimation, false, true);
                }
                else
                {
                    if (player.inputHandler.twoHandFlag)
                    {
                        //��װ������������ת�Ƶ�������
                        backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                        leftHandSlot.UnloadWeaponAndDestroy();
                        //animator.CrossFade(weaponItem.th_idle, 0.2f);  ��������������
                        player.playerAnimatorManager.PlayTargetAnimation("Left Arm Empty", false, true);
                    }
                    else
                    {
                        //animator.CrossFade("Both Arms Empty", 0.2f);  ������
                        backSlot.UnloadWeaponAndDestroy();
                        //animator.CrossFade(weaponItem.right_hand_idle, 0.2f);  ��������������
                    }

                    rightHandSlot.currentWeapon = weaponItem;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();    //�����������˺���ײ��
                    player.uiManager.quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    player.animator.runtimeAnimatorController = weaponItem.weaponController; //��������������������
                }
            }
            else
            {
                weaponItem = unarmedWeapon;

                if (isLeft)
                {
                    //animator.CrossFade("left Arm Empty", 0.2f);  ��������������
                    player.playerInventoryManager.leftWeapon = unarmedWeapon;
                    leftHandSlot.currentWeapon = unarmedWeapon;
                    leftHandSlot.LoadWeaponModel(weaponItem);
                    LoadLeftWeaponDamageCollider();
                    player.uiManager.quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                    //player.playerAnimatorManager.PlayTargetAnimation(weaponItem.offHandIdleAnimation, false, true);
                }
                else
                {
                    //animator.CrossFade("Right Arm Empty", 0.2f);  ��������������
                    player.playerInventoryManager.rightWeapon = unarmedWeapon;
                    rightHandSlot.currentWeapon = unarmedWeapon;
                    rightHandSlot.LoadWeaponModel(weaponItem);
                    LoadRightWeaponDamageCollider();
                    player.uiManager.quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
                    player.animator.runtimeAnimatorController = weaponItem.weaponController;
                }
            }
        }

        public void SucessfullyThrowFireBomb()
        {
            Destroy(player.playerEffectsManager.instantiatedFXModel);
            BombConsumeableItem fireBombItem = player.playerInventoryManager.currentConsumable as BombConsumeableItem;

            GameObject activeModelBomb = Instantiate(fireBombItem.liveBombModel, rightHandSlot.transform.position, player.cameraHandler.cameraPivotTransform.rotation);
            activeModelBomb.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
            //̽��ը����ײ��
            BombDamageCollider damageCollider = activeModelBomb.GetComponentInChildren<BombDamageCollider>();
            //��ը������˺�����
            damageCollider.explosionDamage = fireBombItem.baseDamage;
            damageCollider.explosionSplashDamage = fireBombItem.explosiveDamage;
            damageCollider.bombRigidBody.AddForce(activeModelBomb.transform.forward * fireBombItem.forwardVelocity);
            Debug.Log(activeModelBomb.transform.forward * fireBombItem.forwardVelocity);
            damageCollider.bombRigidBody.AddForce(activeModelBomb.transform.up * fireBombItem.upwardVelocity);
            Debug.Log(activeModelBomb.transform.up * fireBombItem.upwardVelocity);
            //�����ң�������������˺�
            damageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;
            LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);        //װ����������
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Fire Arrow Action")]
    public class FireArrowAction : ItemAction
    {
        public AmmoPoolManager arrowPool;    //对象池

        public override void PerformAction(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;

            //创建一个Live Arrow在实例化位置
            ArrowInstantiationLocation arrowInstantiationLocation;
            arrowInstantiationLocation = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<ArrowInstantiationLocation>();

            //播放射箭动画
            Animator bowAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnimator.SetBool("isDrawn", false);
            bowAnimator.Play("Bow_TH_Fire_01");

            //摧毁Loaded Arrow，和可能有的FX
            Destroy(character.characterEffectsManager.instantiatedFXModel);

            //Reset players holding arrow flag
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Fire_01", true);
            character.animator.SetBool("isHoldingArrow", false);

            //如果是玩家使用摄像头控制，如果是AI不使用摄像头
            if (player != null)
            {
                Debug.Log("玩家在用弓");
                //创建并发射Live Arrow
                GameObject liveArrow = AmmoPoolManager.Instance.GetPooledObject(
                        "Arrow", // 在Inspector中配置的tag
                        arrowInstantiationLocation.transform.position,
                        player.cameraHandler.cameraPivotTransform.rotation
                );
                //GameObject liveArrow = Instantiate(player.characterInventoryManager.currentAmmo.liveAmmoModel, arrowInstantiationLocation.transform.position, player.cameraHandler.cameraPivotTransform.rotation);

                // 如果对象池返回null（理论上不会发生，因为会自动扩展）
                if (liveArrow == null)
                {
                    Debug.LogError("Failed to get arrow from pool!");
                    return;
                }

                // 重置刚体状态
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = false;

                // 启用碰撞器
                CapsuleCollider collider = liveArrow.GetComponent<CapsuleCollider>();
                if (collider != null) collider.enabled = true;
                RangedProjectileDamageCollider damageCollider = liveArrow.GetComponent<RangedProjectileDamageCollider>();

                if (character.isAiming)
                {
                    Ray ray = player.cameraHandler.cameraObject.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                    RaycastHit hitPoint;

                    if (Physics.Raycast(ray, out hitPoint, 100.0f))
                    {
                        liveArrow.transform.LookAt(hitPoint.point);
                        Debug.Log(hitPoint.transform.name);
                    }
                    else
                    {
                        liveArrow.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraTransform.localEulerAngles.x, character.lockOnTransform.eulerAngles.y, 0);
                    }
                }
                else
                {
                    if (player.cameraHandler.currentLockOnTarget != null)
                    {
                        //锁定之后永远面向目标，可以复制朝向方向给箭
                        Quaternion arrowRotation = Quaternion.LookRotation(player.cameraHandler.currentLockOnTarget.lockOnTransform.position - liveArrow.gameObject.transform.position);
                        liveArrow.transform.rotation = arrowRotation;
                    }
                    else
                    {
                        liveArrow.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                    }
                }

                //给弓箭施加力，设置为箭的属性
                rigidbody.AddForce(liveArrow.transform.forward * player.playerInventoryManager.currentAmmo.forwardVelocity);
                rigidbody.AddForce(liveArrow.transform.up * player.playerInventoryManager.currentAmmo.upwardVelocity);
                rigidbody.useGravity = player.playerInventoryManager.currentAmmo.useGravity;
                rigidbody.mass = player.playerInventoryManager.currentAmmo.ammoMass;
                liveArrow.transform.parent = null;

                //设定Live Arrow伤害
                damageCollider.characterManager = character;
                damageCollider.ammoItem = player.playerInventoryManager.currentAmmo;
                damageCollider.physicalDamage = player.playerInventoryManager.currentAmmo.physicalDamage;
            }
            else
            {
                Debug.Log("敌人在用弓");
                EnemyManager enemy = character as EnemyManager;

                //创建并发射Live Arrow
                // 使用对象池获取箭矢
                GameObject liveArrow = AmmoPoolManager.Instance.GetPooledObject(
                    "Arrow_Enemy", // 使用敌人弓箭的tag
                    arrowInstantiationLocation.transform.position,
                    Quaternion.identity
                );

                // 对象池有效性检查
                if (liveArrow == null)
                {
                    Debug.LogError("敌人箭矢获取失败!");
                    return;
                }

                // 重置物理状态
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = false;

                // 启用碰撞器
                CapsuleCollider collider = liveArrow.GetComponent<CapsuleCollider>();
                if (collider != null) collider.enabled = true;
                //GameObject liveArrow = Instantiate(character.characterInventoryManager.currentAmmo.liveAmmoModel, arrowInstantiationLocation.transform.position, Quaternion.identity);

                RangedProjectileDamageCollider damageCollider = liveArrow.GetComponent<RangedProjectileDamageCollider>();

                if (enemy.currentTarget != null)
                {
                    //锁定之后永远面向目标，可以复制朝向方向给箭
                    Quaternion arrowRotation = Quaternion.LookRotation(enemy.currentTarget.lockOnTransform.position - liveArrow.gameObject.transform.position);
                    liveArrow.transform.rotation = arrowRotation;
                }
                else
                {
                    //liveArrow.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                }
                //给弓箭施加力，设置为箭的属性
                rigidbody.AddForce(liveArrow.transform.forward * enemy.characterInventoryManager.currentAmmo.forwardVelocity);
                rigidbody.AddForce(liveArrow.transform.up * enemy.characterInventoryManager.currentAmmo.upwardVelocity);
                rigidbody.useGravity = enemy.characterInventoryManager.currentAmmo.useGravity;
                rigidbody.mass = enemy.characterInventoryManager.currentAmmo.ammoMass;
                liveArrow.transform.parent = null;

                //设定Live Arrow伤害
                damageCollider.characterManager = character;
                damageCollider.ammoItem = enemy.characterInventoryManager.currentAmmo;
                damageCollider.physicalDamage = enemy.characterInventoryManager.currentAmmo.physicalDamage;
                damageCollider.teamIDNumber = enemy.characterStatsManager.teamIDNumber;
            }
        }
    }
}
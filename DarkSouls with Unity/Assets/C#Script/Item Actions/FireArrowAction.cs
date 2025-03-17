using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Fire Arrow Action")]
    public class FireArrowAction : ItemAction
    {
        public AmmoPoolManager arrowPool;    //�����

        public override void PerformAction(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;

            //����һ��Live Arrow��ʵ����λ��
            ArrowInstantiationLocation arrowInstantiationLocation;
            arrowInstantiationLocation = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<ArrowInstantiationLocation>();

            //�����������
            Animator bowAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnimator.SetBool("isDrawn", false);
            bowAnimator.Play("Bow_TH_Fire_01");

            //�ݻ�Loaded Arrow���Ϳ����е�FX
            Destroy(character.characterEffectsManager.instantiatedFXModel);

            //Reset players holding arrow flag
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Fire_01", true);
            character.animator.SetBool("isHoldingArrow", false);

            //��������ʹ������ͷ���ƣ������AI��ʹ������ͷ
            if (player != null)
            {
                Debug.Log("������ù�");
                //����������Live Arrow
                GameObject liveArrow = AmmoPoolManager.Instance.GetPooledObject(
                        "Arrow", // ��Inspector�����õ�tag
                        arrowInstantiationLocation.transform.position,
                        player.cameraHandler.cameraPivotTransform.rotation
                );
                //GameObject liveArrow = Instantiate(player.characterInventoryManager.currentAmmo.liveAmmoModel, arrowInstantiationLocation.transform.position, player.cameraHandler.cameraPivotTransform.rotation);

                // �������ط���null�������ϲ��ᷢ������Ϊ���Զ���չ��
                if (liveArrow == null)
                {
                    Debug.LogError("Failed to get arrow from pool!");
                    return;
                }

                // ���ø���״̬
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = false;

                // ������ײ��
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
                        //����֮����Զ����Ŀ�꣬���Ը��Ƴ��������
                        Quaternion arrowRotation = Quaternion.LookRotation(player.cameraHandler.currentLockOnTarget.lockOnTransform.position - liveArrow.gameObject.transform.position);
                        liveArrow.transform.rotation = arrowRotation;
                    }
                    else
                    {
                        liveArrow.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                    }
                }

                //������ʩ����������Ϊ��������
                rigidbody.AddForce(liveArrow.transform.forward * player.playerInventoryManager.currentAmmo.forwardVelocity);
                rigidbody.AddForce(liveArrow.transform.up * player.playerInventoryManager.currentAmmo.upwardVelocity);
                rigidbody.useGravity = player.playerInventoryManager.currentAmmo.useGravity;
                rigidbody.mass = player.playerInventoryManager.currentAmmo.ammoMass;
                liveArrow.transform.parent = null;

                //�趨Live Arrow�˺�
                damageCollider.characterManager = character;
                damageCollider.ammoItem = player.playerInventoryManager.currentAmmo;
                damageCollider.physicalDamage = player.playerInventoryManager.currentAmmo.physicalDamage;
            }
            else
            {
                Debug.Log("�������ù�");
                EnemyManager enemy = character as EnemyManager;

                //����������Live Arrow
                // ʹ�ö���ػ�ȡ��ʸ
                GameObject liveArrow = AmmoPoolManager.Instance.GetPooledObject(
                    "Arrow_Enemy", // ʹ�õ��˹�����tag
                    arrowInstantiationLocation.transform.position,
                    Quaternion.identity
                );

                // �������Ч�Լ��
                if (liveArrow == null)
                {
                    Debug.LogError("���˼�ʸ��ȡʧ��!");
                    return;
                }

                // ��������״̬
                Rigidbody rigidbody = liveArrow.GetComponent<Rigidbody>();
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.isKinematic = false;

                // ������ײ��
                CapsuleCollider collider = liveArrow.GetComponent<CapsuleCollider>();
                if (collider != null) collider.enabled = true;
                //GameObject liveArrow = Instantiate(character.characterInventoryManager.currentAmmo.liveAmmoModel, arrowInstantiationLocation.transform.position, Quaternion.identity);

                RangedProjectileDamageCollider damageCollider = liveArrow.GetComponent<RangedProjectileDamageCollider>();

                if (enemy.currentTarget != null)
                {
                    //����֮����Զ����Ŀ�꣬���Ը��Ƴ��������
                    Quaternion arrowRotation = Quaternion.LookRotation(enemy.currentTarget.lockOnTransform.position - liveArrow.gameObject.transform.position);
                    liveArrow.transform.rotation = arrowRotation;
                }
                else
                {
                    //liveArrow.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.lockOnTransform.eulerAngles.y, 0);
                }
                //������ʩ����������Ϊ��������
                rigidbody.AddForce(liveArrow.transform.forward * enemy.characterInventoryManager.currentAmmo.forwardVelocity);
                rigidbody.AddForce(liveArrow.transform.up * enemy.characterInventoryManager.currentAmmo.upwardVelocity);
                rigidbody.useGravity = enemy.characterInventoryManager.currentAmmo.useGravity;
                rigidbody.mass = enemy.characterInventoryManager.currentAmmo.ammoMass;
                liveArrow.transform.parent = null;

                //�趨Live Arrow�˺�
                damageCollider.characterManager = character;
                damageCollider.ammoItem = enemy.characterInventoryManager.currentAmmo;
                damageCollider.physicalDamage = enemy.characterInventoryManager.currentAmmo.physicalDamage;
                damageCollider.teamIDNumber = enemy.characterStatsManager.teamIDNumber;
            }
        }
    }
}
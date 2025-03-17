using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem
    {
        [Header("Projectile Damage")]
        public float baseDamage;

        [Header("Projectile Physics")]
        public float projectileForwardVelocity;
        public float projectileUpwardVelocity;
        public float projectileMess;
        public bool isEffectedByGravity;
        Rigidbody rigidbody;

        public override void AttemptToCastSpell(CharacterManager character)
        {
            base.AttemptToCastSpell(character);
            if(character.isUsingLeftHand)
            {
                //ʵ����������ʩ��λ�ã�������ʩ������
                GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, character.characterWeaponSlotManager.leftHandSlot.transform);
                instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(100, 100, 100);
                character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true, false, character.isUsingLeftHand);
            }
            else
            {
                GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, character.characterWeaponSlotManager.rightHandSlot.transform);
                instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(100, 100, 100);
                character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true, false, character.isUsingLeftHand);
            }
        }

        public override void SuccessfullyCastSpell(CharacterManager character)
        {
            base.SuccessfullyCastSpell(character);

            PlayerManager player = character as PlayerManager;

            //��������ʹ������ͷ���ƣ������AI��ʹ������ͷ
            if(player != null)
            {
                if (player.isUsingLeftHand)
                {
                    GameObject instantiatedSpellFX = Instantiate(spellCastFX, player.playerWeaponSlotManager.leftHandSlot.transform.position, player.cameraHandler.cameraPivotTransform.rotation);
                    //�����ж�
                    SpellDamageCollider spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
                    spellDamageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;

                    rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();

                    //�������Ͷ��ƫת�����Լ��������˺������׼����
                    if (player.cameraHandler.currentLockOnTarget != null)
                    {
                        instantiatedSpellFX.transform.LookAt(player.cameraHandler.currentLockOnTarget.transform);
                    }
                    else
                    {
                        instantiatedSpellFX.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.playerStatsManager.transform.eulerAngles.y, 0);
                    }

                    rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
                    rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
                    rigidbody.useGravity = isEffectedByGravity;
                    rigidbody.mass = projectileMess;
                    instantiatedSpellFX.transform.parent = null;
                    //֮��ʵ�������������ʩ��λ��
                }
                else
                {
                    GameObject instantiatedSpellFX = Instantiate(spellCastFX, player.playerWeaponSlotManager.rightHandSlot.transform.position, player.cameraHandler.cameraPivotTransform.rotation);
                    //�����ж�
                    SpellDamageCollider spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
                    spellDamageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber;

                    rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();

                    //�������Ͷ��ƫת�����Լ��������˺������׼����
                    if (player.cameraHandler.currentLockOnTarget != null)
                    {
                        instantiatedSpellFX.transform.LookAt(player.cameraHandler.currentLockOnTarget.transform);
                    }
                    else
                    {
                        instantiatedSpellFX.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.playerStatsManager.transform.eulerAngles.y, 0);
                    }

                    rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
                    rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
                    rigidbody.useGravity = isEffectedByGravity;
                    rigidbody.mass = projectileMess;
                    instantiatedSpellFX.transform.parent = null;
                }

                //spellDamageCollider = instantiatedSpellFx.GetComponent<SpellDamageCollider>();
            }
            else
            {

            }

        }
    }
}
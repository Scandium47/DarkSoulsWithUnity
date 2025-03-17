using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Buff FX")]
        [SerializeField] GameObject physicalBuffFX;
        [SerializeField] GameObject fireBuffFX;

        [Header("Trail FX")]
        [SerializeField] ParticleSystem defaultTrailFX;
        [SerializeField] ParticleSystem fireTrailFX;

        public bool weaponIsBuffed;
        private BuffClass weaponBuffClass;

        [HideInInspector] public MeleeWeaponDamageCollider damageCollider;
        public AudioSource audioSource;

        private void Awake()
        {
            damageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void BuffWeapon(BuffClass buffClass, float physicalBuffDamage, float fireBuffDamage, float poiseBuffDamage)
        {
            //重置任何已经激活的buff
            DebuffWeapon();
            weaponIsBuffed = true;
            weaponBuffClass = buffClass;
            audioSource.Play();

            switch (buffClass)
            {
                case BuffClass.Physical: physicalBuffFX.SetActive(true);
                    break;
                case BuffClass.Fire: fireBuffFX.SetActive(true);
                    break;
                default:
                    break;
            }

            damageCollider.physicalBuffDamage = physicalBuffDamage;
            damageCollider.fireBuffDamage = fireBuffDamage;
            damageCollider.poiseBuffDamage = poiseBuffDamage;
        }

        public void DebuffWeapon()
        {
            weaponIsBuffed = false;
            if (audioSource != null && audioSource.gameObject != null)
            {
                audioSource.Stop();
            }
            physicalBuffFX.SetActive(false);
            fireBuffFX.SetActive(false);

            damageCollider.physicalBuffDamage = 0;
            damageCollider.fireBuffDamage = 0;
            damageCollider.poiseBuffDamage = 0;
        }

        public void PlayWeaponTrailFX()
        {
            //更换武器拖尾效果
            switch (weaponBuffClass)
            {
                case BuffClass.Physical:
                    if (defaultTrailFX == null)
                        return;
                    defaultTrailFX.Play();
                    break;
                case BuffClass.Fire:
                    if (fireTrailFX == null)
                        return;
                    fireTrailFX.Play();
                    break;
                default:
                    break;
            }
        }
    }
}
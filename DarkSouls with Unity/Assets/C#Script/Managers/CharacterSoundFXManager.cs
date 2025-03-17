using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        CharacterManager character;
        AudioSource audioSource;
        //挥剑人声

        //受击人声

        //挥剑受击声
        [Header("Taking Damage Sounds")]
        public AudioClip[] takingDamageSounds;
        private List<AudioClip> potentialDamageSounds;
        private AudioClip lastDamageSoundPlayed;
        //int lastSound;

        //挥剑声
        [Header("Weapon Whooshes")]
        public List<AudioClip> potentialWeaponWhooshes;
        private AudioClip lastWeaponWhooshes;

        //脚步声

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
            audioSource = GetComponent<AudioSource>();
        }

        public virtual void PlayRandomDamageSoundFX()
        {
            potentialDamageSounds = new List<AudioClip>();

            foreach (var damageSound in takingDamageSounds)
            {
                //如果声音在添加到potentialDamageSounds之前还没有被播放过，加到potentialDamageSounds里面（避免重复播放）
                if (damageSound != lastDamageSoundPlayed)
                {
                    potentialDamageSounds.Add(damageSound);
                }
            }

            int randomValue = Random.Range(0, potentialDamageSounds.Count);
            lastDamageSoundPlayed = takingDamageSounds[randomValue];
            audioSource.PlayOneShot(takingDamageSounds[randomValue], 0.4f);
            #region 可以优化为↓
            //int randomSound = Random.Range(0, takingDamageSounds.Length);
            //if (randomSound == lastSound)
            //    PlayRandomDamageSoundFX();
            //else
            //{
            //    audioSource.PlayOneShot(takingDamageSounds[randomSound]);
            //    lastSound = randomSound;
            //}
            #endregion
        }

        public virtual void PlayRandomWeaponWhoosh()
        {
            potentialWeaponWhooshes = new List<AudioClip>();

            if(character.isUsingRightHand)
            {
                foreach (var whooshSound in character.characterInventoryManager.rightWeapon.weaponWhooshes)
                {
                    if(whooshSound != lastWeaponWhooshes)
                    {
                        potentialWeaponWhooshes.Add(whooshSound);
                    }
                }

                int randomValue = Random.Range(0, potentialWeaponWhooshes.Count);
                lastDamageSoundPlayed = character.characterInventoryManager.rightWeapon.weaponWhooshes[randomValue];
                audioSource.PlayOneShot(character.characterInventoryManager.rightWeapon.weaponWhooshes[randomValue]);
            }
            else if(character.isUsingLeftHand)
            {
                foreach (var whooshSound in character.characterInventoryManager.leftWeapon.weaponWhooshes)
                {
                    if (whooshSound != lastWeaponWhooshes)
                    {
                        potentialWeaponWhooshes.Add(whooshSound);
                    }
                }

                int randomValue = Random.Range(0, potentialWeaponWhooshes.Count);
                if(character.characterInventoryManager.leftWeapon.weaponWhooshes != null)
                {
                    lastDamageSoundPlayed = character.characterInventoryManager.leftWeapon.weaponWhooshes[randomValue];
                }
                audioSource.PlayOneShot(character.characterInventoryManager.leftWeapon.weaponWhooshes[randomValue]);
            }
        }

        public virtual void PlaySoundFX(AudioClip soundFX)
        {
            audioSource.PlayOneShot(soundFX);
        }

        public virtual void PlayRandomSoundFXFromArray(AudioClip[] soundArray)
        {
            int index = Random.Range(0, soundArray.Length);

            PlaySoundFX(soundArray[index]);
        }
    }
}
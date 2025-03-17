using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterSoundFXManager : MonoBehaviour
    {
        CharacterManager character;
        AudioSource audioSource;
        //�ӽ�����

        //�ܻ�����

        //�ӽ��ܻ���
        [Header("Taking Damage Sounds")]
        public AudioClip[] takingDamageSounds;
        private List<AudioClip> potentialDamageSounds;
        private AudioClip lastDamageSoundPlayed;
        //int lastSound;

        //�ӽ���
        [Header("Weapon Whooshes")]
        public List<AudioClip> potentialWeaponWhooshes;
        private AudioClip lastWeaponWhooshes;

        //�Ų���

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
                //�����������ӵ�potentialDamageSounds֮ǰ��û�б����Ź����ӵ�potentialDamageSounds���棨�����ظ����ţ�
                if (damageSound != lastDamageSoundPlayed)
                {
                    potentialDamageSounds.Add(damageSound);
                }
            }

            int randomValue = Random.Range(0, potentialDamageSounds.Count);
            lastDamageSoundPlayed = takingDamageSounds[randomValue];
            audioSource.PlayOneShot(takingDamageSounds[randomValue], 0.4f);
            #region �����Ż�Ϊ��
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
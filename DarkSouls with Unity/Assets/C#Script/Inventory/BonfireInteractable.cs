using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class BonfireInteractable : Interactable
    {
        WorldSaveGameManager instance;

        //����ĵ��������ͣ�

        [Header("Bonfire Teleport Transform")]
        public Transform bonfireTeleportTransform;

        [Header("Activation Status")]
        public bool hasBeenActivated;

        //����ID���浵�����������һ��ʹ�õ�����

        [Header("Bonfire FX")]
        public ParticleSystem activationFX;
        public ParticleSystem fireFX;
        public AudioClip bonfireActivationSoundFX;

        AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            instance = FindObjectOfType<WorldSaveGameManager>();
            //��������Ѿ��������ˣ�����fireFX
            if (hasBeenActivated)
            {
                fireFX.gameObject.SetActive(true);
                fireFX.Play();
                interactableText = "Rest";
            }
            else
            {
                interactableText = "Light Bonfire";
            }

            audioSource = GetComponent<AudioSource>();
        }

        public override void Interact(PlayerManager playerManager)
        {
            Debug.Log("����");

            if (hasBeenActivated)
            {
                //������Զ����棬ͬʱ��ѡ��
                instance.SaveGame();
                playerManager.uiManager.bonfiresRestWindow.SetActive(true);
                playerManager.inputHandler.bonforesRestFlag = true;
                playerManager.playerAnimatorManager.PlayTargetAnimation("Bonfire_Start", true);
            }
            else
            {
                //��������
                playerManager.playerAnimatorManager.PlayTargetAnimation("Bonfire_Activate", true);
                playerManager.uiManager.ActivateBonfirePopUp();
                hasBeenActivated = true;
                interactableText = "Rest";
                activationFX.gameObject.SetActive(true);
                activationFX.Play();
                fireFX.gameObject.SetActive(true);
                fireFX.Play();
                audioSource.PlayOneShot(bonfireActivationSoundFX);

                //�����Զ�����
                instance.SaveGame();
            }
        }
    }
}
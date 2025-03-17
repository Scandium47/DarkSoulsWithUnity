using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class BonfireInteractable : Interactable
    {
        WorldSaveGameManager instance;

        //篝火的地区（传送）

        [Header("Bonfire Teleport Transform")]
        public Transform bonfireTeleportTransform;

        [Header("Activation Status")]
        public bool hasBeenActivated;

        //篝火ID（存档，保存你最后一个使用的篝火）

        [Header("Bonfire FX")]
        public ParticleSystem activationFX;
        public ParticleSystem fireFX;
        public AudioClip bonfireActivationSoundFX;

        AudioSource audioSource;

        protected override void Awake()
        {
            base.Awake();
            instance = FindObjectOfType<WorldSaveGameManager>();
            //如果篝火已经被激活了，播放fireFX
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
            Debug.Log("篝火");

            if (hasBeenActivated)
            {
                //坐火后自动保存，同时打开选单
                instance.SaveGame();
                playerManager.uiManager.bonfiresRestWindow.SetActive(true);
                playerManager.inputHandler.bonforesRestFlag = true;
                playerManager.playerAnimatorManager.PlayTargetAnimation("Bonfire_Start", true);
            }
            else
            {
                //激活篝火
                playerManager.playerAnimatorManager.PlayTargetAnimation("Bonfire_Activate", true);
                playerManager.uiManager.ActivateBonfirePopUp();
                hasBeenActivated = true;
                interactableText = "Rest";
                activationFX.gameObject.SetActive(true);
                activationFX.Play();
                fireFX.gameObject.SetActive(true);
                fireFX.Play();
                audioSource.PlayOneShot(bonfireActivationSoundFX);

                //点火后自动保存
                instance.SaveGame();
            }
        }
    }
}
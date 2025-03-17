using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterController characterController;
        public Animator animator;
        public CharacterAnimatorManager characterAnimatorManager;
        public CharacterWeaponSlotManager characterWeaponSlotManager;
        public CharacterStatsManager characterStatsManager;
        public CharacterInventoryManager characterInventoryManager;
        public CharacterEffectsManager characterEffectsManager;
        public CharacterSoundFXManager characterSoundFXManager;
        public CharacterCombatManager characterCombatManager;

        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Ray Casts")]
        public Transform criticalAttackRayCastStartPoint;

        [Header("Interaction")]
        public bool isInteracting;

        [Header("Combat Flags")]
        public bool canBeRiposted;
        public bool canBeParried;
        public bool canDoCombo;
        public bool canRoll = true;
        public bool isParrying;
        public bool isBlocking;
        public bool isInvulnerable;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isHoldingArrow;
        public bool isAiming;
        public bool isTwoHandingWeapon;
        public bool isPerformingFullyChargedAttack;
        public bool isAttacking;
        public bool isBeingBackstabbed;
        public bool isBeingRiposted;
        public bool isPerformingBackstab;
        public bool isPerformingRiposte;

        [Header("Movement Flags")]
        public bool isRotatingWithRootMotion;
        public bool canRotate;
        public bool isSprinting;
        public bool isGrounded;

        [Header("Spells")]
        public bool isFiringSpell;

        [Header("Status")]
        public bool isDead;

        //Damage will be inflicted during an animation event
        //Used in backstab or riposte animations
        public int pendingCriticalDamage;

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void FixedUpdate()
        {
            characterAnimatorManager.CheckHandIKWeight(characterWeaponSlotManager.rightHandIKTarget, characterWeaponSlotManager.leftHandIKTarget, isTwoHandingWeapon);
        }

        protected virtual void Update()
        {
            characterEffectsManager.ProcessAllTimedEffects();
        }

        public virtual void UpdateWhitchHandCharacterIsUsing(bool usingRightHand)
        {
            if(usingRightHand)
            {
                isUsingLeftHand = false;
                isUsingRightHand = true;
            }
            else
            {
                isUsingLeftHand = true;
                isUsingRightHand = false;
            }
        }
    }
}
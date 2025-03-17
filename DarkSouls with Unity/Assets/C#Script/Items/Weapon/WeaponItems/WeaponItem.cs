using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Animator Replacer")]
        public AnimatorOverrideController weaponController;
        //public string offHandIdleAnimation = "Left_Arm_Idle_01";

        [Header("weapon Type")]
        public WeaponType weaponType;

        [Header("Damage")]
        public int physicalDamage;
        public int magicDamage;
        public int fireDamage;
        public int lightningDamage;
        public int darknessDamage;

        [Header("Damage Modifiers")]
        public float lightAttackDamageModifier = 1;
        public float lightAttackComboDamageModifier;
        public float runningAttackDamageModifier;
        public float heavyAttackDamageModifier = 2;
        public float heavyAttackComboDamageModifier;
        public float jumpingAttackDamageModifier;
        public int criticalDamageMuiltiplier = 4;   //ÖÂÃüÉËº¦Îª»ù´¡ÉËº¦µÄËÄ±¶
        public float guardBreakModifier = 1;

        [Header("Poise")]
        public float poiseBreak;
        public float offensivePoiseBonus;

        [Header("Absorption")]
        public float physicalBlockingDamageAbsorption;
        public float magicBlockingDamageAbsorption;
        public float fireBlockingDamageAbsorption;
        public float lightningBlockingDamageAbsorption;
        public float darknessBlockingDamageAbsorption;

        [Header("Stability")]
        public int stability = 67;

        #region EP80ºóÉ¾³ý
        //[Header("Idle Animations")]
        //public string right_hand_idle;
        //public string left_hand_idle;
        //public string th_idle;  //twohand_idle

        //[Header("One Handed Attack Animations")]
        //public string OH_Light_Attack_01;
        //public string OH_Light_Attack_02;
        //public string OH_Heavy_Attack_01;

        //[Header("Two Handed Attack Animations")]
        //public string th_light_attack_01;
        //public string th_light_attack_02;
        //public string th_light_attack_03;
        //public string th_heavy_attack_01;
        //public string th_heavy_attack_02;

        //[Header("Weapon Art")]
        //public string weapon_art;

        //[Header("Weapon Type")]
        //public bool isSpellCaster;
        //public bool isFaithCaster;
        //public bool isPyroCaster;
        //public bool isMeleeWeapon;
        //public bool isShieldWeapon;
        #endregion

        [Header("Stamina Costs")]
        public int baseStaminaCost;     //»ù´¡ÄÍÁ¦
        public float lightAttackStaminaMultiplier;      //Çá¹¥»÷ÄÍÁ¦±¶Êý
        public float lightAttackComboStaminaMultiplier;
        public float runningAttackStaminaMultiplier;
        public float heavyAttackStaminaMultiplier;     //ÖØ¹¥»÷ÄÍÁ¦±¶Êý
        public float heavyAttackComboStaminaMultiplier;
        public float jumpingAttackStaminaMultiplier;


        [Header("Item Actions")]
        public ItemAction oh_hold_RB_Action;
        public ItemAction oh_tap_RB_Action;
        public ItemAction oh_tap_RB_Critical_Action;
        public ItemAction oh_tap_LB_Action;
        public ItemAction oh_hold_LB_Action;
        public ItemAction oh_tap_RT_Action;
        public ItemAction oh_hold_RT_Action;
        public ItemAction oh_tap_LT_Action;
        public ItemAction oh_hold_LT_Action;

        [Header("Two Handed Item Actions")]
        public ItemAction th_hold_RB_Action;
        public ItemAction th_tap_RB_Action;
        public ItemAction th_tap_LB_Action;
        public ItemAction th_hold_LB_Action;
        public ItemAction th_tap_RT_Action;
        public ItemAction th_hold_RT_Action;
        public ItemAction th_tap_LT_Action;
        public ItemAction th_hold_LT_Action;

        [Header("Sound FX")]
        public AudioClip[] weaponWhooshes;
        public AudioClip[] blockingNoises;
    }
}

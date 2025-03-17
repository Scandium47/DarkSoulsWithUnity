using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("NAME")]
        public string characterName = "Nameless";

        [Header("Team I.D")]
        public int teamIDNumber = 0;

        public int maxHealth;
        public int currentHealth;

        public float maxFocusPoints;
        public float currentFocusPoints;

        public float maxStamina;
        public float currentStamina;

        public int currentSoulCount = 0;
        public int soulsAwardedOnDeath = 50;

        [Header("CHARACTER LEVEL")]
        public int playerLevel = 1;

        [Header("STAT LEVELS")]
        public int healthLevel = 10;            //�����ӵ�
        public int staminaLevel = 10;         //�����ӵ�
        public int focusLevel = 10;             //רע�ӵ�
        public int poiseLevel = 10;             //���Լӵ�
        public int strengthLevel = 10;        //�����ӵ�
        public int dexterityLevel = 10;       //���ݼӵ�
        public int intelligenceLevel = 10;   //�����ӵ�
        public int faithLevel = 10;              //�����ӵ�

        [Header("Equip Load")]
        public float currentEquipLoad = 0;
        public float maxEquipLoad = 0;
        public EncumbranceLevel encumbranceLevel;

        [Header("poise")]
        public float totalPoiseDefence; //The total poise during damage calculation ������
        public float offensivePoiseBonus;   //The poise you Gain during an attack with a weapon ͨ������������ȡ
        public float armorPoiseBonus;       //The poise you Gain from wearing what ever you have equipped ͨ��װ����ȡ
        public float totalPoiseResetTime = 15;
        public float poiseResetTimer = 0;

        [Header("Armor Absorptions")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionHands;
        public float physicalDamageAbsorptionLegs;

        public float fireDamageAbsorptionHead;
        public float fireDamageAbsorptionBody;
        public float fireDamageAbsorptionHands;
        public float fireDamageAbsorptionLegs;

        [Header("Resistances")]
        public float poisonResistance;

        [Header("Blocking Absorptions")]
        public float blockingPhysicalDamageAbsorption;
        public float blockingFireDamageAbsorption;
        public float blockingStabilityRating;         //���ȶ��ȼ�

        //�κ��˺���Ҫ�������µı��ʼӳ�
        [Header("Damage Type Modifiers")]
        public float physicalDamagePercentageModifier = 100;
        public float fireDamagePercentageModifier = 100;

        //���׼�����˺���ȥ�˺�����ֵ
        [Header("Damage Absorption Modifiers")]
        public float physicalAbsorptionPercentageModifier = 0;
        public float fireAbsorptionPercentageModifier = 0;
        //Lightning Absorption
        //Magic Absorption
        //Dark Absorption

        [Header("Poison")]
        public bool isPoisoned;
        public float poisonBuildup = 0;     //���Բۣ����ܵ�100������ж�
        public float poisonAmount = 100;        //�ж��ۣ�����ж����100��ʼ���٣���������˺����ڼ���0��ֹͣ

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandlePoiseResetTimer();
        }

        protected virtual void Start()
        {
            totalPoiseDefence = armorPoiseBonus;
            CalculateAndSetMaxEquipload();
        }

        public virtual void TakeDamageNoAnimation(int physicalDamage, int fireDamage)
        {
            if (character.isDead)
                return;

            //�����˺�
            float totalPhysicalDamageAbsorption = 1 -
                (1 - physicalDamageAbsorptionHead / 100) *
                (1 - physicalDamageAbsorptionBody / 100) *
                (1 - physicalDamageAbsorptionHands / 100) *
                (1 - physicalDamageAbsorptionLegs / 100);
            physicalDamage = Mathf.RoundToInt(physicalDamage - (physicalDamage * totalPhysicalDamageAbsorption));

            //�����˺�
            float totalFireDamageAbsorption = 1 -
                (1 - fireDamageAbsorptionHead / 100) *
                (1 - fireDamageAbsorptionBody / 100) *
                (1 - fireDamageAbsorptionHands / 100) *
                (1 - fireDamageAbsorptionLegs / 100);
            fireDamage = Mathf.RoundToInt(fireDamage - (fireDamage * totalFireDamageAbsorption));

            float finalDamage = physicalDamage + fireDamage; //+ extra damage

            currentHealth = Mathf.RoundToInt(currentHealth - finalDamage);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                character.isDead = true;
            }
        }

        public virtual void TakePoisonDamage(int damage)
        {
            currentHealth = currentHealth - damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                character.isDead = true;
            }
        }

        public virtual void TakeFireDamage(int damage)
        {
            currentHealth = currentHealth - damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                character.isDead = true;
            }
        }

        public virtual void HandlePoiseResetTimer()
        {
            if(poiseResetTimer > 0)
            {
                poiseResetTimer = poiseResetTimer - Time.deltaTime;
            }
            else
            {
                totalPoiseDefence = armorPoiseBonus;
            }
        }

        public virtual void DeductStamina(float staminaToDeduct)
        {
            currentStamina = currentStamina - staminaToDeduct;
        }

        public int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public float SetMaxFocusPointsFromFocusLevel()
        {
            maxFocusPoints = focusLevel * 10;
            return maxFocusPoints;
        }

        public float SetMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void CalculateAndSetMaxEquipload()
        {
            //���������ȼ����ӳ������ֵ
            float totalEquipLoad = 40;
            
            for(int i = 0; i < staminaLevel; i++)
            {
                if(i < 25)
                {
                    totalEquipLoad = totalEquipLoad + 1.2f;
                }
                if(i >= 25 && i <= 50)
                {
                    totalEquipLoad = totalEquipLoad + 1.4f;
                }
                if(i > 50)
                {
                    totalEquipLoad = totalEquipLoad + 1;
                }
            }

            maxEquipLoad = totalEquipLoad;
        }

        public void CalculateAndSetCurrentEquipLoad(float equipLoad)
        {
            currentEquipLoad = equipLoad;

            encumbranceLevel = EncumbranceLevel.Light;

            if (currentEquipLoad > (maxEquipLoad * 0.3f))
            {
                encumbranceLevel = EncumbranceLevel.Medium;
            }
            if (currentEquipLoad > (maxEquipLoad * 0.7f))
            {
                encumbranceLevel = EncumbranceLevel.Heavy;
            }
            if (currentEquipLoad > maxEquipLoad)
            {
                encumbranceLevel = EncumbranceLevel.Overloaded;
            }
        }

        public virtual void HealCharacter(int healAmount)
        {
            currentHealth = currentHealth + healAmount;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }
}
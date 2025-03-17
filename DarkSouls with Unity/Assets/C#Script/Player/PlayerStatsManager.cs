using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;

        public HealthBar healthBar;
        public FocusPointBar focusPointsBar;
        public StaminaBar staminaBar;

        public float staminaRegenerationAmount = 50;
        public float staminaRegenerationAmountWhilstBlocking = 0.5f;
        public float staminaRegenTimer = 0;

        private float sprintingTimer = 0;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
            focusPointsBar = FindObjectOfType<FocusPointBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
        }

        protected override void Start()
        {
            base.Start();

            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            maxFocusPoints = SetMaxFocusPointsFromFocusLevel();
            currentFocusPoints = maxFocusPoints;
            focusPointsBar.SetMaxFocusPoints(maxFocusPoints);
            focusPointsBar.SetCurrentFocusPoints(currentFocusPoints);

            maxStamina = SetMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
            staminaBar.SetMaxStamina(maxStamina);
            staminaBar.SetCurrentStamina(currentStamina);
        }

        public override void HandlePoiseResetTimer()
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer = poiseResetTimer - Time.deltaTime;
            }
            else if(poiseResetTimer <= 0 && !player.isInteracting)
            {
                totalPoiseDefence = armorPoiseBonus;
            }
        }

        public override void TakePoisonDamage(int damage)
        {
            if (player.isDead)
                return;

            base.TakePoisonDamage(damage);
            healthBar.SetCurrentHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                player.isDead = true;
                player.playerAnimatorManager.PlayTargetAnimation("Dead_01", true);
                player.isInteracting = player.animator.GetBool("isInteracting");
                //清除双持IK（权重降为零0）
                player.playerAnimatorManager.EraseHandIKForWeapon();
                //Handle Player Death
            }
        }

        public override void TakeFireDamage(int damage)
        {
            if (player.isDead)
                return;

            base.TakePoisonDamage(damage);
            healthBar.SetCurrentHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                player.isDead = true;
                player.playerAnimatorManager.PlayTargetAnimation("Dead_01", true);
                player.isInteracting = player.animator.GetBool("isInteracting");
                //清除双持IK（权重降为零0）
                player.playerAnimatorManager.EraseHandIKForWeapon();
                //Handle Player Death
            }
        }

        public override void TakeDamageNoAnimation(int physicalDamage, int fireDamage)
        {
            base.TakeDamageNoAnimation(physicalDamage, fireDamage);
            healthBar.SetCurrentHealth(currentHealth);
        }

        public override void DeductStamina(float staminaToDeduct)
        {
            base.DeductStamina(staminaToDeduct);
            staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
        }

        public void DeductSprintingStamina(float staminaToDeduct)
        {
            if(player.isSprinting)
            {
                sprintingTimer = sprintingTimer + Time.deltaTime;

                if(sprintingTimer > 0.05f)
                {
                    sprintingTimer = 0;
                    currentStamina = currentStamina - staminaToDeduct;
                    staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                }
            }
            else
            {
                sprintingTimer = 0;
            }
        }

        public void RegenerateStamina()
        {
            if(player.isInteracting || player.isSprinting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;

                if (currentStamina < maxStamina && staminaRegenTimer > 0.5f)
                {
                    if(player.isBlocking)
                    {
                        //格挡精力恢复速度变慢
                        currentStamina += staminaRegenerationAmountWhilstBlocking * Time.deltaTime;
                        staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                    }
                    else
                    {
                        currentStamina += staminaRegenerationAmount * Time.deltaTime;
                        staminaBar.SetCurrentStamina(Mathf.RoundToInt(currentStamina));
                    }
                }
            }
        }

        public override void HealCharacter(int healAmount)
        {
            base.HealCharacter(healAmount);

            healthBar.SetCurrentHealth(currentHealth);
        }

        public void DeductFocusPoints(int focusPoints)
        {
            currentFocusPoints = currentFocusPoints - focusPoints;

            if(currentFocusPoints < 0)
            {
                currentFocusPoints = 0;
            }

            focusPointsBar.SetCurrentFocusPoints(currentFocusPoints);
        }

        public void AddSouls(int souls)
        {
            currentSoulCount = currentSoulCount + souls;
        }
    }
}

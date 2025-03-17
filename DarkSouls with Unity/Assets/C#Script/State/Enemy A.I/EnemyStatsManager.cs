using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyStatsManager : CharacterStatsManager
    {
        EnemyManager enemy;
        public UIAICharacterHealthBar enemyHealthBar;

        public bool isBoss;

        public float staminaRegenerationAmount = 30;
        public float staminaRegenerationAmountWhilstBlocking = 0.1f;
        public float staminaRegenTimer = 0;
        protected override void Awake()
        {
            base.Awake();
            enemy = GetComponent<EnemyManager>();
            maxHealth = SetEnemyMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            maxStamina = SetEnemyMaxStaminaFromStaminaLevel();
            currentStamina = maxStamina;
        }

        protected override void Start()
        {
            if(!isBoss)
            {
                enemyHealthBar.SetMaxHealth(maxHealth);
                enemyHealthBar.SetMaxStamina(Mathf.RoundToInt(maxStamina));
            }
        }

        public int SetEnemyMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public int SetEnemyMaxStaminaFromStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return Mathf.RoundToInt(maxStamina);
        }

        public void RegenerateStamina()
        {
            if (enemy.isInteracting)
            {
                staminaRegenTimer = 0;
            }
            else
            {
                staminaRegenTimer += Time.deltaTime;

                if (currentStamina < maxStamina && staminaRegenTimer > 0.5f)
                {
                    if (enemy.isBlocking)
                    {
                        //格挡精力恢复速度变慢
                        currentStamina += staminaRegenerationAmountWhilstBlocking * Time.deltaTime;
                    }
                    else
                    {
                        currentStamina += staminaRegenerationAmount * Time.deltaTime;
                    }
                }
            }
        }

        public override void TakeDamageNoAnimation(int physicalDamage, int fireDamage)
        {
            base.TakeDamageNoAnimation(physicalDamage, fireDamage);

            if (!isBoss)
            {
                enemyHealthBar.SetHealth(currentHealth);
                enemyHealthBar.timeUntilBarIsHidden = 5;
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    if(!enemy.isBeingBackstabbed && !enemy.isBeingRiposted)
                    {
                        Debug.Log("无动画死亡");
                        enemy.enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
                    }
                    enemy.isDead = true;
                }
            }
            else if (isBoss && enemy.enemyBossManager != null)
            {
                enemy.enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    if (!enemy.isBeingBackstabbed && !enemy.isBeingRiposted)
                    {
                        enemy.enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
                    }
                    enemy.isDead = true;
                }
            }
        }

        public override void TakePoisonDamage(int damage)
        {
            if (enemy.isDead)
                return;

            base.TakePoisonDamage(damage);
            if (!isBoss)
            {
                enemyHealthBar.SetHealth(currentHealth);
                enemyHealthBar.timeUntilBarIsHidden = 5;
            }
            else if (isBoss && enemy.enemyBossManager != null)
            {
                enemy.enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            }
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                enemy.isDead = true;
                enemy.enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
                //Handle Player Death
            }
        }

        public override void TakeFireDamage(int damage)
        {
            if (enemy.isDead)
                return;

            base.TakePoisonDamage(damage);
            if (!isBoss)
            {
                enemyHealthBar.SetHealth(currentHealth);
                enemyHealthBar.timeUntilBarIsHidden = 5;
            }
            else if (isBoss && enemy.enemyBossManager != null)
            {
                enemy.enemyBossManager.UpdateBossHealthBar(currentHealth, maxHealth);
            }
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                enemy.isDead = true;
                enemy.enemyAnimatorManager.PlayTargetAnimation("Dead_01", true);
            }
        }

        public void BreakGoard()
        {
            enemy.enemyAnimatorManager.PlayTargetAnimation("Break Guard", true);
        }
    }
}

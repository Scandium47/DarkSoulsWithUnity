using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SG
{
    public class UIAICharacterHealthBar : MonoBehaviour
    {
        public Slider slider;
        public float timeUntilBarIsHidden = 0;     //几秒后隐藏血条(初始化)
        [SerializeField] UIYellowBar yellowBar;
        [SerializeField] float yellowBarTimer = 2;
        [SerializeField] TextMeshProUGUI damageText;
        [SerializeField] int currentDamageTaken;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        //private void OnDisable()
        //{
        //    damageText.text = "";
        //    currentDamageTaken = 0;     //在禁用时重置伤害弹出文本
        //}

        public void SetHealth(int health)
        {
            if (yellowBar != null)
            {
                yellowBar.gameObject.SetActive(true);   //触发黄条的OnEnable
                yellowBar.timer = yellowBarTimer;   //每次受击都会刷新黄条消失时间，以便于看到短期总伤

                if (health > slider.value)
                {
                    yellowBar.slider.value = health;
                }
            }

            currentDamageTaken = currentDamageTaken + Mathf.RoundToInt(slider.value - health);
            damageText.text = currentDamageTaken.ToString();

            slider.value = health;
            timeUntilBarIsHidden = 5;   //几秒后隐藏血条
        }

        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

            if(yellowBar != null)
            {
                yellowBar.SetMaxStat(maxHealth);
            }
        }

        public void SetStamina(int stamina)
        {
            slider.value = stamina;
        }

        public void SetMaxStamina(int maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }

        private void Update()
        {
            timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;

            if(slider != null)
            {
                if(yellowBar.slider.value <= slider.value)
                {
                    damageText.text = "";
                    currentDamageTaken = 0;     //在黄条消耗后时重置伤害弹出文本
                }
                //倒计时结束，隐藏血条
                if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    //倒计时如果没有结束，但是血条已经隐藏，激活
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        slider.gameObject.SetActive(true);
                    }
                }

                //生命降到0以下销毁血条
                if (slider.value <= 0 && timeUntilBarIsHidden <= 0)
                {
                    Destroy(slider.gameObject);
                }
            }      
        }

        //血条朝向相机
        private void LateUpdate()
        {
            if(slider != null)
            {
                transform.LookAt(Camera.main.transform);
            }
        }
    }
}
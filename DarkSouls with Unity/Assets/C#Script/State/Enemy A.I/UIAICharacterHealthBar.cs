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
        public float timeUntilBarIsHidden = 0;     //���������Ѫ��(��ʼ��)
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
        //    currentDamageTaken = 0;     //�ڽ���ʱ�����˺������ı�
        //}

        public void SetHealth(int health)
        {
            if (yellowBar != null)
            {
                yellowBar.gameObject.SetActive(true);   //����������OnEnable
                yellowBar.timer = yellowBarTimer;   //ÿ���ܻ�����ˢ�»�����ʧʱ�䣬�Ա��ڿ�����������

                if (health > slider.value)
                {
                    yellowBar.slider.value = health;
                }
            }

            currentDamageTaken = currentDamageTaken + Mathf.RoundToInt(slider.value - health);
            damageText.text = currentDamageTaken.ToString();

            slider.value = health;
            timeUntilBarIsHidden = 5;   //���������Ѫ��
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
                    currentDamageTaken = 0;     //�ڻ������ĺ�ʱ�����˺������ı�
                }
                //����ʱ����������Ѫ��
                if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    //����ʱ���û�н���������Ѫ���Ѿ����أ�����
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        slider.gameObject.SetActive(true);
                    }
                }

                //��������0��������Ѫ��
                if (slider.value <= 0 && timeUntilBarIsHidden <= 0)
                {
                    Destroy(slider.gameObject);
                }
            }      
        }

        //Ѫ���������
        private void LateUpdate()
        {
            if(slider != null)
            {
                transform.LookAt(Camera.main.transform);
            }
        }
    }
}
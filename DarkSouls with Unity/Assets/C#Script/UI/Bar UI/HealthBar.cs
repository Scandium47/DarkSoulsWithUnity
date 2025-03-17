using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;
        public float widthPerHealth = 3f;   //ÿ��Ѫ����Ӧ�Ŀ��

        private RectTransform sliderRect;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            sliderRect = GetComponent<RectTransform>();
        }

        private void Start()
        {

        }
        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

            sliderRect.sizeDelta = new Vector2(maxHealth * widthPerHealth, sliderRect.sizeDelta.y);
        }

        public void SetCurrentHealth(int currenthealth)
        {
            slider.value = currenthealth;
        }
    }
}

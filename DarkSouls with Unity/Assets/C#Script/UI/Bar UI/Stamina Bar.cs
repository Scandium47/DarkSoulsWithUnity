using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider slider;
        public float widthPerHealth = 3f;   //每点耐力对应的宽度

        private RectTransform sliderRect;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            sliderRect = GetComponent<RectTransform>();
        }

        private void Start()
        {

        }
        public void SetMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;

            sliderRect.sizeDelta = new Vector2(maxStamina * widthPerHealth, sliderRect.sizeDelta.y);
        }

        public void SetCurrentStamina(float currentstamina)
        {
            slider.value = currentstamina;
        }
    }
}
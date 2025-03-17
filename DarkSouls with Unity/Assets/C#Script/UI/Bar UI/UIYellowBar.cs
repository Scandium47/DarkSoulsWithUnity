using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIYellowBar : MonoBehaviour
    {
        public Slider slider;
        public UIAICharacterHealthBar parentHealthBar;

        public float timer;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            parentHealthBar = GetComponentInParent<UIAICharacterHealthBar>();
        }

        private void OnEnable()
        {
            if(timer <= 0)
            {
                timer = 1.5f;     //黄条在消耗前的停留时间
            }
        }

        public void SetMaxStat(int maxStat)
        {
            slider.maxValue = maxStat;
            slider.value = maxStat;
        }

        private void Update()
        {
            if(timer <= 0)
            {
                if(slider.value > parentHealthBar.slider.value)
                {
                    slider.value = slider.value - 2f;
                }
                else if(slider.value <= parentHealthBar.slider.value)
                {
                    //slider.value = parentHealthBar.slider.value;
                    gameObject.SetActive(false);
                }
            }
            else
            {
                timer = timer - Time.deltaTime;
            }
        }
    }
}
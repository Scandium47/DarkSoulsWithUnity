using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class FocusPointBar : MonoBehaviour
    {
        public Slider slider;
        public float widthPerHealth = 3f;   //每点专注对应的宽度

        private RectTransform sliderRect;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            sliderRect = GetComponent<RectTransform>();
        }

        private void Start()
        {

        }
        public void SetMaxFocusPoints(float maxFocusPoints)
        {
            slider.maxValue = maxFocusPoints;
            slider.value = maxFocusPoints;

            sliderRect.sizeDelta = new Vector2(maxFocusPoints * widthPerHealth, sliderRect.sizeDelta.y);
        }

        public void SetCurrentFocusPoints(float currentFocusPoints)
        {
            slider.value = currentFocusPoints;
        }
    }
}
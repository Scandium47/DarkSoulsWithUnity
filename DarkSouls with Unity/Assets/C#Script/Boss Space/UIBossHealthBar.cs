using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SG
{
    public class UIBossHealthBar : MonoBehaviour
    {
        public TextMeshProUGUI bossName;
        Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            bossName = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            SetHealthBarToInactive();
        }

        public void SetBossName(string name)
        {
            bossName.text = name;
        }

        public void SetUIHealthBarToActive()
        {
            slider.gameObject.SetActive(true);
        }

        public void SetHealthBarToInactive()
        {
            slider.gameObject.SetActive(false);
        }

        public void SetBossMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetBossCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SG
{
    public class SettingUI : MonoBehaviour
    {
        WorldSaveGameManager instance;

        private void Awake()
        {
            instance = FindObjectOfType<WorldSaveGameManager>();
        }

        public void SaveGameInSetting()
        {
            instance.SaveGame();
        }

        public void LoadGameInSetting()
        {
            instance.LoadGame();
        }
    }
}
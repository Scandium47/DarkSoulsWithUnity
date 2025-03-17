using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

namespace SG
{
    public class MainMenu : MonoBehaviour
    {
        public AssetReference persistentSence;
        public void PlayGame()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 1);
            //SceneManager.LoadScene("Hyde Tower");
            Addressables.LoadSceneAsync(persistentSence);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
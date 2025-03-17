using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InitialLoad : MonoBehaviour
{
    public AssetReference persistentSence;

    private void Awake()
    {
        Addressables.LoadSceneAsync(persistentSence);
    }
}

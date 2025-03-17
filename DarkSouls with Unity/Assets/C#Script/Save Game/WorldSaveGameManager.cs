using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        public PlayerManager player;

        [Header("Save Data Writer")]
        SaveGameDataWriter saveGameDataWriter;

        [Header("Current Character Data")]
        //角色数据
        public CharacterSaveData currentCharacterSaveData;
        [SerializeField] private string fileName;

        [Header("Save/Load")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            //加载所有角色信息
            //初始化掉落物ID
            InitializeItemIdCounter();
        }

        private void Update()
        {
            if(saveGame)
            {
                saveGame = false;
                //保存游戏
                SaveGame();
            }
            else if(loadGame)
            {
                loadGame = false;
                //加载保存的数据
                LoadGame();
            }
        }

        //新游戏

        //保存游戏
        public void SaveGame()
        {
            //声明已经有的变量
            saveGameDataWriter = new SaveGameDataWriter();
            //根据不同的设备和平台（Windows、Max OS、Android、iOS），找到返回的持久化数据存储路径
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            //找到文件名
            saveGameDataWriter.dataSaveFileName = fileName;

            //将数据传递到当前的保存文件
            player.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            //将角色数据写入Json文件并保存到设备上
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("正在保存...");
            Debug.Log("文件保存至：" + fileName);
        }

        //加载游戏
        public void LoadGame()
        {
            //根据角色槽来选择加载对应文件

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveGameDataWriter.dataSaveFileName = fileName;
            currentCharacterSaveData = saveGameDataWriter.LoadCharacterDataFromJson();

            StartCoroutine(LoadWorldSceneAsynchronously());
        }

        //初始化掉落物ID
        public void InitializeItemIdCounter()
        {
            // 获取场景中所有现存的 WeaponPickUP
            WeaponPickUP[] allItemsInScene = Resources.FindObjectsOfTypeAll<WeaponPickUP>();

            // 如果当前是新存档，从80000开始遍历所有掉落物ID并计算最大ID
            if (currentCharacterSaveData.nextItemPickUpId == 80001)
            {
                int maxIdInScene = 80000; // 默认起始值-1
                foreach (WeaponPickUP item in allItemsInScene)
                {
                    if (item.itemPickUpId > maxIdInScene)
                        maxIdInScene = item.itemPickUpId;
                }
                currentCharacterSaveData.nextItemPickUpId = maxIdInScene + 1;
            }
            else // 如果是加载旧存档，更新下一个生成掉落物的ID
            {
                // 检查场景中是否有更大 ID（防止手动放置新物品导致冲突）
                int maxIdInScene = allItemsInScene.Length > 80001 ?
                    allItemsInScene.Max(item => item.itemPickUpId) : 80000;

                if (maxIdInScene >= currentCharacterSaveData.nextItemPickUpId)
                {
                    currentCharacterSaveData.nextItemPickUpId = maxIdInScene + 1;
                }
            }
        }


        private IEnumerator LoadWorldSceneAsynchronously()
        {
            if(player == null)
            {
                player = FindObjectOfType<PlayerManager>();

                if (player == null)
                {
                    Debug.LogError("PlayerManager not found in the scene.");
                    yield break;
                }
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(2);

            while(!loadOperation.isDone)
            {
                float loadingProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                //启用一个加载中的场景 → 将进度转化为加载屏幕上的滑动条，加载时启用，加载结束后禁用
                yield return null;
            }

            player.LoadCharacterDataFromCurrentCharacterSvaeData(ref currentCharacterSaveData);
        }
    }
}
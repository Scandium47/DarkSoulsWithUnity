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
        //��ɫ����
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
            //�������н�ɫ��Ϣ
            //��ʼ��������ID
            InitializeItemIdCounter();
        }

        private void Update()
        {
            if(saveGame)
            {
                saveGame = false;
                //������Ϸ
                SaveGame();
            }
            else if(loadGame)
            {
                loadGame = false;
                //���ر��������
                LoadGame();
            }
        }

        //����Ϸ

        //������Ϸ
        public void SaveGame()
        {
            //�����Ѿ��еı���
            saveGameDataWriter = new SaveGameDataWriter();
            //���ݲ�ͬ���豸��ƽ̨��Windows��Max OS��Android��iOS�����ҵ����صĳ־û����ݴ洢·��
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            //�ҵ��ļ���
            saveGameDataWriter.dataSaveFileName = fileName;

            //�����ݴ��ݵ���ǰ�ı����ļ�
            player.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            //����ɫ����д��Json�ļ������浽�豸��
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("���ڱ���...");
            Debug.Log("�ļ���������" + fileName);
        }

        //������Ϸ
        public void LoadGame()
        {
            //���ݽ�ɫ����ѡ����ض�Ӧ�ļ�

            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveGameDataWriter.dataSaveFileName = fileName;
            currentCharacterSaveData = saveGameDataWriter.LoadCharacterDataFromJson();

            StartCoroutine(LoadWorldSceneAsynchronously());
        }

        //��ʼ��������ID
        public void InitializeItemIdCounter()
        {
            // ��ȡ�����������ִ�� WeaponPickUP
            WeaponPickUP[] allItemsInScene = Resources.FindObjectsOfTypeAll<WeaponPickUP>();

            // �����ǰ���´浵����80000��ʼ�������е�����ID���������ID
            if (currentCharacterSaveData.nextItemPickUpId == 80001)
            {
                int maxIdInScene = 80000; // Ĭ����ʼֵ-1
                foreach (WeaponPickUP item in allItemsInScene)
                {
                    if (item.itemPickUpId > maxIdInScene)
                        maxIdInScene = item.itemPickUpId;
                }
                currentCharacterSaveData.nextItemPickUpId = maxIdInScene + 1;
            }
            else // ����Ǽ��ؾɴ浵��������һ�����ɵ������ID
            {
                // ��鳡�����Ƿ��и��� ID����ֹ�ֶ���������Ʒ���³�ͻ��
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
                //����һ�������еĳ��� �� ������ת��Ϊ������Ļ�ϵĻ�����������ʱ���ã����ؽ��������
                yield return null;
            }

            player.LoadCharacterDataFromCurrentCharacterSvaeData(ref currentCharacterSaveData);
        }
    }
}
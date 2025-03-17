using System;
using System.IO;
using UnityEngine;

namespace SG
{
    public class SaveGameDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string dataSaveFileName = "";

        public CharacterSaveData LoadCharacterDataFromJson()
        {
            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);

            CharacterSaveData loadedSaveData = null;

            if(File.Exists(savePath))
            {
                try
                {
                    //Ҫ��ȡ������
                    string saveDataToLoad = "";

                    using(FileStream stream = new FileStream(savePath, FileMode.Open))
                    {
                        using(StreamReader reader = new StreamReader(stream))
                        {
                            saveDataToLoad = reader.ReadToEnd();
                        }
                    }

                    // �����л�����
                    loadedSaveData = JsonUtility.FromJson<CharacterSaveData>(saveDataToLoad);
                }
                catch(Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
            else
            {
                Debug.Log("�ļ�������");
            }

            return loadedSaveData;
        }

        public void WriteCharacterDataToSaveFile(CharacterSaveData characterData)
        {
            //����һ��·��ȥ�����ļ�
            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);

            try
            {
                //����һ��Ŀ¼�ļ�
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("SAVE PATH = " + savePath);

                //���������л�ΪJson�ļ�
                string dataToStore = JsonUtility.ToJson(characterData, true);

                //���ļ�д��ϵͳ
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using(StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("���Ա���ʱ�����׳��쳣����Ϸ���ᱻ����" + ex);
            }
        }

        public void DeleteSaveFile()
        {
            //ɾ���ļ�
            File.Delete(Path.Combine(saveDataDirectoryPath, dataSaveFileName));
        }

        public bool CheckSaveFileExists()
        {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, dataSaveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
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
                    //要读取的数据
                    string saveDataToLoad = "";

                    using(FileStream stream = new FileStream(savePath, FileMode.Open))
                    {
                        using(StreamReader reader = new StreamReader(stream))
                        {
                            saveDataToLoad = reader.ReadToEnd();
                        }
                    }

                    // 反序列化数据
                    loadedSaveData = JsonUtility.FromJson<CharacterSaveData>(saveDataToLoad);
                }
                catch(Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
            else
            {
                Debug.Log("文件不存在");
            }

            return loadedSaveData;
        }

        public void WriteCharacterDataToSaveFile(CharacterSaveData characterData)
        {
            //创建一个路径去保存文件
            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);

            try
            {
                //创建一个目录文件
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("SAVE PATH = " + savePath);

                //将数据序列化为Json文件
                string dataToStore = JsonUtility.ToJson(characterData, true);

                //将文件写入系统
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
                Debug.LogError("尝试保存时错误抛出异常，游戏不会被保存" + ex);
            }
        }

        public void DeleteSaveFile()
        {
            //删除文件
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
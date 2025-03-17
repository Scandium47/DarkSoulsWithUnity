using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SG
{
    public class DeepSeekAPI_Sample : MonoBehaviour
    {
        private string apiKey = "sk-cvncosdvyvmtojwwvefxfibkeikekvjrfnetmqzmqmqxiwbo";
        private string apiUrl = "https://api.siliconflow.cn/v1/chat/completions";

        void Start()
        {
            SendMessageToDeepSeek("��ð�", null);
        }

        public void SendMessageToDeepSeek(string message, UnityAction<string> callback)
        {
            StartCoroutine(PostRequest(message, callback));
        }

        IEnumerator PostRequest(string message, UnityAction<string> callback)
        {
            //������������������
            var requestBody = new
            {
                model = "deepseek-ai/DeepSeek-V3",
                messages = new[]
                {
                new { role = "user", content = message }
            }
            };
            //ʹ��Newtonsoft.Json���л�
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            Debug.Log(jsonBody);
            //yield return null;
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");   //�����ϴ�������
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);   //�������ش�����

            yield return request.SendWebRequest();
            Debug.Log("Request status code: " + request.responseCode);
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);    //��ӡ��ϸ������Ϣ
            }
            else
            {
                //������Ӧ
                string responseJson = request.downloadHandler.text;
                Debug.Log("Response: " + responseJson);
            }
        }
    }
}
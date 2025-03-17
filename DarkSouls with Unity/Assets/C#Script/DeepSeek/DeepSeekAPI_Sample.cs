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
            SendMessageToDeepSeek("你好啊", null);
        }

        public void SendMessageToDeepSeek(string message, UnityAction<string> callback)
        {
            StartCoroutine(PostRequest(message, callback));
        }

        IEnumerator PostRequest(string message, UnityAction<string> callback)
        {
            //创建匿名类型请求体
            var requestBody = new
            {
                model = "deepseek-ai/DeepSeek-V3",
                messages = new[]
                {
                new { role = "user", content = message }
            }
            };
            //使用Newtonsoft.Json序列化
            string jsonBody = JsonConvert.SerializeObject(requestBody);
            Debug.Log(jsonBody);
            //yield return null;
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");   //设置上传处理器
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);   //设置下载处理器

            yield return request.SendWebRequest();
            Debug.Log("Request status code: " + request.responseCode);
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);    //打印详细错误信息
            }
            else
            {
                //处理响应
                string responseJson = request.downloadHandler.text;
                Debug.Log("Response: " + responseJson);
            }
        }
    }
}
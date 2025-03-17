using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace SG
{
    public class DeepSeekAPI : MonoBehaviour
    {
        [Header("API Setting")]
        [SerializeField]
        private string apiKey = "sk-cvncosdvyvmtojwwvefxfibkeikekvjrfnetmqzmqmqxiwbo";
        [SerializeField]
        private string modelName = "deepseek-ai/DeepSeek-V3";  //使用的模型名称
        [SerializeField]
        private string apiUrl = "https://api.siliconflow.cn/v1/chat/completions";

        //对话参数
        [Header("Dialogue Settings")]
        [Range(0, 2)] public float temperature = 0.5f;  //控制生成文本的随机性（0-2，值越高越随机）
        [Range(1, 1000)] public int maxTokens = 100;    //生成的最大令牌数（控制回复的长度）1个token约等于1-2个字符，或0.75个单词

        [System.Serializable]
        public class NPCCharacter
        {
            public string name = "Fire Keeper";
            [TextArea(3, 10)]
            public string personalityPrompt = "你是《黑暗之魂3》中的防火女，熟悉游戏世界观，并且把提问的人默认为灰烬";
        }
        [SerializeField] public NPCCharacter npcCharacter;

        //回调委托，用于异步处理API响应
        public delegate void DialogueCallback(string content, bool isSuccess);


        void Start()
        {
            //SendMessageToDeepSeek("你好啊", null);
        }

        public void SendMessageToDeepSeek(string message, DialogueCallback callback)
        {
            StartCoroutine(PostRequest(message, callback));
        }


        /// <summary>
        /// 处理对话请求的协程
        /// </summary>
        /// <param name="message">玩家的输入内容</param>
        /// <param name="callback">回调函数，用于处理API响应</param>
        /// <returns></returns>
        IEnumerator PostRequest(string message, DialogueCallback callback)
        {
            //构建消息列表，包含系统提示和用户输入
            List<Message> messages = new List<Message>
        {
            new Message { role = "system", content = npcCharacter.personalityPrompt },  //系统角色设定
            new Message { role = "user", content = message }    //用户输入
        };

            //构建请求体
            ChatRequest requestBody = new ChatRequest
            {
                model = modelName,        //模型名称
                messages = messages,        //消息列表
                temperature = temperature,      //温度参数
                max_tokens = maxTokens          //最大令牌数
            };

            //使用Newtonsoft.Json序列化，可以请求匿名类，Unity自带的Json无法转换匿名类
            //string jsonBody = JsonConvert.SerializeObject(requestBody);
            string jsonBody = JsonUtility.ToJson(requestBody);

            Debug.Log(jsonBody);
            //yield return null;/

            //创建UnityWebRequest
            UnityWebRequest request = CreateWebRequest(jsonBody);
            yield return request.SendWebRequest();

            if (IsRequestError(request))
            {
                if (request.responseCode == 429)     //速率限制
                {
                    Debug.LogWarning("速率限制，延迟重试中...");
                    yield return new WaitForSeconds(5);     //延迟5秒后重试
                    StartCoroutine(PostRequest(message, callback));
                    yield break;
                }
                else
                {
                    Debug.Log("Request status code: " + request.responseCode);
                    Debug.LogError($"API Error: {request.responseCode}\n{request.downloadHandler.text}");
                    callback?.Invoke($"API请求失败:{request.downloadHandler.text}", false);
                    yield break;
                }
            }

            Debug.Log(request.downloadHandler.text);
            DeepSeekResponse response = ParseResponse(request.downloadHandler.text);

            if (response != null && response.choices.Length > 0)
            {
                Debug.Log("Reply " + request.downloadHandler.text);
                string npcReply = response.choices[0].message.content;
                Debug.Log(npcReply);
                callback?.Invoke(npcReply, true);
            }
            else
            {
                callback?.Invoke(name + "（陷入沉默）", false);
            }
            request.Dispose();      //确保释放UnityWebRequest，否则可能出现GC相关报错
        }

        /// <summary>
        /// 创建UnityWebRequest对象
        /// </summary>
        /// <param name="jsonBody">请求体的JSON字符串</param>
        /// <returns>配置好的UnityWebRequest对象</returns>
        private UnityWebRequest CreateWebRequest(string jsonBody)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);      //设置上传处理器
            request.downloadHandler = new DownloadHandlerBuffer();          //设置下载处理器
            request.SetRequestHeader("Content-Type", "application/json");   //设置请求头
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);      //设置认证头
            request.SetRequestHeader("Accept", "application/json");         //设置接受类型
            return request;
        }

        /// <summary>
        /// 请求是否出错
        /// </summary>
        /// <param name="request">UnityWebRequest对象</param>
        /// <returns>如果请求出错返回true，否则返回false</returns>
        private bool IsRequestError(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError;
        }

        /// <summary>
        /// 解析API响应
        /// </summary>
        /// <param name="jsonResponse">API响应的JSON字符串</param>
        /// <returns>解析后的DeepSeekResponse对象</returns>
        private DeepSeekResponse ParseResponse(string jsonResponse)
        {
            try
            {
                DeepSeekResponse response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                if (response == null || response.choices == null || response.choices.Length == 0)
                {
                    Debug.LogError("API响应格式错误或未包含有效数据");
                    return null;
                }
                return response;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON解析失败:{e.Message}\n相应内容:{jsonResponse}");
                return null;
            }
        }

        [System.Serializable]
        private class ChatRequest
        {
            public string model;        //模型名称
            public List<Message> messages;      //消息列表
            public float temperature;       //温度参数
            public int max_tokens;      //最大令牌数
        }

        [System.Serializable]
        public class Message
        {
            public string role;             //角色(system/user/assistant) 预设身份/发送消息的人/接收消息
            public string content;      //消息内容
                                        //public string reasoning_content;        //思考内容  R1模型的推理内容
        }

        [System.Serializable]
        private class Choice
        {
            public Message message;        //生成的消息
        }

        [System.Serializable]
        private class DeepSeekResponse
        {
            public Choice[] choices;        //生成的选择列表
        }
    }
}
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
        private string modelName = "deepseek-ai/DeepSeek-V3";  //ʹ�õ�ģ������
        [SerializeField]
        private string apiUrl = "https://api.siliconflow.cn/v1/chat/completions";

        //�Ի�����
        [Header("Dialogue Settings")]
        [Range(0, 2)] public float temperature = 0.5f;  //���������ı�������ԣ�0-2��ֵԽ��Խ�����
        [Range(1, 1000)] public int maxTokens = 100;    //���ɵ���������������ƻظ��ĳ��ȣ�1��tokenԼ����1-2���ַ�����0.75������

        [System.Serializable]
        public class NPCCharacter
        {
            public string name = "Fire Keeper";
            [TextArea(3, 10)]
            public string personalityPrompt = "���ǡ��ڰ�֮��3���еķ���Ů����Ϥ��Ϸ����ۣ����Ұ����ʵ���Ĭ��Ϊ�ҽ�";
        }
        [SerializeField] public NPCCharacter npcCharacter;

        //�ص�ί�У������첽����API��Ӧ
        public delegate void DialogueCallback(string content, bool isSuccess);


        void Start()
        {
            //SendMessageToDeepSeek("��ð�", null);
        }

        public void SendMessageToDeepSeek(string message, DialogueCallback callback)
        {
            StartCoroutine(PostRequest(message, callback));
        }


        /// <summary>
        /// ����Ի������Э��
        /// </summary>
        /// <param name="message">��ҵ���������</param>
        /// <param name="callback">�ص����������ڴ���API��Ӧ</param>
        /// <returns></returns>
        IEnumerator PostRequest(string message, DialogueCallback callback)
        {
            //������Ϣ�б�����ϵͳ��ʾ���û�����
            List<Message> messages = new List<Message>
        {
            new Message { role = "system", content = npcCharacter.personalityPrompt },  //ϵͳ��ɫ�趨
            new Message { role = "user", content = message }    //�û�����
        };

            //����������
            ChatRequest requestBody = new ChatRequest
            {
                model = modelName,        //ģ������
                messages = messages,        //��Ϣ�б�
                temperature = temperature,      //�¶Ȳ���
                max_tokens = maxTokens          //���������
            };

            //ʹ��Newtonsoft.Json���л����������������࣬Unity�Դ���Json�޷�ת��������
            //string jsonBody = JsonConvert.SerializeObject(requestBody);
            string jsonBody = JsonUtility.ToJson(requestBody);

            Debug.Log(jsonBody);
            //yield return null;/

            //����UnityWebRequest
            UnityWebRequest request = CreateWebRequest(jsonBody);
            yield return request.SendWebRequest();

            if (IsRequestError(request))
            {
                if (request.responseCode == 429)     //��������
                {
                    Debug.LogWarning("�������ƣ��ӳ�������...");
                    yield return new WaitForSeconds(5);     //�ӳ�5�������
                    StartCoroutine(PostRequest(message, callback));
                    yield break;
                }
                else
                {
                    Debug.Log("Request status code: " + request.responseCode);
                    Debug.LogError($"API Error: {request.responseCode}\n{request.downloadHandler.text}");
                    callback?.Invoke($"API����ʧ��:{request.downloadHandler.text}", false);
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
                callback?.Invoke(name + "�������Ĭ��", false);
            }
            request.Dispose();      //ȷ���ͷ�UnityWebRequest��������ܳ���GC��ر���
        }

        /// <summary>
        /// ����UnityWebRequest����
        /// </summary>
        /// <param name="jsonBody">�������JSON�ַ���</param>
        /// <returns>���úõ�UnityWebRequest����</returns>
        private UnityWebRequest CreateWebRequest(string jsonBody)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);      //�����ϴ�������
            request.downloadHandler = new DownloadHandlerBuffer();          //�������ش�����
            request.SetRequestHeader("Content-Type", "application/json");   //��������ͷ
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);      //������֤ͷ
            request.SetRequestHeader("Accept", "application/json");         //���ý�������
            return request;
        }

        /// <summary>
        /// �����Ƿ����
        /// </summary>
        /// <param name="request">UnityWebRequest����</param>
        /// <returns>������������true�����򷵻�false</returns>
        private bool IsRequestError(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError ||
                request.result == UnityWebRequest.Result.DataProcessingError;
        }

        /// <summary>
        /// ����API��Ӧ
        /// </summary>
        /// <param name="jsonResponse">API��Ӧ��JSON�ַ���</param>
        /// <returns>�������DeepSeekResponse����</returns>
        private DeepSeekResponse ParseResponse(string jsonResponse)
        {
            try
            {
                DeepSeekResponse response = JsonUtility.FromJson<DeepSeekResponse>(jsonResponse);
                if (response == null || response.choices == null || response.choices.Length == 0)
                {
                    Debug.LogError("API��Ӧ��ʽ�����δ������Ч����");
                    return null;
                }
                return response;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON����ʧ��:{e.Message}\n��Ӧ����:{jsonResponse}");
                return null;
            }
        }

        [System.Serializable]
        private class ChatRequest
        {
            public string model;        //ģ������
            public List<Message> messages;      //��Ϣ�б�
            public float temperature;       //�¶Ȳ���
            public int max_tokens;      //���������
        }

        [System.Serializable]
        public class Message
        {
            public string role;             //��ɫ(system/user/assistant) Ԥ�����/������Ϣ����/������Ϣ
            public string content;      //��Ϣ����
                                        //public string reasoning_content;        //˼������  R1ģ�͵���������
        }

        [System.Serializable]
        private class Choice
        {
            public Message message;        //���ɵ���Ϣ
        }

        [System.Serializable]
        private class DeepSeekResponse
        {
            public Choice[] choices;        //���ɵ�ѡ���б�
        }
    }
}
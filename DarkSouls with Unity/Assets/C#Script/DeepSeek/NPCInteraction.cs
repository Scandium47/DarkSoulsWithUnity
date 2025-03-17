using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;

namespace SG
{
    public class NPCInteraction : MonoBehaviour
    {
        //���ú�����
        [Header("References")]
        [SerializeField] private DeepSeekAPI deepSeekAPI;   //�Ի�������
        [SerializeField] private TMP_InputField inputField;     //��������
        [SerializeField] private TextMeshProUGUI dialogueText;          //�ظ����ı�����
        private string characterName;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.05f;     //���ֻ�Ч�����ַ���ʾ�ٶ�

        [SerializeField] private GameObject loadingIndicator;       //���ؽ���

        private void Start()
        {
            characterName = deepSeekAPI.npcCharacter.name;  //��ɫ���ָ�ֵ
            inputField.onSubmit.AddListener((text) =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    Debug.LogWarning("��������Ϊ�գ����������롣");
                    return;
                }
                inputField.text = "";   //��������
                loadingIndicator.SetActive(true);
                deepSeekAPI.SendMessageToDeepSeek(text, HandleAIResponse);      //���ͶԻ�����DeepSeek AI
            });
        }

        /// <summary>
        /// ����AI����Ӧ
        /// </summary>
        /// <param name="content">AI�Ļظ�����</param>
        /// <param name="isSuccess">�����Ƿ�ɹ�</param>
        private void HandleAIResponse(string content, bool isSuccess)
        {
            StopAllCoroutines();
            string message = content;
            StartCoroutine(TypewriterEffect(isSuccess ? characterName + ":" + message : characterName + ":������Ů.exe����Ӧ��"));        //�������ֻ�Ч��Э��
        }

        /// <summary>
        /// ���ֻ�Ч��Э��
        /// </summary>
        /// <param name="text">��ɫ�Ļظ�����</param>
        /// <returns></returns>
        private IEnumerator TypewriterEffect(string text)
        {
            loadingIndicator.SetActive(false);

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)        //����ÿ���ַ�
            {
                sb.Append(c);
                dialogueText.text = sb.ToString();
                yield return new WaitForSeconds(typingSpeed);   //�ȴ�һ��ʱ��
            }
        }
    }
}
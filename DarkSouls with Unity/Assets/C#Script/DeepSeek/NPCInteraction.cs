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
        //引用和配置
        [Header("References")]
        [SerializeField] private DeepSeekAPI deepSeekAPI;   //对话管理器
        [SerializeField] private TMP_InputField inputField;     //玩家输入框
        [SerializeField] private TextMeshProUGUI dialogueText;          //回复的文本内容
        private string characterName;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.05f;     //打字机效果的字符显示速度

        [SerializeField] private GameObject loadingIndicator;       //加载界面

        private void Start()
        {
            characterName = deepSeekAPI.npcCharacter.name;  //角色名字赋值
            inputField.onSubmit.AddListener((text) =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    Debug.LogWarning("输入内容为空，请重新输入。");
                    return;
                }
                inputField.text = "";   //清空输入框
                loadingIndicator.SetActive(true);
                deepSeekAPI.SendMessageToDeepSeek(text, HandleAIResponse);      //发送对话请求到DeepSeek AI
            });
        }

        /// <summary>
        /// 处理AI的响应
        /// </summary>
        /// <param name="content">AI的回复内容</param>
        /// <param name="isSuccess">请求是否成功</param>
        private void HandleAIResponse(string content, bool isSuccess)
        {
            StopAllCoroutines();
            string message = content;
            StartCoroutine(TypewriterEffect(isSuccess ? characterName + ":" + message : characterName + ":（防火女.exe无响应）"));        //启动打字机效果协程
        }

        /// <summary>
        /// 打字机效果协程
        /// </summary>
        /// <param name="text">角色的回复内容</param>
        /// <returns></returns>
        private IEnumerator TypewriterEffect(string text)
        {
            loadingIndicator.SetActive(false);

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)        //遍历每个字符
            {
                sb.Append(c);
                dialogueText.text = sb.ToString();
                yield return new WaitForSeconds(typingSpeed);   //等待一段时间
            }
        }
    }
}
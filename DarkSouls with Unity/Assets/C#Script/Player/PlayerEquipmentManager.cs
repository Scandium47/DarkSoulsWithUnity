using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        PlayerManager player;

        [Header("Equipment Model Changers")]
        HelmetModelChanger helmetModelChanger;  //头盔

        TrosoModelChanger torsoModelChanger;    //胸甲
        UpperLeftArmModelChanger upperLeftArmModelChanger;  //左肩甲
        UpperRightArmModelChanger upperRightArmModelChanger;    //右肩甲

        LowerLeftArmModelChanger lowerLeftArmModelChanger;  //左臂甲
        LowerRightArmModelChanger lowerRightArmModelChanger;    //右臂甲
        LeftHandModelChanger leftHandModelChanger;  //左手甲
        RightHandModelChanger rightHandModelChanger;    //右手甲

        HipModelChanger hipModelChanger;    //腰甲
        LeftLegModelChanger leftLegModelChanger;    //左腿甲
        RightLegModelChanger rightLegModelChanger;  //右腿甲


        [Header("Default Naked Models")]
        //头盔未装备引用 默认头 和 默认头发（gameobject)
        public GameObject nakedHairModel;
        public GameObject nakedHeadModel;

        public string nakedTorsoModel;
        public string nakedUpperLeftArmModel;
        public string nakedUpperRightArmModel;

        public string nakedLowerLeftArmModel;
        public string nakedLowerRightArmModel;
        public string nakedLeftHandModel;
        public string nakedRightHandModel;

        public string nakedHipModel;
        public string nakedLeftLeg;
        public string nakedRightLeg;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();

            helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
            torsoModelChanger = GetComponentInChildren<TrosoModelChanger>();
            hipModelChanger = GetComponentInChildren<HipModelChanger>();
            leftLegModelChanger = GetComponentInChildren<LeftLegModelChanger>();
            rightLegModelChanger = GetComponentInChildren<RightLegModelChanger>();
            upperLeftArmModelChanger = GetComponentInChildren<UpperLeftArmModelChanger>();
            upperRightArmModelChanger= GetComponentInChildren<UpperRightArmModelChanger>();
            lowerLeftArmModelChanger = GetComponentInChildren<LowerLeftArmModelChanger>();
            lowerRightArmModelChanger = GetComponentInChildren<LowerRightArmModelChanger>();
            leftHandModelChanger = GetComponentInChildren<LeftHandModelChanger>();
            rightHandModelChanger= GetComponentInChildren<RightHandModelChanger>();
        }

        private void Start()
        {
            EquipAllArmor();
        }

        public void EquipAllArmor()
        {
            float poisonResistance = 0;
            float totalEquipmentLoad = 0;
            //只是调用来重置装备
            helmetModelChanger.UnEquipAllHelmetModles();

            //头盔
            if (player.playerInventoryManager.currentHelmetEquipment != null)
            {
                nakedHairModel.SetActive(false);
                nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(player.playerInventoryManager.currentHelmetEquipment.helmetModelName);
                player.playerStatsManager.physicalDamageAbsorptionHead = player.playerInventoryManager.currentHandEquipment.physicalDefense;
                //毒抗性计算
                poisonResistance += player.playerInventoryManager.currentHelmetEquipment.poisonResistance;
                //Debug.Log("Head Absorption is " + playerStats.physicalDamageAbsorptionHead + "%");

                totalEquipmentLoad += player.playerInventoryManager.currentHelmetEquipment.weight;
            }
            else
            {
                //默认头和默认头发
                nakedHairModel.SetActive(true);
                nakedHeadModel.SetActive(true);
                player.playerStatsManager.physicalDamageAbsorptionHead = 0;
            }

            //胸甲
            torsoModelChanger.UnEquipAllTorsoModles();
            upperLeftArmModelChanger.UnEquipAllUpperModles();
            upperRightArmModelChanger.UnEquipAllUpperModles();

            if(player.playerInventoryManager.currentBodyEquipment != null)
            {
                torsoModelChanger.EquipTorsoModelByName(player.playerInventoryManager.currentBodyEquipment.torsoModelName);
                upperLeftArmModelChanger.EquipUpperModelByName(player.playerInventoryManager.currentBodyEquipment.upperLeftArmModelName);
                upperRightArmModelChanger.EquipUpperModelByName(player.playerInventoryManager.currentBodyEquipment.upperRightArmModelName);
                player.playerStatsManager.physicalDamageAbsorptionBody = player.playerInventoryManager.currentBodyEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentBodyEquipment.poisonResistance;
                //Debug.Log("Body Absorption is " + playerStats.physicalDamageAbsorptionBody + "%");

                totalEquipmentLoad += player.playerInventoryManager.currentBodyEquipment.weight;
            }
            else
            {
                torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
                upperLeftArmModelChanger.EquipUpperModelByName(nakedUpperLeftArmModel);
                upperRightArmModelChanger.EquipUpperModelByName(nakedUpperRightArmModel);
                player.playerStatsManager.physicalDamageAbsorptionBody = 0;
            }

            //手甲
            lowerLeftArmModelChanger.UnEquipAllModles();
            lowerRightArmModelChanger.UnEquipAllModles();
            leftHandModelChanger.UnEquipAllModles();
            rightHandModelChanger.UnEquipAllModles();

            if(player.playerInventoryManager.currentHandEquipment != null)
            {
                lowerLeftArmModelChanger.EquipModelByName(player.playerInventoryManager.currentHandEquipment.lowerLeftArmModelName);
                lowerRightArmModelChanger.EquipModelByName(player.playerInventoryManager.currentHandEquipment.lowerRightArmModelName);
                leftHandModelChanger.EquipModelByName(player.playerInventoryManager.currentHandEquipment.leftHandModelName);
                rightHandModelChanger.EquipModelByName(player.playerInventoryManager.currentHandEquipment.rightHandModelName);
                player.playerStatsManager.physicalDamageAbsorptionHands = player.playerInventoryManager.currentHandEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentHandEquipment.poisonResistance;
                //Debug.Log("Hands Absorption is " + playerStats.physicalDamageAbsorptionHands + "%");

                totalEquipmentLoad += player.playerInventoryManager.currentHandEquipment.weight;
            }
            else
            {
                lowerLeftArmModelChanger.EquipModelByName(nakedLowerLeftArmModel);
                lowerRightArmModelChanger.EquipModelByName(nakedLowerRightArmModel);
                leftHandModelChanger.EquipModelByName(nakedLeftHandModel);
                rightHandModelChanger.EquipModelByName(nakedRightHandModel);
                player.playerStatsManager.physicalDamageAbsorptionHands = 0;
            }

            //腿甲
            hipModelChanger.UnEquipAllHipModles();
            leftLegModelChanger.UnEquipAllLegModles();
            rightLegModelChanger.UnEquipAllLegModles();

            if(player.playerInventoryManager.currentLegEquipment != null)
            {
                hipModelChanger.EquipHipModelByName(player.playerInventoryManager.currentLegEquipment.hipModelName);
                leftLegModelChanger.EquipLegModelByName(player.playerInventoryManager.currentLegEquipment.leftLegName);
                rightLegModelChanger.EquipLegModelByName(player.playerInventoryManager.currentLegEquipment.rightLegName);
                player.playerStatsManager.physicalDamageAbsorptionLegs = player.playerInventoryManager.currentLegEquipment.physicalDefense;
                poisonResistance += player.playerInventoryManager.currentLegEquipment.poisonResistance;
                //Debug.Log("Legs Absorption is " + playerStats.physicalDamageAbsorptionLegs + "%");

                totalEquipmentLoad += player.playerInventoryManager.currentLegEquipment.weight;
            }
            else
            {
                hipModelChanger.EquipHipModelByName(nakedHipModel);
                leftLegModelChanger.EquipLegModelByName(nakedLeftLeg);
                rightLegModelChanger.EquipLegModelByName(nakedRightLeg);
                player.playerStatsManager.physicalDamageAbsorptionLegs = 0;
            }

            //将所有装备的韧性总和
            player.playerStatsManager.poisonResistance = poisonResistance;
            //计算承重并设置正确的状态
            player.playerStatsManager.CalculateAndSetMaxEquipload();
            player.playerStatsManager.CalculateAndSetCurrentEquipLoad(totalEquipmentLoad);
        }
    }
}
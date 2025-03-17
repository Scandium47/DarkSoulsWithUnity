using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        PlayerManager player;

        [Header("Equipment Model Changers")]
        HelmetModelChanger helmetModelChanger;  //ͷ��

        TrosoModelChanger torsoModelChanger;    //�ؼ�
        UpperLeftArmModelChanger upperLeftArmModelChanger;  //����
        UpperRightArmModelChanger upperRightArmModelChanger;    //�Ҽ��

        LowerLeftArmModelChanger lowerLeftArmModelChanger;  //��ۼ�
        LowerRightArmModelChanger lowerRightArmModelChanger;    //�ұۼ�
        LeftHandModelChanger leftHandModelChanger;  //���ּ�
        RightHandModelChanger rightHandModelChanger;    //���ּ�

        HipModelChanger hipModelChanger;    //����
        LeftLegModelChanger leftLegModelChanger;    //���ȼ�
        RightLegModelChanger rightLegModelChanger;  //���ȼ�


        [Header("Default Naked Models")]
        //ͷ��δװ������ Ĭ��ͷ �� Ĭ��ͷ����gameobject)
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
            //ֻ�ǵ���������װ��
            helmetModelChanger.UnEquipAllHelmetModles();

            //ͷ��
            if (player.playerInventoryManager.currentHelmetEquipment != null)
            {
                nakedHairModel.SetActive(false);
                nakedHeadModel.SetActive(false);
                helmetModelChanger.EquipHelmetModelByName(player.playerInventoryManager.currentHelmetEquipment.helmetModelName);
                player.playerStatsManager.physicalDamageAbsorptionHead = player.playerInventoryManager.currentHandEquipment.physicalDefense;
                //�����Լ���
                poisonResistance += player.playerInventoryManager.currentHelmetEquipment.poisonResistance;
                //Debug.Log("Head Absorption is " + playerStats.physicalDamageAbsorptionHead + "%");

                totalEquipmentLoad += player.playerInventoryManager.currentHelmetEquipment.weight;
            }
            else
            {
                //Ĭ��ͷ��Ĭ��ͷ��
                nakedHairModel.SetActive(true);
                nakedHeadModel.SetActive(true);
                player.playerStatsManager.physicalDamageAbsorptionHead = 0;
            }

            //�ؼ�
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

            //�ּ�
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

            //�ȼ�
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

            //������װ���������ܺ�
            player.playerStatsManager.poisonResistance = poisonResistance;
            //������ز�������ȷ��״̬
            player.playerStatsManager.CalculateAndSetMaxEquipload();
            player.playerStatsManager.CalculateAndSetCurrentEquipLoad(totalEquipmentLoad);
        }
    }
}
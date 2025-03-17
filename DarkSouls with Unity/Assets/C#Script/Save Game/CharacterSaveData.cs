using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [System.Serializable]
    public class CharacterSaveData
    {
        public string characterName;

        public int characterLevel;

        [Header("Equipment")]
        public int currentRightHandWeaponID;
        public int currentLeftHandWeaponID;

        public int currentHeadGearItemID;
        public int currentChestGearItemID;
        public int currentLegGearItemID;
        public int currentHandGearItemID;

        public int nextItemPickUpId = 80001;

        //��������
        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Items Looted From World")]
        public SerializbleDictionary<int, bool> itemsInWorld;       //int -> ��ƷID��bool -> ��Ʒ�Ƿ��ѹ�

        //�޲ι��캯��
        public CharacterSaveData()
        {
            itemsInWorld = new SerializbleDictionary<int, bool>();
        }

        //ID������
        public int GetNextItemPickUpId()
        {
            int currentId = nextItemPickUpId;
            nextItemPickUpId++;
            return currentId;
        }
    }
}
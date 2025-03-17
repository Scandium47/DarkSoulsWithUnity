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

        //世界坐标
        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Items Looted From World")]
        public SerializbleDictionary<int, bool> itemsInWorld;       //int -> 物品ID，bool -> 物品是否被搜刮

        //无参构造函数
        public CharacterSaveData()
        {
            itemsInWorld = new SerializbleDictionary<int, bool>();
        }

        //ID计数器
        public int GetNextItemPickUpId()
        {
            int currentId = nextItemPickUpId;
            nextItemPickUpId++;
            return currentId;
        }
    }
}
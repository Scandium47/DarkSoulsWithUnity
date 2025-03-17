using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Ammo")]
    public class RangedAmmoItem : Item
    {
        [Header("Ammo Type")]
        public AmmoType ammoType;

        [Header("Ammo Velocity")]
        public float forwardVelocity = 550;
        public float upwardVelocity = 0;
        public float ammoMass = 0;  //弹药质量
        public bool useGravity = false;

        [Header("Ammo Capacity")]
        public int carryLimit = 99;
        public int currentAmount = 99;

        [Header("Ammo Base Damage")]
        public int physicalDamage = 50;
        //法术，火，暗，光伤害

        [Header("ITem Models")]
        public GameObject loadedItemModel;  //只是拉弓时候的模型，无碰撞器刚体
        public GameObject liveAmmoModel;    //实际有碰撞器的模型
        public GameObject penetratedModel;    //实例化在碰撞器或者游戏场景内的（插在人身上，插在墙上的断箭）
    }
}
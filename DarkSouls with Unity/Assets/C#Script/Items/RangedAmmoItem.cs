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
        public float ammoMass = 0;  //��ҩ����
        public bool useGravity = false;

        [Header("Ammo Capacity")]
        public int carryLimit = 99;
        public int currentAmount = 99;

        [Header("Ammo Base Damage")]
        public int physicalDamage = 50;
        //�������𣬰������˺�

        [Header("ITem Models")]
        public GameObject loadedItemModel;  //ֻ������ʱ���ģ�ͣ�����ײ������
        public GameObject liveAmmoModel;    //ʵ������ײ����ģ��
        public GameObject penetratedModel;    //ʵ��������ײ��������Ϸ�����ڵģ����������ϣ�����ǽ�ϵĶϼ���
    }
}
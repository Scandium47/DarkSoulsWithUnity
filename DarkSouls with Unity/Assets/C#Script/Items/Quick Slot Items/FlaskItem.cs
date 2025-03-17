using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Consumables/Flask")]
    public class FlaskItem : ConsumableItem
    {
        [Header("Flask Type")]
        public bool estusFlask;
        public bool ashenFlask;

        [Header("Recovery Amount")]
        public int healthRecoverAmount;
        public int focusPointsRecoverAmount;

        [Header("Recovery FX")]
        public GameObject recoveryFX;

        public override void AttemtToConsumeItem(PlayerManager player)
        {
            base.AttemtToConsumeItem(player);
            //Instantiate flask in hand and play drink anim ʵ����ԭ��ƿ/���ź�ҩ������
            GameObject flask = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            //play recovery FX when/if we drink without being hit �ָ���Ч û�����ʱ���ҩ
            player.playerEffectsManager.currentParticleFX = recoveryFX;

            #region ����
            //Add health or FP ��Ѫ
            player.playerEffectsManager.amountToBeHealed = healthRecoverAmount;
            #endregion

            player.playerEffectsManager.instantiatedFXModel = flask;
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();     //�����������
        }
    }
}
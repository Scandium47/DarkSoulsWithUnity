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
            //Instantiate flask in hand and play drink anim 实例化原素瓶/播放喝药动画↑
            GameObject flask = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            //play recovery FX when/if we drink without being hit 恢复特效 没被打的时候喝药
            player.playerEffectsManager.currentParticleFX = recoveryFX;

            #region 作用
            //Add health or FP 加血
            player.playerEffectsManager.amountToBeHealed = healthRecoverAmount;
            #endregion

            player.playerEffectsManager.instantiatedFXModel = flask;
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();     //解除右手武器
        }
    }
}
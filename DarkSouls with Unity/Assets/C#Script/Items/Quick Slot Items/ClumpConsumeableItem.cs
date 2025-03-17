using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Consumables/Cure Effect Clump")]
    public class ClumpConsumeableItem : ConsumableItem
    {
        [Header("Recovery FX")]
        public GameObject clumpConsumeFX;

        [Header("Cure FX")]
        public bool curePoison;
        //冰冻
        //流血
        //咒死

        public override void AttemtToConsumeItem(PlayerManager player)
        {
            base.AttemtToConsumeItem(player);
            GameObject clump = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            player.playerEffectsManager.currentParticleFX = clumpConsumeFX;
            player.playerEffectsManager.instantiatedFXModel = clump;

            #region 作用
            if (curePoison)
            {
                player.playerStatsManager.poisonBuildup = 0;
                player.playerStatsManager.isPoisoned = false;

                //if(player.playerEffectsManager.currentPoisonParticleFX != null)
                //{
                //    //等待两秒再销毁
                //    player.playerEffectsManager.StartCoroutine(DelayedDestroy(player.playerEffectsManager));
                //}
            }
            #endregion

            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }

        //IEnumerator DelayedDestroy(PlayerEffectsManager playerEffectsManager)
        //{
        //    // 等待 2 秒
        //    yield return new WaitForSeconds(2f);
        //    Destroy(playerEffectsManager.currentPoisonParticleFX);
        //}
    }
}
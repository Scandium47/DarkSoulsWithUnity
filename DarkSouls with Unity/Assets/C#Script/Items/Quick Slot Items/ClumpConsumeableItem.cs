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
        //����
        //��Ѫ
        //����

        public override void AttemtToConsumeItem(PlayerManager player)
        {
            base.AttemtToConsumeItem(player);
            GameObject clump = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            player.playerEffectsManager.currentParticleFX = clumpConsumeFX;
            player.playerEffectsManager.instantiatedFXModel = clump;

            #region ����
            if (curePoison)
            {
                player.playerStatsManager.poisonBuildup = 0;
                player.playerStatsManager.isPoisoned = false;

                //if(player.playerEffectsManager.currentPoisonParticleFX != null)
                //{
                //    //�ȴ�����������
                //    player.playerEffectsManager.StartCoroutine(DelayedDestroy(player.playerEffectsManager));
                //}
            }
            #endregion

            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }

        //IEnumerator DelayedDestroy(PlayerEffectsManager playerEffectsManager)
        //{
        //    // �ȴ� 2 ��
        //    yield return new WaitForSeconds(2f);
        //    Destroy(playerEffectsManager.currentPoisonParticleFX);
        //}
    }
}
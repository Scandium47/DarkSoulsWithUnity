using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Draw Arrow Action")]
    public class DrawArrowAction : ItemAction
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.isInteracting)
                return;

            if (character.isHoldingArrow)
                return;

            //玩家动画
            character.animator.SetBool("isHoldingArrow", true);
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Draw_01", true);

            //实例化弓箭
            GameObject loadedArrow = Instantiate(character.characterInventoryManager.currentAmmo.loadedItemModel, character.characterWeaponSlotManager.leftHandSlot.transform);   //实例化拉弓时候的弓的模型（无碰撞器刚体）
            character.characterEffectsManager.instantiatedFXModel = loadedArrow;

            //弓动画
            Animator bowAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnimator.SetBool("isDrawn", true);
            bowAnimator.Play("Bow_TH_Draw_01");
        }
    }
}
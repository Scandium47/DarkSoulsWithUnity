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

            //��Ҷ���
            character.animator.SetBool("isHoldingArrow", true);
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Draw_01", true);

            //ʵ��������
            GameObject loadedArrow = Instantiate(character.characterInventoryManager.currentAmmo.loadedItemModel, character.characterWeaponSlotManager.leftHandSlot.transform);   //ʵ��������ʱ��Ĺ���ģ�ͣ�����ײ�����壩
            character.characterEffectsManager.instantiatedFXModel = loadedArrow;

            //������
            Animator bowAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnimator.SetBool("isDrawn", true);
            bowAnimator.Play("Bow_TH_Draw_01");
        }
    }
}
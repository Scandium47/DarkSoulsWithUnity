using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ConsumableItem : Item
    {
        [Header("Item Quantity")]
        public int maxItemAmount;
        public int currentItemAmount;

        [Header("Item Model")]
        public GameObject itemModel;

        [Header("Animations")]
        public string consumeAnimation;
        public bool isInteracting;

        [Header("Items Description")]
        [TextArea]
        public string itemsDescription;

        public virtual void AttemtToConsumeItem(PlayerManager player)
        {
            if (currentItemAmount > 0)
            {
                player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, isInteracting, true);
            }
            else
            {
                player.playerAnimatorManager.PlayTargetAnimation("Drink_Empty", true);
            }
        }

        public virtual void SucessfullyConsumeItem(PlayerManager player)
        {
            currentItemAmount = currentItemAmount - 1;
        }

        public virtual bool CanIUseThisItem(PlayerManager player)
        {
            return true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Item Actions/Parry Action")]
    public class ParryAction : ItemAction
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.isInteracting)
                return;

            character.characterAnimatorManager.EraseHandIKForWeapon();

            WeaponItem parryingWeapon = character.characterInventoryManager.currentItemBeingUsed as WeaponItem;

            //��ⵯ������ �� С�ܻ����ж�
            if(parryingWeapon.weaponType == WeaponType.SmallShield)
            {
                //���ٶܷ�
                character.characterAnimatorManager.PlayTargetAnimation("Parry",  true);
            }
            else if(parryingWeapon.weaponType == WeaponType.Shield)
            {
                //�ܷ�
                character.characterAnimatorManager.PlayTargetAnimation("Parry", true);
            }
        }
    }
}
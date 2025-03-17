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

            //检测弹反武器 是 小盾还是中盾
            if(parryingWeapon.weaponType == WeaponType.SmallShield)
            {
                //快速盾反
                character.characterAnimatorManager.PlayTargetAnimation("Parry",  true);
            }
            else if(parryingWeapon.weaponType == WeaponType.Shield)
            {
                //盾反
                character.characterAnimatorManager.PlayTargetAnimation("Parry", true);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Ring")]
    public class RingItem : Item
    {
        //在脚本化对象上编辑内容，会改变其内容，这里实例化一个克隆以免内容被修改
        [SerializeField] StaticCharacterEffect effect;
        private StaticCharacterEffect effectClone;

        [Header("Item Effect Description")]
        [TextArea] public string itemEffectInformation;

        //装备戒指时调用，添加戒指效果
        public void EquipRing(CharacterManager character)
        {
            //克隆一个实例化对象，以便于可编写脚本的对象不受影响
            effectClone = Instantiate(effect);

            character.characterEffectsManager.AddStaticEffect(effectClone);
        }   
        
        //脱下戒指时调用，解除戒指效果
        public void UnEquipRing(CharacterManager character)
        {
            character.characterEffectsManager.RemoveStaticEffect(effect.effectID);
        }
    }
}
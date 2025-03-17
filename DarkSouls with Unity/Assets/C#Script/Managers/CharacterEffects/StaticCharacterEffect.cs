using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class StaticCharacterEffect : ScriptableObject
    {
        public int effectID;
        //静态效果，玩家装备戒指之类的加成，装备时添加效果，卸下后移除效果
        public virtual void AddStaticEffect(CharacterManager character)
        {

        }

        public virtual void RemoveStaticEffect(CharacterManager character)
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class StaticCharacterEffect : ScriptableObject
    {
        public int effectID;
        //��̬Ч�������װ����ָ֮��ļӳɣ�װ��ʱ���Ч����ж�º��Ƴ�Ч��
        public virtual void AddStaticEffect(CharacterManager character)
        {

        }

        public virtual void RemoveStaticEffect(CharacterManager character)
        {

        }
    }
}
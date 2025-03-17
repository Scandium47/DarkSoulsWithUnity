using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Items/Ring")]
    public class RingItem : Item
    {
        //�ڽű��������ϱ༭���ݣ���ı������ݣ�����ʵ����һ����¡�������ݱ��޸�
        [SerializeField] StaticCharacterEffect effect;
        private StaticCharacterEffect effectClone;

        [Header("Item Effect Description")]
        [TextArea] public string itemEffectInformation;

        //װ����ָʱ���ã���ӽ�ָЧ��
        public void EquipRing(CharacterManager character)
        {
            //��¡һ��ʵ���������Ա��ڿɱ�д�ű��Ķ�����Ӱ��
            effectClone = Instantiate(effect);

            character.characterEffectsManager.AddStaticEffect(effectClone);
        }   
        
        //���½�ָʱ���ã������ָЧ��
        public void UnEquipRing(CharacterManager character)
        {
            character.characterEffectsManager.RemoveStaticEffect(effect.effectID);
        }
    }
}
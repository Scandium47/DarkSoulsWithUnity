using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Poison Build Up")]
    public class PoisonBuildUpEffect : CharacterEffect
    {
        //ÿ�λ��۵��ж�����
        [SerializeField] float basePoisonBuildUpAmount = 7;
        //��ɫ�ж���Ķ���ʱ��
        [SerializeField] float poisonAmount = 100;
        //�ж���ÿ���յ����˺�
        [SerializeField] int poisonDamagePerTick = 5;
        public override void ProecssEffect(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;

            //���Ƕ�����֮��Ķ��Ի���
            float finalPosionBuildUp = 0;

            if(character.characterStatsManager.poisonResistance >= 0)
            {
                //������Ի��ܵ�100�ˣ���ʱ���߶���
                if(character.characterStatsManager.poisonResistance >= 100)
                {
                    finalPosionBuildUp = 0;
                }
                else
                {
                    float resistancePercentage = character.characterStatsManager.poisonResistance / 100;

                    finalPosionBuildUp = basePoisonBuildUpAmount - (basePoisonBuildUpAmount * resistancePercentage);
                }
            }

            //����ÿ��tick�Ķ��Ի���
            character.characterStatsManager.poisonBuildup += finalPosionBuildUp;

            //�ж������б����Ƴ��ж���ʱ�����
            if (character.characterStatsManager.isPoisoned)
            {
                character.characterEffectsManager.timedEffects.Remove(this);
            }

            //������û���ж�����ÿ��tick���ٶ��������ĵ�characterEffectsManager��ProcessBuildUpDecay�����У�

            //����������۳���100���ж�
            if (character.characterStatsManager.poisonBuildup >= 100)
            {
                character.characterStatsManager.isPoisoned = true;
                character.characterStatsManager.poisonAmount = poisonAmount;
                character.characterStatsManager.poisonBuildup = 0;

                if(player != null)
                {
                    player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(Mathf.RoundToInt(poisonAmount));
                }

                //ÿ��tickʹ�ö�Ҫʵ��������Ҫ��ԭʼ�汾����Ϊ��ȫ�ֶ��Ե����Ч��
                PoisonedEffect poisonedEffect = Instantiate(WorldCharacterEffectsManager.instance.poisonedEffect);
                poisonedEffect.poisonDamage = poisonDamagePerTick;
                character.characterEffectsManager.timedEffects.Add(poisonedEffect);
                character.characterEffectsManager.timedEffects.Remove(this);
                character.characterSoundFXManager.PlaySoundFX(WorldCharacterEffectsManager.instance.poisonSFX);

                character.characterEffectsManager.AddTimedEffectParticle(Instantiate(WorldCharacterEffectsManager.instance.poisonFX));
            }

            character.characterEffectsManager.timedEffects.Remove(this);
        }
    }
}
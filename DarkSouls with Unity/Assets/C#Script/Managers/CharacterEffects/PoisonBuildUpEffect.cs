using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Poison Build Up")]
    public class PoisonBuildUpEffect : CharacterEffect
    {
        //每次积累的中毒条。
        [SerializeField] float basePoisonBuildUpAmount = 7;
        //角色中毒后的毒害时间
        [SerializeField] float poisonAmount = 100;
        //中毒后每次收到的伤害
        [SerializeField] int poisonDamagePerTick = 5;
        public override void ProecssEffect(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;

            //考虑毒抗性之后的毒性积累
            float finalPosionBuildUp = 0;

            if(character.characterStatsManager.poisonResistance >= 0)
            {
                //如果毒性积攒到100了，暂时免疫毒性
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

            //最终每次tick的毒性积累
            character.characterStatsManager.poisonBuildup += finalPosionBuildUp;

            //中毒后在列表中移除中毒的时间计算
            if (character.characterStatsManager.isPoisoned)
            {
                character.characterEffectsManager.timedEffects.Remove(this);
            }

            //如果玩家没有中毒区域，每次tick减少毒条（更改到characterEffectsManager的ProcessBuildUpDecay方法中）

            //如果毒条积累超过100，中毒
            if (character.characterStatsManager.poisonBuildup >= 100)
            {
                character.characterStatsManager.isPoisoned = true;
                character.characterStatsManager.poisonAmount = poisonAmount;
                character.characterStatsManager.poisonBuildup = 0;

                if(player != null)
                {
                    player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(Mathf.RoundToInt(poisonAmount));
                }

                //每次tick使用都要实例化，不要用原始版本，因为会全局都吃到这个效果
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
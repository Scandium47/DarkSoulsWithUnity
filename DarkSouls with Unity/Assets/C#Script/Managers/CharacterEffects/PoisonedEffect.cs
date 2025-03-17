using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Poison")]
    public class PoisonedEffect : CharacterEffect
    {
        public int poisonDamage = 1;

        public override void ProecssEffect(CharacterManager character)
        {
            PlayerManager player = character as PlayerManager;

            if(character.characterStatsManager.isPoisoned)
            {
                if(character.characterStatsManager.poisonAmount > 0)
                {
                    character.characterStatsManager.poisonAmount -= 1;
                    //对玩家造成伤害
                    Debug.Log("Damage");

                    if(player != null)
                    {
                        player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(Mathf.RoundToInt(character.characterStatsManager.poisonAmount));
                    }
                }
                else
                {
                    character.characterStatsManager.isPoisoned = false;
                    character.characterStatsManager.poisonAmount = 0;
                    player.playerEffectsManager.poisonAmountBar.SetPoisonAmount(0);
                }
            }
            else
            {
                character.characterEffectsManager.timedEffects.Remove(this);
                character.characterEffectsManager.RemoveTimedEffectParticle(EffectParticleType.poison);
            }
        }
    }
}
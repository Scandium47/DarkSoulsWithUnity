using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem
    {
        public int healAmount;

        public override void AttemptToCastSpell(CharacterManager character)
        {
            base.AttemptToCastSpell(character);
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, character.transform);
            character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true, false, character.isUsingLeftHand);
            Debug.Log("Attempting to cast spell...");
        }

        public override void SuccessfullyCastSpell(CharacterManager character)
        {
            base.SuccessfullyCastSpell(character);
            GameObject instantiateSpellFX = Instantiate(spellCastFX, character.transform);
            character.characterStatsManager.HealCharacter(healAmount);
            Debug.Log("spell cast successful");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        PlayerManager player;

        PoisonBuildUpBar poisonBuildUpBar;
        public PoisonAmountBar poisonAmountBar;

        public GameObject currentParticleFX;    //The particles that will play of the current effect that is effecting the player(drinking estus, poison ect)
        public int amountToBeHealed;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();

            poisonBuildUpBar = FindObjectOfType<PoisonBuildUpBar>();
            poisonAmountBar = FindObjectOfType<PoisonAmountBar>();
        }

        public void HealPlayerFromEffect()
        {
            player.playerStatsManager.HealCharacter(amountToBeHealed);
            GameObject healParticles = Instantiate(currentParticleFX, player.playerStatsManager.transform);
            Destroy(instantiatedFXModel.gameObject);
            player.playerWeaponSlotManager.LoadBothWeaponsOnSlots();
        }

        protected override void ProcessBuildUpDecay()
        {
            if (player.characterStatsManager.poisonBuildup >= 0)
            {
                player.characterStatsManager.poisonBuildup -= 1;

                //传递到毒性条UI
                poisonBuildUpBar.gameObject.SetActive(true);
                poisonBuildUpBar.SetPoisonBuildUpAmount(Mathf.RoundToInt(player.characterStatsManager.poisonBuildup));
            }
        }
    }
}
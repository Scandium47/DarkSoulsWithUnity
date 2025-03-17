using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class LevelUpInteractable : Interactable
    {
        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            playerManager.uiManager.FireKeeperWindow.SetActive(true);
            playerManager.inputHandler.fireKeeperInventoryFlag = true;
        }
    }
}
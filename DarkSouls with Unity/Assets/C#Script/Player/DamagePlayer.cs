using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DemagePlayer : MonoBehaviour
    {
        private int damage = 25;

        private void OnTriggerEnter(Collider other)
        {
            PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();

            if (playerStats != null)
            {
                playerStats.TakeFireDamage(damage);
            }
        }
    }
}

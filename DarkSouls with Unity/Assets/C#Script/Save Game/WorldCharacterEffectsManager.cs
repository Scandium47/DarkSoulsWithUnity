using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager instance;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        public TakeBlockedDamageEffect takeBlockedDamageEffect;

        [Header("Poison")]
        public PoisonBuildUpEffect poisonBuildUpEffect;
        public PoisonedEffect poisonedEffect;
        public GameObject poisonFX;
        public AudioClip poisonSFX;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
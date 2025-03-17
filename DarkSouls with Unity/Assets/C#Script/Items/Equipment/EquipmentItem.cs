using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EquipmentItem : Item
    {
        [Header("Defense Bonus")]
        public float physicalDefense;
        public float MagicDefense;
        public float FireDefense;
        public float LightningDefense;
        public float DarknessDefense;

        [Header("Weight")]
        public float weight = 0;

        [Header("Resistance")]
        public float poisonResistance;
    }
}
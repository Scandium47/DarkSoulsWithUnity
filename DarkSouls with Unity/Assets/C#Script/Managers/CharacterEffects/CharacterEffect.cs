using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffect : ScriptableObject
    {
        public int effectID;
        public virtual void ProecssEffect(CharacterManager character)
        {

        }
    }
}
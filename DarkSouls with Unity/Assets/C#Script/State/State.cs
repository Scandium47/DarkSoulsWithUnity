using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public abstract class State : MonoBehaviour
    {
        //³éÏóÀà×´Ì¬³Ø
        public abstract State Tick(EnemyManager aiCharacter);

    }
}
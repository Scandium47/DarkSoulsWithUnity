using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public abstract class State : MonoBehaviour
    {
        //������״̬��
        public abstract State Tick(EnemyManager aiCharacter);

    }
}
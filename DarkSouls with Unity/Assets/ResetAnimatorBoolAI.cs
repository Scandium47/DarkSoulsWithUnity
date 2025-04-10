using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ResetAnimatorBoolAI : ResetAnimatorBool
    {
        public string isPhaseShifting = "isPhaseShifting";
        public bool isPhaseShiftingStatus = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(isPhaseShifting, isPhaseShiftingStatus);
        }

    }
}
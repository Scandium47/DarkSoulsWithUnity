using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class ResetAnimatorBool : StateMachineBehaviour
    {
        public string isInvulnerable = "isInvulnerable";
        public bool isInvulnerableStatus = false;

        public string isInteractingBool = "isInteracting";
        public bool isInteractingStatus = false;

        public string isFiringSpellBool = "isFiringSpell";
        public bool isFiringSpellStatus = false;

        public string isRotatingWithRootMotion = "isRotatingWithRootMotion";
        public bool isRotatingWithRootMotionStatus = false;

        public string canRotateBool = "canRotate";
        public bool canRotateStatus = true;

        public string isMirroredBool = "isMirrored";
        public bool isMirroredStatus = false;

        public string isPerformingFullyChargedAttackBool = "isPerformingFullyChargedAttack";
        public bool isPerformingFullyChargedAttackStatus = false;

        public string isRestByBornfiresBool = "isRestByBornfires";
        public bool isRestByBornfiresStatus = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterManager character = animator.GetComponent<CharacterManager>();

            character.isUsingLeftHand = false;
            character.isUsingRightHand = false;
            character.isAttacking = false;
            character.isBeingBackstabbed = false;
            character.isBeingRiposted = false;
            character.isPerformingBackstab = false;
            character.isPerformingRiposte = false;
            character.canBeParried = false;
            character.canBeRiposted = false;
            character.canRoll = true;

            //伤害动画之后，重置之前的poise伤害到0
            character.characterCombatManager.previousPoiseDamageTaken = 0;

            animator.SetBool(isInteractingBool, isInteractingStatus);
            animator.SetBool(isFiringSpellBool, isFiringSpellStatus);
            animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
            animator.SetBool(canRotateBool, canRotateStatus);
            animator.SetBool(isInvulnerable, isInvulnerableStatus);
            animator.SetBool(isMirroredBool, isMirroredStatus);
            animator.SetBool(isPerformingFullyChargedAttackBool, isPerformingFullyChargedAttackStatus);
            animator.SetBool(isRestByBornfiresBool, isRestByBornfiresStatus);
        }
    }
}

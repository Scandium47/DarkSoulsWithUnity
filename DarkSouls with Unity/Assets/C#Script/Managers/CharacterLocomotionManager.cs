using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;

        public Vector3 moveDirection;
        public LayerMask groundLayer;

        [Header("Gravity Settings")]
        public float inAirTimer;
        [SerializeField] protected Vector3 yVelcoity;
        [SerializeField] protected float groundedYVelocity = -20;   //在地面上时垂直施加的力
        [SerializeField] protected float fallStartYVelocity = -7;   //掉落时垂直施加的力（随时间增加）
        [SerializeField] protected float gravityForce = -25;
        [SerializeField] float groundCheckSphereRadius = 0.22f;
        protected bool fallingVelocitySet = false;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            character.animator.SetBool("isGrounded", character.isGrounded);
            HandleGroundCheck();
        }

        public virtual void HandleGroundCheck()
        {
            if(character.isGrounded)
            {
                if(yVelcoity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocitySet = false;
                    yVelcoity.y = groundedYVelocity;
                }
            }
            else
            {
                if(!fallingVelocitySet)
                {
                    fallingVelocitySet=true;
                    yVelcoity.y = fallStartYVelocity;
                }

                inAirTimer = inAirTimer + Time.deltaTime;
                yVelcoity.y += gravityForce * Time.deltaTime;
            }

            character.animator.SetFloat("inAirTimer", inAirTimer);
            character.characterController.Move(yVelcoity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }
}
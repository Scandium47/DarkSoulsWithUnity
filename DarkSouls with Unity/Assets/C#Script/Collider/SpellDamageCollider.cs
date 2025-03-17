using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class SpellDamageCollider : DamageCollider
    {
        public GameObject impactParticles;      //撞击粒子
        public GameObject projectileParticles;      //飞行粒子
        public GameObject muzzleParticles;      //枪口粒子

        bool hasCollided = false;

        CharacterManager spellTarget;     //施法目标
        new Rigidbody rigidbody;
        Vector3 impactNormal;   //used to rotate the impact particles

        protected override void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();    
        }

        private void Start()
        {
            //以施法对象的位置为父物体
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if(muzzleParticles)     //枪口粒子，几秒销毁
            {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 0.1f);   //How long the muzzle particles last
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if(!hasCollided)    //如果发生碰撞，对目标造成伤害
            {
                spellTarget = other.transform.GetComponent<CharacterManager>();

                if(spellTarget != null && spellTarget.characterStatsManager.teamIDNumber != teamIDNumber)
                {
                    TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                    takeDamageEffect.physicalDamage = physicalDamage;
                    takeDamageEffect.fireDamage = fireDamage;
                    takeDamageEffect.poiseDamage = poiseDamage;
                    takeDamageEffect.contactPoint = contactPoint;
                    takeDamageEffect.angleHitFrom = angleHitFrom;
                    spellTarget.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
                }
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));

                Debug.Log("销毁");
                Destroy(impactParticles, 1f);     //几秒后销毁撞击粒子
                Destroy(gameObject);      //销毁法术物体
            }
        }
    }
}
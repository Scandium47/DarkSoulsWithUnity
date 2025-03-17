using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class SpellDamageCollider : DamageCollider
    {
        public GameObject impactParticles;      //ײ������
        public GameObject projectileParticles;      //��������
        public GameObject muzzleParticles;      //ǹ������

        bool hasCollided = false;

        CharacterManager spellTarget;     //ʩ��Ŀ��
        new Rigidbody rigidbody;
        Vector3 impactNormal;   //used to rotate the impact particles

        protected override void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();    
        }

        private void Start()
        {
            //��ʩ�������λ��Ϊ������
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if(muzzleParticles)     //ǹ�����ӣ���������
            {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 0.1f);   //How long the muzzle particles last
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if(!hasCollided)    //���������ײ����Ŀ������˺�
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

                Debug.Log("����");
                Destroy(impactParticles, 1f);     //���������ײ������
                Destroy(gameObject);      //���ٷ�������
            }
        }
    }
}
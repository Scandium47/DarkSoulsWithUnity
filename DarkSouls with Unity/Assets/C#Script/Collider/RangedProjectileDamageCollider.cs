using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class RangedProjectileDamageCollider : DamageCollider
    {
        public RangedAmmoItem ammoItem;
        protected bool hasAlreadyPenetratedASurface;    //�Ƿ�͸

        Rigidbody arrowRigidbody;
        CapsuleCollider arrowCapsuleCollider;

        protected override void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.enabled = true;

            arrowCapsuleCollider = GetComponent<CapsuleCollider>();
            arrowRigidbody = GetComponent<Rigidbody>();
        }

        // ������÷���
        public void ResetArrowState()
        {
            hasAlreadyPenetratedASurface = false;
            arrowRigidbody.isKinematic = false;
            arrowCapsuleCollider.enabled = true;
            transform.parent = null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            hasBeenParried = false;
            shieldHasBeenHit = false;
            CharacterManager enemyManager = collision.gameObject.GetComponentInParent<CharacterManager>();

            if (enemyManager != null)
            {
                if (enemyManager.characterStatsManager.teamIDNumber == teamIDNumber)
                    return;

                CheckForParry(enemyManager);
                CheckForBlock(enemyManager);

                if (hasBeenParried)
                    return;

                if (shieldHasBeenHit)
                    return;

                enemyManager.characterStatsManager.poiseResetTimer = enemyManager.characterStatsManager.totalPoiseResetTime;
                enemyManager.characterStatsManager.totalPoiseDefence = enemyManager.characterStatsManager.totalPoiseDefence - poiseDamage;
                //Debug.Log("������� " + enemyStats.totalPoiseDefence);

                //����λ�ã����������һ�νӴ���ײ����λ��
                contactPoint = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                angleHitFrom = (Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up));

                TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                takeDamageEffect.physicalDamage = physicalDamage;
                takeDamageEffect.fireDamage = fireDamage;
                takeDamageEffect.poiseDamage = poiseDamage;
                takeDamageEffect.contactPoint = contactPoint;
                takeDamageEffect.angleHitFrom = angleHitFrom;
                enemyManager.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
            }

            if (collision.gameObject.tag == "Illusionary Wall")
            {
                IllusionaryWall illusionaryWall = collision.gameObject.GetComponent<IllusionaryWall>();

                illusionaryWall.wallHasBeenHit = true;
            }

            if(!hasAlreadyPenetratedASurface)
            {
                hasAlreadyPenetratedASurface = true;
                arrowRigidbody.isKinematic = true;      //��������ΪIK����������Ӱ��
                arrowCapsuleCollider.enabled = false;       //���ý�����ײ��

                gameObject.transform.position = collision.GetContact(0).point;      //�״νӴ���
                gameObject.transform.rotation = Quaternion.LookRotation(transform.forward);     //��ת����Ӵ���
                gameObject.transform.parent = collision.collider.transform;     //����Ϊ�Ӵ������������
                // ���ռ�ʸ
                if (ammoItem != null)
                { 
                    StartCoroutine(DestroyObjectAfterDelay(5f));
                }
            }
        }

        private void FixedUpdate()
        {
            if (arrowRigidbody.velocity != Vector3.zero)
            {
                arrowRigidbody.rotation = Quaternion.LookRotation(arrowRigidbody.velocity);
            }
        }

        private IEnumerator DestroyObjectAfterDelay(float delay)
        {
            // �ȴ�ָ����ʱ��
            yield return new WaitForSeconds(delay);
            // ��������
            //Destroy(gameObject);
            // ����״̬
            ResetArrowState();

            // �黹�������
            AmmoPoolManager.Instance.ReturnToPool("Arrow", gameObject);
        }
    }
}
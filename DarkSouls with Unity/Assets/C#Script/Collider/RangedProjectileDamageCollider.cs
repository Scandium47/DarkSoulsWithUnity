using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class RangedProjectileDamageCollider : DamageCollider
    {
        public RangedAmmoItem ammoItem;
        protected bool hasAlreadyPenetratedASurface;    //是否穿透

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

        // 添加重置方法
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
                //Debug.Log("玩家韧性 " + enemyStats.totalPoiseDefence);

                //击中位置，检测武器第一次接触碰撞器的位置
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
                arrowRigidbody.isKinematic = true;      //刚体设置为IK，不受物理影响
                arrowCapsuleCollider.enabled = false;       //禁用胶囊碰撞器

                gameObject.transform.position = collision.GetContact(0).point;      //首次接触点
                gameObject.transform.rotation = Quaternion.LookRotation(transform.forward);     //旋转面向接触点
                gameObject.transform.parent = collision.collider.transform;     //设置为接触物体的子物体
                // 回收箭矢
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
            // 等待指定的时间
            yield return new WaitForSeconds(delay);
            // 销毁物体
            //Destroy(gameObject);
            // 重置状态
            ResetArrowState();

            // 归还到对象池
            AmmoPoolManager.Instance.ReturnToPool("Arrow", gameObject);
        }
    }
}
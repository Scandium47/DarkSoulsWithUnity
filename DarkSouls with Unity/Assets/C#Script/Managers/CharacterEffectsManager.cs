using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Static Effects")]
        [SerializeField] List<StaticCharacterEffect> staticCharacterEffects;

        [Header("Timed Effects")]
        public List<CharacterEffect> timedEffects;
        [SerializeField] float effectTickTimer = 0;

        [Header("Timed Effect Visual FX")]
        public List<GameObject> timedEffectParticles;

        [Header("Current Range FX")]
        public GameObject instantiatedFXModel;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Weapon FX")]
        public WeaponManager rightWeaponManager;
        public WeaponManager leftWeaponManager;

        [Header("Right Weapon Buff")]
        public WeaponBuffEffect rightWeaponBuffEffect;

        [Header("Poison")]
        public Transform buildUpTransform;      //中毒效果的位置

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            foreach (var effect in staticCharacterEffects)
            {
                effect.AddStaticEffect(character);
            }
        }

        public virtual void ProcessEffectInstantly(CharacterEffect effect)
        {
            effect.ProecssEffect(character);
        }

        public virtual void ProcessAllTimedEffects()
        {
            effectTickTimer = effectTickTimer + Time.deltaTime;

            if (effectTickTimer >= 1)
            {
                effectTickTimer = 0;
                ProcessWeaponBuffs();
                
                //在每次tick内处理所有效果
                for (int i = timedEffects.Count -1; i > -1; i--)
                {
                    timedEffects[i].ProecssEffect(character);
                }

                //每次tick缩短毒性条
                ProcessBuildUpDecay();
            }
        }

        public void ProcessWeaponBuffs()
        {
            if(rightWeaponBuffEffect != null)
            {
                rightWeaponBuffEffect.ProecssEffect(character);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect)
        {
            //检查列表确认没有添加同一种效果两次以上
            StaticCharacterEffect staticEffect;

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effect.effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //移除角色身上的效果
                        staticEffect.RemoveStaticEffect(character);
                        //从效果列表中移除效果
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //将效果添加到角色
            staticCharacterEffects.Add(effect);
            //将效果添加到列表中
            effect.AddStaticEffect(character);

            //检查列表有没有空的效果，有的话移除
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public void RemoveStaticEffect(int effectID)
        {
            StaticCharacterEffect staticEffect;

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if(staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //移除角色身上的效果
                        staticEffect.RemoveStaticEffect(character);
                        //从效果列表中移除效果
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //检查列表有没有空的效果，有的话移除
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public virtual void PlayWeaponFX(bool isLeft)
        {
            if(isLeft == false)
            {
                if(rightWeaponManager != null)
                {
                    rightWeaponManager.PlayWeaponTrailFX();
                }
            }
            else
            {
                if(leftWeaponManager != null)
                {
                    leftWeaponManager.PlayWeaponTrailFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation)
        {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }

        public virtual void InterruptEffect()
        {
            //如果弓箭的特效还在的话，中断特效（在喝药或者使用道具时使用该函数）
            if (instantiatedFXModel != null)
            {
                Destroy(instantiatedFXModel);
            }
            //如果正在拉弓，把箭射出去并移除箭头
            if (character.isHoldingArrow)
            {
                character.animator.SetBool("isHoldingArrow", false);
                Animator rangedWeaponAnimator = character.characterWeaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<Animator>();

                if(rangedWeaponAnimator != null)
                {
                    rangedWeaponAnimator.SetBool("isDrawn", false);
                    rangedWeaponAnimator.Play("Bow_TH_Fire_01");
                }
            }

            //如果角色正在瞄准，退出瞄准状态
            if(character.isAiming)
            {
                character.animator.SetBool("isAiming", false);
            }
        }

        protected virtual void ProcessBuildUpDecay()
        {
            if(character.characterStatsManager.poisonBuildup > 0)
            {
                character.characterStatsManager.poisonBuildup -= 1;
            }
        }

        public virtual void AddTimedEffectParticle(GameObject effect)
        {
            GameObject effectGameObject = Instantiate(effect, buildUpTransform);
            timedEffectParticles.Add(effectGameObject);
        }

        public virtual void RemoveTimedEffectParticle(EffectParticleType effectType)
        {
            for (int i = timedEffectParticles.Count - 1; i > -1; i--)
            {
                if (timedEffectParticles[i].GetComponent<EffectParticle>().effectType == effectType)
                {
                    Destroy(timedEffectParticles[i]);
                    timedEffectParticles.RemoveAt(i);
                }
            }
        }
    }
}